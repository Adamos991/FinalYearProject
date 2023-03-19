using UnityEngine;

public class PickPocketingItem : MonoBehaviour
{
    public PickPocketingManager pickPocketingManager;
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, initialPosition) > pickPocketingManager.disturbanceThreshold)
        {
            pickPocketingManager.FailPickPocketing();
        }
    }
}