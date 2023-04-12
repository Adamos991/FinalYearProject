using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PickPocketingManager : MonoBehaviour
{
    public GameObject playerCamera;
    public GameObject pickPocketingCamera;
    public Guard guard;
    public List<GameObject> pocketItems;
    public GameObject targetItem;
    public float disturbanceThreshold = 0.03f;
    public GameObject player;
    private static int failPickPocketing = 0;
    private int success = 0;
    //public PickPocketingTarget puzzle;

    public void ActivatePickPocketing()
    {
        playerCamera.SetActive(false);
        player.SetActive(false);
        //enemies.SetActive(false);
        pickPocketingCamera.SetActive(true);
        
        //guard.Freeze(true);
        //puzzle.SetActive(true);
    }

    public void DeactivatePickPocketing()
    {
        pickPocketingCamera.SetActive(false);
        playerCamera.SetActive(true);
        player.SetActive(true);
        //guard.Freeze(false);
        //enemies.SetActive(true);
        //puzzle.SetActive(false);
    }

    public int getSuccess() {
        return success;
    }

    void Start()
    {
        DeactivatePickPocketing();
    }

    public void FailPickPocketing()
    {
        if (Interlocked.CompareExchange(ref failPickPocketing, 1, 0) == 0)
        {
            guard.initiateCombat();
            Debug.Log("Pick-pocketing failed!");
            DeactivatePickPocketing();
        }
        // Apply any penalties (e.g., alerting the NPC)
    }

    public void SuccessfulPickPocketing()
    {
        success = 1;
        Debug.Log("Pick-pocketing successful!");
        DeactivatePickPocketing();
        // Trigger relevant event or action (e.g., add the stolen item to the player's inventory)
    }
}