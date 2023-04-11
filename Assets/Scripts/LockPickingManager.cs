using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPickingManager : MonoBehaviour
{
    public GameObject playerCamera;
    public GameObject lockPickingCamera;
    public LockPickingSystem lockPickingSystem;
    public GameObject player;
    public GameObject enemies;
    public GameObject chest;
    public bool isCombatStarter = false;
    public bool success;
    //public GameObject enemyManagerTwo;
    //private GameObject storedEnemyManagerTwo;
    [SerializeField]
    private GameObject _enemyManagerTwo;

    public GameObject EnemyManagerTwo
    {
        get
        {
            if (_enemyManagerTwo == null)
            {
                _enemyManagerTwo = GameObject.Find("EnemiesManagerTwo");
            }
            return _enemyManagerTwo;
        }
    }
    
    void Start()
    {
        success = false;
        //Debug.Log(isCombatStarter);
        lockPickingCamera.SetActive(false);
        playerCamera.SetActive(true);
        lockPickingSystem.enabled = false;
        player.SetActive(true);
        //enemies.SetActive(true);
        //enemyManagerTwo.GetComponent<EnemyManager>().EngageInCombat(false);
        EnemyManagerTwo.SetActive(false);
        Debug.Log("Start: enemyManagerTwo instance ID = " + EnemyManagerTwo.GetInstanceID());
        //enemyManagerTwo.GetComponent<EnemyManager>().EngageInCombat(false);
    }

    public void ActivateLockPicking()
    {
        playerCamera.SetActive(false);
        lockPickingCamera.SetActive(true);
        lockPickingSystem.enabled = true;
        lockPickingSystem.ActivateLockPicking();
        player.SetActive(false);
        //enemies.SetActive(false);
    }

    public void DeactivateLockPicking()
    {
        lockPickingCamera.SetActive(false);
        playerCamera.SetActive(true);
        lockPickingSystem.enabled = false;
        player.SetActive(true);
        //enemies.SetActive(true);
        //Debug.Log("bloop");
        Debug.Log(success);
        if(success) {
            //Debug.Log("Deactivate: enemyManagerTwo instance ID = " + storedEnemyManagerTwo.GetInstanceID());
            //enemies.SetActive(false);
            EnemyManagerTwo.SetActive(true);
            chest.SetActive(false);
            //EnemyManagerTwo.transform.position = new Vector3(154.6461f, -5.699432f, 80.95973f);
            EnemyManagerTwo.GetComponent<EnemyManager>().begin("EnemyManagerTwo");
            EnemyManagerTwo.GetComponent<EnemyManager>().EngageInCombat(true);

        }
    }

    public void setSuccess(bool set) {
        success = set;
    }
}