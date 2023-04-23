using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.AI;
using System.Threading;

public class EnemyScript : MonoBehaviour
{
    //Declarations
    private Animator animator;
    private CombatScript playerCombat;
    private EnemyManager enemyManager;
    private EnemyDetection enemyDetection;
    private CharacterController characterController;
    private Vector3 previousPosition;
    private static int coroutineStarted = 0;

    [Header("Stats")]
    public int health = 3;
    private float moveSpeed = 1;
    private Vector3 moveDirection;

    [Header("States")]
    [SerializeField] private bool isPreparingAttack;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isRetreating;
    [SerializeField] private bool isLockedTarget;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isWaiting = true;
    [SerializeField] private bool isInCombat = true;

    [Header("Polish")]
    //[SerializeField] private ParticleSystem counterParticle;

    [Header("Gravity")]
    public float gravity = -9.81f;
    private Vector3 velocity;

    private Coroutine PrepareAttackCoroutine;
    private Coroutine RetreatCoroutine;
    private Coroutine DamageCoroutine;
    private Coroutine MovementCoroutine;

    //Events
    public UnityEvent<EnemyScript> OnDamage;
    public UnityEvent<EnemyScript> OnStopMoving;
    public UnityEvent<EnemyScript> OnRetreat;

    [Header("Line of Sight")]
    public float lineOfSightDistance = 10f;
    public LayerMask lineOfSightLayerMask;

    private NavMeshAgent navMeshAgent;
    private bool hasLineOfSight;

    [Header("Line of Sight")]
    public LayerMask lineOfSightMask;

    void Awake()
    {
        lineOfSightMask = LayerMask.GetMask("LineOfSightMask");
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = false; // We will enable the NavMeshAgent only when needed
        enemyManager = GetComponentInParent<EnemyManager>();

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        playerCombat = FindObjectOfType<CombatScript>();
        enemyDetection = playerCombat.GetComponentInChildren<EnemyDetection>();

        playerCombat.OnHit.AddListener((x) => OnPlayerHit(x));
        playerCombat.OnCounterAttack.AddListener((x) => OnPlayerCounter(x));
        playerCombat.OnTrajectory.AddListener((x) => OnPlayerTrajectory(x));

        MovementCoroutine = StartCoroutine(EnemyMovement());
        EngageInCombatYuh(true);
    }

    private void OnEnable() {
        if (Interlocked.CompareExchange(ref coroutineStarted, 1, 0) == 0)
        {
            enemyManager.StartAI();
        }
        int LayerEnemy = LayerMask.NameToLayer("Enemy");
        gameObject.layer = LayerEnemy;
        MovementCoroutine = StartCoroutine(EnemyMovement());
        EngageInCombatYuh(true);
    }

    public Animator getEnemyAnimator() {
        return animator;
    }

    private void OnDisable()
    {
        Interlocked.Exchange(ref coroutineStarted, 0);
    }

    public void begin(string tag) {
        //playerCombat.OnTrajectory.AddListener((x) => OnPlayerTrajectory(x));
        playerCombat.begin(tag);
        // playerCombat.OnHit.AddListener((x) => OnPlayerHit(x));
        // playerCombat.OnCounterAttack.AddListener((x) => OnPlayerCounter(x));
        // playerCombat.OnTrajectory.AddListener((x) => OnPlayerTrajectory(x));
        MovementCoroutine = StartCoroutine(EnemyMovement());
        EngageInCombatYuh(true);
    }

    private bool HasLineOfSight()
{
    float distance = 10f;
    Vector3 startPosition = transform.position + new Vector3(0, 0, 0); // Adding Y-axis offset
    Vector3 direction = playerCombat.transform.position - (transform.position + new Vector3(0, -0.5f, 0));
    RaycastHit hit;

    if (Physics.Raycast(startPosition, direction, out hit, distance, lineOfSightMask))
    {
        //Debug.DrawRay(startPosition, direction * hit.distance, Color.yellow);
       // Debug.Log("Hit: " + hit.collider.gameObject.name);

        if (hit.collider.gameObject.CompareTag("Player"))
        {
            return true;
        }
    }
    else
    {
        //Debug.DrawRay(startPosition, direction * distance, Color.red);
    }

    return false;
}

