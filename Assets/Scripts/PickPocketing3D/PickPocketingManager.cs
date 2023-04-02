using System.Collections.Generic;
using UnityEngine;

public class PickPocketingManager : MonoBehaviour
{
    public GameObject playerCamera;
    public GameObject pickPocketingCamera;
    public List<GameObject> pocketItems;
    public GameObject targetItem;
    public float disturbanceThreshold = 0.1f;
    public GameObject player;
    public GameObject enemies;
    //public PickPocketingTarget puzzle;

    public void ActivatePickPocketing()
    {
        playerCamera.SetActive(false);
        player.SetActive(false);
        enemies.SetActive(false);
        pickPocketingCamera.SetActive(true);
        //puzzle.SetActive(true);
    }

    public void DeactivatePickPocketing()
    {
        pickPocketingCamera.SetActive(false);
        //playerCamera.SetActive(true);
        //player.SetActive(true);
        //enemies.SetActive(true);
        //puzzle.SetActive(false);
    }

    void Start()
    {
        DeactivatePickPocketing();
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
        DeactivatePickPocketing();
        // Trigger relevant event or action (e.g., add the stolen item to the player's inventory)
    }
}