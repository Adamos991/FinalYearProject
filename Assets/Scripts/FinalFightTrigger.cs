using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalFightTrigger : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyManager;

    public CombatScript player;

    public GameObject EnemyManager
    {
        get
        {
            if (_enemyManager == null)
            {
                _enemyManager = GameObject.Find("EnemyManager");
            }
            return _enemyManager;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        EnemyManager.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnemyManager.SetActive(true);
            EnemyManager.GetComponent<EnemyManager>().begin("EnemyManager");
            EnemyManager.GetComponent<EnemyManager>().EngageInCombat(true);
            player.TrainClassifier();
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
