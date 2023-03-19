using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockInteraction : MonoBehaviour
{
    public LockPickingManager lockPickingManager;

    // private void Update()
    // {
    //     // Check your criteria (e.g., distance to the lock, player input, etc.)
    //     if (true)
    //     {
    //         lockPickingManager.ActivateLockPicking();
    //     }
    // }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lockPickingManager.ActivateLockPicking();
        }
    }
}
