using UnityEngine;

public class LockPickingSystem : MonoBehaviour
{
    public LockPickingManager lockPickingManager;
    public GameObject lockObject;
    public GameObject lockpickObject;
    public float pickSpeed = 0.5f;
    public float sweetSpotRange = 0.1f;
    public float forceFactor = 1.5f;
    public int maxAttempts = 5;

    private float sweetSpot;
    private float pickPosition;
    private int attempts;

    private float initialDistance;
    public GameObject pivotPoint;
    private float previousAngle;
    public float maxLockRotation = 90.0f;
    private float currentLockRotation;
     private Vector3 initialLockpickLocalPosition;
    private float vibrationStartTime;
    private bool isVibrating = false;
    private float vibrationDuration = 1f; // Adjust the vibration duration
    private Quaternion initialLockpickRotation;
    private float previousVibrationAngle = 5f;
    private Quaternion initialLockRotation;
    private int clickCounter = 0;
    private int maxClicks = 7;
    public GameObject door;



    void Start()
    {
        sweetSpot = Random.Range(0.0f, 1.0f);
        pickPosition = 0.0f;
        attempts = maxAttempts;
        previousAngle = 0.0f;
        // Calculate the initial distance between the lock and lockpick
        initialDistance = Vector3.Distance(lockObject.transform.position, lockpickObject.transform.position);
        currentLockRotation = 0.0f;
        initialLockpickLocalPosition = lockpickObject.transform.localPosition;
        initialLockpickRotation = lockpickObject.transform.localRotation;
        initialLockRotation = lockObject.transform.rotation;
    }

    void UpdateLockpickPosition(float position = -1, float extraRotationZ = 0)
    {
        if (position < 0)
        {
            position = pickPosition;
        }
        
        float angle = Mathf.Lerp(90, -90, position);
        float deltaAngle = angle - previousAngle + extraRotationZ;
        
        lockpickObject.transform.RotateAround(pivotPoint.transform.position, pivotPoint.transform.forward, deltaAngle);

        previousAngle = angle;
    }
    
    private void Awake() {
        sweetSpot = Random.Range(0.0f, 1.0f);
        pickPosition = 0.0f;
        attempts = maxAttempts;
        previousAngle = 0.0f;
        // Calculate the initial distance between the lock and lockpick
        initialDistance = Vector3.Distance(lockObject.transform.position, lockpickObject.transform.position);
        currentLockRotation = 0.0f;
        initialLockpickLocalPosition = lockpickObject.transform.localPosition;
        initialLockpickRotation = lockpickObject.transform.localRotation;
        initialLockRotation = lockObject.transform.rotation;    
    }

    void Update()
    {
        // Move the lockpick based on user mouse movement
        if (!Input.GetMouseButton(0))
        {
            // Move the lockpick based on user mouse movement
            float horizontalInput = Input.GetAxis("Mouse X");
            pickPosition = Mathf.Clamp(pickPosition + horizontalInput * pickSpeed * Time.deltaTime, 0, 1);
            UpdateLockpickPosition();
        }

        if (Input.GetMouseButtonDown(0))
        {
            clickCounter++;
            if (clickCounter >= maxClicks)
            {
                isVibrating = true;
                vibrationStartTime = Time.time;
                Invoke("LockPickFailed", 0.2f);
                clickCounter = 0; // Reset the click counter after the vibration
            }
        }

        // Check if the player tries to turn the lock using the left mouse button
        if (Input.GetMouseButton(0))
        {
            float distanceToSweetSpot = Mathf.Abs(pickPosition - sweetSpot);

            if (distanceToSweetSpot <= sweetSpotRange)
            {
                // Calculate the desired lock rotation based on the distance to the sweet spot
                float desiredLockRotation = Mathf.Lerp(0, maxLockRotation, 1 - distanceToSweetSpot / sweetSpotRange);

                // Calculate the rotation difference and apply it to the lock
                float rotationDelta = (desiredLockRotation - currentLockRotation) * Time.deltaTime * 2.0f;
                lockObject.transform.Rotate(rotationDelta, 0, 0); // Change rotation axis to X

                // Update the current lock rotation
                currentLockRotation += rotationDelta;

                if (currentLockRotation >= maxLockRotation -8f)
                {
                    LockPicked();
                }
            }
            else if (!isVibrating)
            {
                isVibrating = true;
                vibrationStartTime = Time.time;
            }
        }

        if (isVibrating && (!Input.GetMouseButton(0) || Time.time - vibrationStartTime >= vibrationDuration))
        {
            isVibrating = false;
            UpdateLockpickPosition();
        }
        else if (isVibrating)
        {
            VibrateLockpick();
        }

        if (!Input.GetMouseButton(0) && currentLockRotation > 0)
        {
            // Reset lock rotation when the left mouse button is released
            float rotationResetSpeed = 100f * Time.deltaTime; // Adjust the speed at which the lock resets
            float rotationDelta = -Mathf.Min(rotationResetSpeed, currentLockRotation);
            lockObject.transform.Rotate(rotationDelta, 0, 0); // Change rotation axis to X
            currentLockRotation += rotationDelta;
        }
    }

    void VibrateLockpick()
    {
        if (isVibrating && Time.time - vibrationStartTime < vibrationDuration)
        {
            float rotationZ = previousVibrationAngle * -1;
            //Debug.Log(rotationZ);
            lockpickObject.transform.RotateAround(pivotPoint.transform.position, pivotPoint.transform.forward, rotationZ);

            previousVibrationAngle = rotationZ;
        }
        else
        {
            isVibrating = false;
            UpdateLockpickPosition();
        }
    }

    private void LockPicked()
    {
        Debug.Log("Lock picked successfully!");
        lockPickingManager.DeactivateLockPicking();
        door.SetActive(false);
        // Trigger relevant event or action (e.g., opening the door)
    }

    private void LockPickFailed()
    {
        Debug.Log("Lock picking failed!");
        lockPickingManager.DeactivateLockPicking();
        // Exit the lock-picking mode and apply any penalties (e.g., lockpick durability loss)
    }
}