    IEnumerator EnemyMovement()
    {
        //Waits until the enemy is not assigned to no action like attacking or retreating
        yield return new WaitUntil(() => isWaiting == true && isInCombat);

        int randomChance = Random.Range(0, 2);

        if (randomChance == 1)
        {
            int randomDir = Random.Range(0, 2);
            moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
            isMoving = true;
        }
        else
        {
            StopMoving();
        }

        yield return new WaitForSeconds(1);

        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    private void UpdateCombatStatus()
    {
        //Debug.Log(hasLineOfSight);
        hasLineOfSight = HasLineOfSight();
        float distanceToPlayer = Vector3.Distance(transform.position, playerCombat.transform.position);

        if (!isInCombat && hasLineOfSight && distanceToPlayer <= lineOfSightDistance)
        {
            EngageInCombatYuh(true);
            navMeshAgent.enabled = false;
            //Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }
        else if (isInCombat && (!hasLineOfSight || distanceToPlayer > lineOfSightDistance))
        {
            EngageInCombatYuh(false);
            navMeshAgent.enabled = true;
            navMeshAgent.SetDestination(playerCombat.transform.position);
            //Debug.Log("huh");
        }
    }

    void Update()
    {
        //UpdateCombatStatus();
        if (Vector3.Distance(transform.position, playerCombat.transform.position) > 10f)
        {
            transform.LookAt(new Vector3(playerCombat.transform.position.x, transform.position.y, playerCombat.transform.position.z));
            Vector3 direction = (playerCombat.transform.position - transform.position).normalized;
            characterController.Move(direction * moveSpeed * Time.deltaTime * 2);
            animator.SetFloat("InputMagnitude", 1f, .2f, Time.deltaTime);
        } else if (isInCombat)
        {
            // Constantly look at player
            transform.LookAt(new Vector3(playerCombat.transform.position.x, transform.position.y, playerCombat.transform.position.z));

            // Only moves if the direction is set
            MoveEnemy(moveDirection);

            // Apply gravity
            if (characterController.isGrounded)
            {
                velocity.y = 0;
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }
            characterController.Move(velocity * Time.deltaTime);
        } else
        {
            // Update Animator's InputMagnitude based on the NavMeshAgent's velocity
            float inputMagnitude = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
            animator.SetFloat("InputMagnitude", inputMagnitude, .2f, Time.deltaTime);
        }
    }

    public void EngageInCombatYuh(bool engage)
    {
        isInCombat = engage;

        if (engage)
        {
            // Start enemy movement if the enemy is engaging in combat
            if (MovementCoroutine == null)
            {
                MovementCoroutine = StartCoroutine(EnemyMovement());
            }
        }
        else
        {
            // Stop enemy movement and reset moveDirection if the enemy is disengaging from combat
            StopMoving();
            moveDirection = Vector3.zero;

            if (MovementCoroutine != null)
            {
                StopCoroutine(MovementCoroutine);
                MovementCoroutine = null;
            }
        }
    }

    //Listened event from Player Animation
    void OnPlayerHit(EnemyScript target)
    {
        if (target == this)
        {
            StopEnemyCoroutines();
            DamageCoroutine = StartCoroutine(HitCoroutine());

            enemyDetection.SetCurrentTarget(null);
            isLockedTarget = false;
            OnDamage.Invoke(this);
            if(playerCombat.getLanded()) {
                health--;

                if (health <= 0)
                {
                    Death();
                    return;
                }
                
                animator.SetTrigger("Hit");
                transform.DOMove(transform.position - (transform.forward / 2), .3f).SetDelay(.1f);
            } else {
                animator.SetTrigger("Block");
            }

            StopMoving();
        }

        IEnumerator HitCoroutine()
        {
            isStunned = true;
            yield return new WaitForSeconds(.5f);
            isStunned = false;
        }
    }

    void OnPlayerCounter(EnemyScript target)
    {
        if (target == this)
        {
            PrepareAttack(false);
        }
    }

    void OnPlayerTrajectory(EnemyScript target)
    {
        if (target == this)
        {
            StopEnemyCoroutines();
            isLockedTarget = true;
            PrepareAttack(false);
            StopMoving();
        }
    }

    void Death()
    {
        StopEnemyCoroutines();

        this.enabled = false;
        characterController.enabled = false;
        animator.SetTrigger("Death");
        enemyManager.SetEnemyAvailiability(this, false);
    }

    public void SetRetreat()
    {
        StopEnemyCoroutines();

        RetreatCoroutine = StartCoroutine(PrepRetreat());

        IEnumerator PrepRetreat()
        {
            yield return new WaitForSeconds(1.4f);
            OnRetreat.Invoke(this);
            isRetreating = true;
            moveDirection = -Vector3.forward;
            isMoving = true;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, playerCombat.transform.position) > 4);
            isRetreating = false;
            StopMoving();

            //Free 
            isWaiting = true;
            MovementCoroutine = StartCoroutine(EnemyMovement());
        }
    }

