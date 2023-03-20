using System.Collections;
using UnityEngine;

public class PhoneScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip phoneSound;
    public float vibrationDuration = 3.0f;

    public void ActivatePhone()
    {
        Debug.Log("blooop");
        StartCoroutine(PhoneSequence());
    }

    private IEnumerator PhoneSequence()
    {
        // Start vibrating and playing the sound
        //Handheld.Vibrate();
        audioSource.clip = phoneSound;
        audioSource.Play();

        // Wait for the vibration duration
        yield return new WaitForSeconds(vibrationDuration);

        // Stop the vibration and the sound
        audioSource.Stop();

        // Call the PickPocketingFailed method in the PickPocketingManager script
        FindObjectOfType<PickPocketingManager>().FailPickPocketing();
    }
}