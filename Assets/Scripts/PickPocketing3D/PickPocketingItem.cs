using System.Collections;
using UnityEngine;
using System.Threading;

public class PickPocketingItem : MonoBehaviour
{
    public PickPocketingManager pickPocketingManager;
    private Vector3 initialPosition;
    private static int coroutineStarted = 0;
    public PickPocketingTarget ppt;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, initialPosition) > pickPocketingManager.disturbanceThreshold && pickPocketingManager.getSuccess() != 1)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            ppt.setWinnable(false);
            if (Interlocked.CompareExchange(ref coroutineStarted, 1, 0) == 0)
            {
                //gameObject.GetComponent<Rigidbody>().useGravity = true;
                 StartCoroutine(WaitSequence());
            }
        }
    }

    private IEnumerator WaitSequence()
    {
        Debug.Log("starting");
        yield return new WaitForSeconds(3f);
        Debug.Log("finishing");
        // Stop the vibration and the sound
        pickPocketingManager.FailPickPocketing();
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }
}