using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cinemachine;
using FixedSizeQueueNamespace;
using Accord.MachineLearning;
//using Accord.Math;
using Accord.Statistics.Filters;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Accord.MachineLearning.DecisionTrees;
using Accord.MachineLearning.DecisionTrees.Learning;
using System.Data;
using System.Linq;

public class CombatScript : MonoBehaviour
{
    private EnemyManager enemyManager;
    private EnemyDetection enemyDetection;
    private MovementInput movementInput;
    private Animator animator;
    private CinemachineImpulseSource impulseSource;
    
    [Header("Target")]
    private EnemyScript lockedTarget;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown;

    [Header("States")]
    public bool isAttackingEnemy = false;
    public bool isCountering = false;

    [Header("Public References")]
    [SerializeField] private Transform punchPosition;
    //[SerializeField] private ParticleSystemScript punchParticle;
    [SerializeField] private GameObject lastHitCamera;
    [SerializeField] private Transform lastHitFocusObject;

    //Coroutines
    private Coroutine counterCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine damageCoroutine;

    [Space]

    //Events
    public UnityEvent<EnemyScript> OnTrajectory;
    public UnityEvent<EnemyScript> OnHit;
    public UnityEvent<EnemyScript> OnCounterAttack;

    int animationCount = 0;
    string[] attacks;

    private List<int[]> attackHistory;
    private DecisionTree tree;
    public DecisionTreeClassifier DTC;
    private Codification codebook;
    private int combatSequenceCounter;
    public int trainingThreshold = 2; // Change this value to set the number of combat sequences before training the classifier
    private bool spaceHeld = false;
    public bool trained = false;

    public List<int> currentAttackSequence;

    void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        //Debug.Log(enemyManager);
        animator = GetComponent<Animator>();
        enemyDetection = GetComponentInChildren<EnemyDetection>();
        movementInput = GetComponent<MovementInput>();
        impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        attackHistory = new List<int[]>();
        currentAttackSequence = new List<int>();
        combatSequenceCounter = 0;
        var inputActions = GetComponent<PlayerInput>().actions;
        inputActions["Space"].started += ctx => OnSpaceStarted();
        inputActions["Space"].canceled += ctx => OnSpaceEnded();
        attacks = new string[] { "AirKick", "AirKick2", "AirPunch", "AirKick3" };
        // Create a DataTable for the attack labels
        DataTable attackTable = new DataTable("Attacks");
        attackTable.Columns.Add("attack", typeof(string));

        // Add the attack labels to the DataTable
        foreach (string attackL in attacks)
        {
            attackTable.Rows.Add(attackL);
        }

