using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockInteraction : MonoBehaviour
{
    public LockPickingManager lockPickingManager;
    private bool isPlayerInsideTrigger = false;
    //public bool isCombatStarter = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInsideTrigger = false;
        }
    }

    private void Update()
    {
        if (isPlayerInsideTrigger && Input.GetKeyDown(KeyCode.E))
        {
            lockPickingManager.ActivateLockPicking();
            // Perform your desired action here
        }
    }
}
