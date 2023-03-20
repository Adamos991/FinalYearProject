using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketInteraction : MonoBehaviour
{
    public PickPocketingManager pickPocketingManager;
    private bool isPlayerInsideTrigger = false;

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
            pickPocketingManager.ActivatePickPocketing();
            // Perform your desired action here
        }
    }
}