        // Create and initialize the codebook
        codebook = new Codification(attackTable);
    }
    public void begin(string tag) {
        enemyManager = GameObject.FindWithTag(tag).GetComponent<EnemyManager>();
        //Debug.Log(enemyManager);
        //enemyManager = FindObjectOfType<EnemyManager>();
        //Debug.Log(enemyManager);
        //animator = GetComponent<Animator>();
        //enemyDetection = GetComponentInChildren<EnemyDetection>();
        //movementInput = GetComponent<MovementInput>();
        //impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        //attackHistory = new List<int[]>();
        currentAttackSequence = new List<int>();
        combatSequenceCounter = 0;
    }
    void AttackCheck(string attackLabel)
    {
        if (isAttackingEnemy)
            return;

        //Check to see if the detection behavior has an enemy set
        if (enemyDetection.CurrentTarget() == null)
        {
            if (enemyManager.AliveEnemyCount() == 0)
            {
                Attack(null, 0, attackLabel);
                return;
            }
            else
            {
                lockedTarget = enemyManager.RandomEnemy();
            }
        }

        //If the player is moving the movement input, use the "directional" detection to determine the enemy
        if (enemyDetection.InputMagnitude() > .2f)
            lockedTarget = enemyDetection.CurrentTarget();

        //Extra check to see if the locked target was set
        if(lockedTarget == null)
            lockedTarget = enemyManager.RandomEnemy();

        //AttackTarget
        Attack(lockedTarget, TargetDistance(lockedTarget), attackLabel);
    }

    public void SwitchEnemyManager(EnemyManager newEnemyManager)
    {
        enemyManager = newEnemyManager;
    }

    public void TrainClassifier()
    {
        // List<int[]> inputData = new List<int[]>();
        // List<int> outputData = new List<int>();
        Debug.Log("training");
        tree = DTC.TrainDecisionTree(attackHistory.ToArray(), 4);
        Debug.Log("trained");
        trained = true;
    }

    //@override
    public void Attack(EnemyScript target, float distance, string attackLabel)
    {
        //Types of attack animation
        attacks = new string[] { "AirKick", "AirKick2", "AirPunch", "AirKick3" };

        //Attack nothing in case target is null
        if (target == null)
        {
            AttackType("GroundPunch", .2f, null, 0);
            return;
        }

        if (distance < 10)
        {
            animationCount = (int)Mathf.Repeat((float)animationCount + 1, (float)attacks.Length);
            //string attackString = isLastHit() ? attacks[Random.Range(0, attacks.Length)] : attacks[animationCount];
            int attackCode = codebook.Transform("attack", attackLabel);
            if (!trained || currentAttackSequence.Count < 4)
            {
                AttackType(attackLabel, attackCooldown, target, .65f);
            }
            else
            {
                foreach(int x in currentAttackSequence) {
                    Debug.Log(x);
                }
                if (PerformAttack(currentAttackSequence, attackCode))
                {
                    AttackType(attackLabel, attackCooldown, target, .65f);
                }
                else
                {
                    currentAttackSequence.Add(attackCode);
                }
            }
            //attackHistory.Add(codebook.Transform("attack", attackLabel));
        }
        else
        {
            lockedTarget = null;
            AttackType("GroundPunch", .2f, null, 0);
            
        }

        //Change impulse
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = Mathf.Max(3, 1 * distance);

    }

    private bool PerformAttack(List<int> currentAttacks, int attackCode)
    {
        int windowSize = 4; // The window size used during training
        int predictionWindowSize = 4; // The number of actions you want to use for prediction

        // Take the last 'predictionWindowSize' elements from 'currentAttackSequence'
        List<int> inputSample = currentAttacks.Skip(currentAttacks.Count - predictionWindowSize).Take(predictionWindowSize).ToList();

        // Pad the input sample with -1 to match the window size
        while (inputSample.Count < windowSize)
        {
            inputSample.Insert(0, -1);
        }

        int predictedAttack = DTC.PredictAttack(tree, inputSample);

        if (predictedAttack == attackCode)
        {
            // The prediction was correct; the enemy blocks the attack
            Debug.Log("Attack blocked!");
            return false; // Attack is not a hit
        }
        else
        {
            // The prediction was incorrect; the attack is a hit
            Debug.Log("Attack hit!");
            return true; // Attack is a hit
        }
    }

    void AttackType(string attackTrigger, float cooldown, EnemyScript target, float movementDuration)
    {
        animator.SetTrigger(attackTrigger);
        if(attackTrigger != "GroundPunch") {
            currentAttackSequence.Add(codebook.Transform("attack", attackTrigger));
        }

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(isLastHit() ? 1.5f : cooldown));

        //Check if last enemy
        if (isLastHit()) {
            //Debug.Log("lasthit");
            // foreach(var x in currentAttackSequence) {
            //     Debug.Log(x);
            // }
            StartCoroutine(FinalBlowCoroutine());
            int[] temp = currentAttackSequence.ToArray();
            attackHistory.Add(temp);
            currentAttackSequence = new List<int>();
        }
        if (target == null) {
            return;
        }
        target.StopMoving();
        MoveTorwardsTarget(target, movementDuration);

        IEnumerator AttackCoroutine(float duration)
        {
            movementInput.acceleration = 0;
            isAttackingEnemy = true;
            movementInput.enabled = false;
            yield return new WaitForSeconds(duration);
            isAttackingEnemy = false;
            yield return new WaitForSeconds(.2f);
            movementInput.enabled = true;
            LerpCharacterAcceleration();
        }

        IEnumerator FinalBlowCoroutine()
        {
            Time.timeScale = .5f;
            lastHitCamera.SetActive(true);
            lastHitFocusObject.position = lockedTarget.transform.position;
            yield return new WaitForSecondsRealtime(2);
            lastHitCamera.SetActive(false);
            Time.timeScale = 1f;
        }
        
        
    }

    void MoveTorwardsTarget(EnemyScript target, float duration)
    {
        OnTrajectory.Invoke(target);
        transform.DOLookAt(target.transform.position, .2f);
        transform.DOMove(TargetOffset(target.transform), duration);
    }

    void CounterCheck()
    {
        //Initial check
        if (isCountering || isAttackingEnemy || !enemyManager.AnEnemyIsPreparingAttack())
            return;

        lockedTarget = ClosestCounterEnemy();
        //OnCounterAttack.Invoke(lockedTarget);
        if(TargetDistance(lockedTarget) < 4) {
            OnCounterAttack.Invoke(lockedTarget);
            if (TargetDistance(lockedTarget) > 2)
            {
                Attack(lockedTarget, TargetDistance(lockedTarget), null);
                return;
            }

            float duration = .2f;
            animator.SetTrigger("Dodge");
            transform.DOLookAt(lockedTarget.transform.position, .2f);
            transform.DOMove(transform.position + lockedTarget.transform.forward, duration);

            if (counterCoroutine != null)
                StopCoroutine(counterCoroutine);
            counterCoroutine = StartCoroutine(CounterCoroutine(duration));

            IEnumerator CounterCoroutine(float duration)
            {
                isCountering = true;
                movementInput.enabled = false;
                yield return new WaitForSeconds(duration);
                Attack(lockedTarget, TargetDistance(lockedTarget), null);
                isCountering = false;

            }
        }
    }

    float TargetDistance(EnemyScript target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    public UnityEngine.Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, transform.position, .95f);
    }

    public void HitEvent()
    {
        if (lockedTarget == null || enemyManager.AliveEnemyCount() == 0)
            return;

        OnHit.Invoke(lockedTarget);

        //Polish
        //punchParticle.PlayParticleAtPosition(punchPosition.position);
    }

    public void DamageEvent()
    {
        animator.SetTrigger("Hit");

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        damageCoroutine = StartCoroutine(DamageCoroutine());

        IEnumerator DamageCoroutine()
        {
            movementInput.enabled = false;
            yield return new WaitForSeconds(.5f);
            movementInput.enabled = true;
            LerpCharacterAcceleration();
        }
    }

    EnemyScript ClosestCounterEnemy()
    {
        float minDistance = 100;
        int finalIndex = 0;

        for (int i = 0; i < enemyManager.allEnemies.Length; i++)
        {
            EnemyScript enemy = enemyManager.allEnemies[i].enemyScript;

            if (enemy.IsPreparingAttack())
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(transform.position, enemy.transform.position);
                    finalIndex = i;
                }
            }
        }

        return enemyManager.allEnemies[finalIndex].enemyScript;

    }

    void LerpCharacterAcceleration()
    {
        movementInput.acceleration = 0;
        DOVirtual.Float(0, 1, .6f, ((acceleration)=> movementInput.acceleration = acceleration));
    }

    bool isLastHit()
    {
        if (lockedTarget == null)
            return false;

        //Debug.Log(enemyManager.AliveEnemyCount() == 1 && lockedTarget.health <= 1);
        return enemyManager.AliveEnemyCount() == 1 && lockedTarget.health <= 1;
    }

    #region Input

    // private void OnCounter()
    // {
    //     CounterCheck();
    // }

    private void OnAttack()
    {
        if (spaceHeld)
        {
            // Handle space + left click attack
            AttackCheck("AirPunch");
        }
        else
        {
            AttackCheck("AirKick");
        }
    }

    private void OnAttackRight() {
        if (spaceHeld)
        {
            // Handle space + left click attack
            AttackCheck("AirKick3");
        }
        else
        {
            AttackCheck("AirKick2");
        }
    }

    private void OnSpaceStarted()
    {
        spaceHeld = true;
    }

    private void OnSpaceEnded()
    {
        spaceHeld = false;
    }
    
    #endregion

}
