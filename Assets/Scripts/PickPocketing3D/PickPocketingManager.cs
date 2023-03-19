using System.Collections.Generic;
using UnityEngine;

public class PickPocketingManager : MonoBehaviour
{
    public GameObject playerCamera;
    public GameObject pickPocketingCamera;
    public List<GameObject> pocketItems;
    public GameObject targetItem;
    public float disturbanceThreshold = 0.1f;
    
    public void ActivatePickPocketing()
    {
        playerCamera.SetActive(false);
        pickPocketingCamera.SetActive(true);
    }

    public void DeactivatePickPocketing()
    {
        playerCamera.SetActive(true);
        pickPocketingCamera.SetActive(false);
    }

    void Start()
    {
        //DeactivatePickPocketing();
    }

    public void FailPickPocketing()
    {
        Debug.Log("Pick-pocketing failed!");
        DeactivatePickPocketing();
        // Apply any penalties (e.g., alerting the NPC)
    }

    public void SuccessfulPickPocketing()
    {
        Debug.Log("Pick-pocketing successful!");
        //DeactivatePickPocketing();
        // Trigger relevant event or action (e.g., add the stolen item to the player's inventory)
    }
}