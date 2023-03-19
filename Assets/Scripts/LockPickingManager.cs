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
    
    public void ActivateLockPicking()
    {
        playerCamera.SetActive(false);
        lockPickingCamera.SetActive(true);
        lockPickingSystem.enabled = true;
        player.SetActive(false);
        enemies.SetActive(false);
    }

    public void DeactivateLockPicking()
    {
        playerCamera.SetActive(true);
        lockPickingCamera.SetActive(false);
        lockPickingSystem.enabled = false;
        player.SetActive(true);
        enemies.SetActive(true);
    }

    void Start()
    {
        DeactivateLockPicking();
    }
}