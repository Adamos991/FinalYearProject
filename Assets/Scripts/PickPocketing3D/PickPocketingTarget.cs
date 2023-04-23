using UnityEngine;

public class PickPocketingTarget : MonoBehaviour
{
    public PickPocketingManager pickPocketingManager;
    public Collider pocketCollider;
    private bool isDragging = false;
    public Camera pocketcamera;
    public LayerMask phoneLayer; 
    //private bool phoneDetected = false;
    public Collider restrictedCollider;
    private Rigidbody rb;
    private bool winnable = true;

    void Awake() {
        rb = GetComponent<Rigidbody>();
        //rb.isKinematic = true;
    }

    public void setWinnable(bool value) {
        winnable = value;
    }

    void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        RaycastHit hit;
        Ray ray = pocketcamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.transform == transform)
            {
                isDragging = true;
                //rb.isKinematic = false;
            }
        }
    }

    if (Input.GetMouseButtonUp(0))
    {
        isDragging = false;
        //rb.isKinematic = true;
    }

    if (isDragging)
    {
        Ray ray = pocketcamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        
        UpdateDragPosition(ray);
        

        if (pocketCollider.bounds.Contains(transform.position) && winnable)
        {
            pickPocketingManager.SuccessfulPickPocketing();
        }
    }
}

    private void UpdateDragPosition(Ray ray)
    {
        Plane plane = new Plane(Vector3.forward, transform.position);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 newPosition = ray.GetPoint(distance);
            newPosition.z = transform.position.z;

            rb.velocity = (newPosition - transform.position) * 10.0f; // Adjust the speed multiplier as needed
        }
    }
}