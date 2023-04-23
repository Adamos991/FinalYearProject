using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class EnemyManager : MonoBehaviour
{
    private EnemyScript[] enemies;
    public EnemyStruct[] allEnemies;
    private List<int> enemyIndexes;
    private EnemyScript attackingEnemy = null;
    private static int guardCombat = 0;
    

    [Header("Main AI Loop - Settings")]
    private Coroutine AI_Loop_Coroutine;

    public int aliveEnemyCount;
    void Start()
    {
        enemies = GetComponentsInChildren<EnemyScript>();

        allEnemies = new EnemyStruct[enemies.Length];
        for (int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].enemyScript = enemies[i];
            allEnemies[i].enemyAvailability = true;
        }

        StartAI();
    }

    public void begin(string tag) {
        enemies = GetComponentsInChildren<EnemyScript>();

        allEnemies = new EnemyStruct[enemies.Length];

        for (int i = 0; i < allEnemies.Length; i++)
        {
            allEnemies[i].enemyScript = enemies[i];
            enemies[i].begin(tag);
            allEnemies[i].enemyAvailability = true;
        }
        //transform.position = new Vector3(154.6461f, -5.699432f, 80.95973f);
        //StopCoroutine(AI_Loop_Coroutine);
        StartAI();
    }

    public void EngageInCombat(bool engage){
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].EngageInCombatYuh(engage);
        }
    }

    public void StartAI()
    {
        
        AI_Loop_Coroutine = StartCoroutine(AI_Loop(null));
    }

    private void OnEnable() {
        //if(AI_Loop_Coroutine != null) { 
            //StopCoroutine(AI_Loop_Coroutine);
            StartAI();
        //}
    }

    IEnumerator AI_Loop(EnemyScript enemy)
    {
        //Debug.Log("bloop");
        //Debug.Log(AliveEnemyCount());
        if (AliveEnemyCount() == 0)
        {
            StopCoroutine(AI_Loop(null));
            yield break;
        }

        yield return new WaitForSeconds(Random.Range(.5f,1.5f));

        attackingEnemy = RandomEnemyExcludingOne(enemy);

        if (attackingEnemy == null)
            attackingEnemy = RandomEnemy();

        if (attackingEnemy == null)
            yield break;
            
        yield return new WaitUntil(()=>attackingEnemy.IsRetreating() == false);
        yield return new WaitUntil(() => attackingEnemy.IsLockedTarget() == false);
        yield return new WaitUntil(() => attackingEnemy.IsStunned() == false);

        attackingEnemy.SetAttack();

        yield return new WaitUntil(() => attackingEnemy.IsPreparingAttack() == false);

        attackingEnemy.SetRetreat();

        yield return new WaitForSeconds(Random.Range(0,.5f));

        if (AliveEnemyCount() > 0)
            AI_Loop_Coroutine = StartCoroutine(AI_Loop(attackingEnemy));
    }

    public EnemyScript RandomEnemy()
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        EnemyScript randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = allEnemies[enemyIndexes[randomIndex]].enemyScript;

        return randomEnemy;
    }

    public EnemyScript RandomEnemyExcludingOne(EnemyScript exclude)
    {
        enemyIndexes = new List<int>();

        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability && allEnemies[i].enemyScript != exclude)
                enemyIndexes.Add(i);
        }

        if (enemyIndexes.Count == 0)
            return null;

        EnemyScript randomEnemy;
        int randomIndex = Random.Range(0, enemyIndexes.Count);
        randomEnemy = allEnemies[enemyIndexes[randomIndex]].enemyScript;

        return randomEnemy;
    }

    public int AvailableEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyAvailability)
                count++;
        }
        return count;
    }

    public bool AnEnemyIsPreparingAttack()
    {
        foreach (EnemyStruct enemyStruct in allEnemies)
        {
            if (enemyStruct.enemyScript.IsPreparingAttack())
            {
                return true;
            }
        }
        return false;
    }


    public int AliveEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyScript.isActiveAndEnabled)
                count++;
        }
        aliveEnemyCount = count;
        return count;
    }

    public void SetEnemyAvailiability (EnemyScript enemy, bool state)
    {
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].enemyScript == enemy)
                allEnemies[i].enemyAvailability = state;
        }

        if (FindObjectOfType<EnemyDetection>().CurrentTarget() == enemy)
            FindObjectOfType<EnemyDetection>().SetCurrentTarget(null);
    }

    public void initiateGuardCombat(){
        if (Interlocked.CompareExchange(ref guardCombat, 1, 0) == 0)
        {
            Guard[] Guards = GetComponentsInChildren<Guard>();
            foreach(Guard guard in Guards) {
                guard.lightsOff();
                guard.enabled = false;
                //guard.GetComponentInParent<>()
            }
            EnemyScript[] enemies = GetComponentsInChildren<EnemyScript>();
            foreach(EnemyScript enemy in enemies) {
                enemy.enabled = true;
            }
        }
    }
}

[System.Serializable]
public struct EnemyStruct
{
    public EnemyScript enemyScript;
    public bool enemyAvailability;
}
