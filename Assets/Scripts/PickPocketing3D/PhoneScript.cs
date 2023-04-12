using System.Collections;
using UnityEngine;
using System.Threading;
public class PhoneScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip phoneSound;
    public float vibrationDuration = 3.0f;
    public PickPocketingManager pickPocketingManager;
    private Vector3 initialPosition;
    private static int coroutineStarted = 0;
    public PickPocketingTarget ppt;

    public void ActivatePhone()
    {
        //Debug.Log("blooop");
        if (Interlocked.CompareExchange(ref coroutineStarted, 1, 0) == 0)
        {
            StartCoroutine(PhoneSequence());
        }
    }

    private IEnumerator PhoneSequence()
    {
        // Start vibrating and playing the sound
        //Handheld.Vibrate();
        audioSource.clip = phoneSound;
        audioSource.Play();
        gameObject.GetComponent<Rigidbody>().useGravity = true;
        // Wait for the vibration duration
        yield return new WaitForSeconds(vibrationDuration);

        // Stop the vibration and the sound
        audioSource.Stop();

        // Call the PickPocketingFailed method in the PickPocketingManager script
        FindObjectOfType<PickPocketingManager>().FailPickPocketing();
    }

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, initialPosition) > pickPocketingManager.disturbanceThreshold && pickPocketingManager.getSuccess() != 1)
        {
            ppt.setWinnable(false);
            ActivatePhone();
        }
    }
}