    public void SetAttack()
    {
        isWaiting = false;

        PrepareAttackCoroutine = StartCoroutine(PrepAttack());

        IEnumerator PrepAttack()
        {
            PrepareAttack(true);
            yield return new WaitForSeconds(.2f);
            moveDirection = Vector3.forward;
            isMoving = true;
        }
    }


    void PrepareAttack(bool active)
    {
        isPreparingAttack = active;

        if (active)
        {
            //counterParticle.Play();
        }
        else
        {
            StopMoving();
            //counterParticle.Clear();
            //counterParticle.Stop();
        }
    }

    void MoveEnemy(Vector3 direction)
    {
        //Set movespeed based on direction
        moveSpeed = 1;

        if (direction == Vector3.forward)
            moveSpeed = 5;
        if (direction == -Vector3.forward)
            moveSpeed = 2;

        Vector3 currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        //Set Animator values
        animator.SetFloat("InputMagnitude", Mathf.Abs(currentVelocity.z) / moveSpeed, .2f, Time.deltaTime);
        animator.SetBool("Strafe", (direction == Vector3.right || direction == Vector3.left));
        animator.SetFloat("StrafeDirection", direction.normalized.x, .2f, Time.deltaTime);

        //Don't do anything if isMoving is false
        if (!isMoving)
            return;

        Vector3 dir = (playerCombat.transform.position - transform.position).normalized;
        Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir; //Vector perpendicular to direction
        Vector3 movedir = Vector3.zero;

        Vector3 finalDirection = Vector3.zero;

        if (direction == Vector3.forward)
            finalDirection = dir;
        if (direction == Vector3.right || direction == Vector3.left)
            finalDirection = (pDir * direction.normalized.x);
        if (direction == -Vector3.forward)
            finalDirection = -transform.forward;

        if (direction == Vector3.right || direction == Vector3.left)
            moveSpeed /= 1.5f;

        movedir += finalDirection * moveSpeed * Time.deltaTime;

        characterController.Move(movedir);

        if (!isPreparingAttack)
            return;

        if(Vector3.Distance(transform.position, playerCombat.transform.position) < 2)
        {
            StopMoving();
            if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
                Attack();
            else
                PrepareAttack(false);
        }
    }

    private void Attack()
    {
        transform.DOMove(transform.position + (transform.forward / 1), .5f);
        animator.SetTrigger("AirPunch");
    }

    public void HitEvent()
    {
        if(!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
            playerCombat.DamageEvent();

        PrepareAttack(false);
    }

    public void StopMoving()
    {
        isMoving = false;
        moveDirection = Vector3.zero;
        if(characterController.enabled)
            characterController.Move(moveDirection);
    }

    void StopEnemyCoroutines()
    {
        PrepareAttack(false);

        if (isRetreating)
        {
            if (RetreatCoroutine != null)
                StopCoroutine(RetreatCoroutine);
        }

        if (PrepareAttackCoroutine != null)
            StopCoroutine(PrepareAttackCoroutine);

        if(DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);

        if (MovementCoroutine != null)
            StopCoroutine(MovementCoroutine);
    }

    #region Public Booleans

    public bool IsAttackable()
    {
        return health > 0;
    }

    public bool IsPreparingAttack()
    {
        return isPreparingAttack;
    }

    public bool IsRetreating()
    {
        return isRetreating;
    }

    public bool IsLockedTarget()
    {
        return isLockedTarget;
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    #endregion
}
