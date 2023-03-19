using UnityEngine;
using UnityEngine.EventSystems;

public class retiredpuzzlegame : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform puzzleSquare;
    public RectTransform targetRegion;
    public float dragSpeed = 100f;
    public float maxForce = 10f;

    private Vector2 originalPosition;
    private Rigidbody2D rb;
    private Vector2 lastCollisionNormal;
    private Vector2 targetPosition;
    private bool isDragging;

    private void Start()
    {
        originalPosition = puzzleSquare.anchoredPosition;
        rb = puzzleSquare.GetComponent<Rigidbody2D>();
        targetPosition = originalPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        targetPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void FixedUpdate()
    {
        if (isDragging)
        {
            // Calculate the direction towards the target position
            Vector2 direction = (targetPosition - (Vector2)puzzleSquare.position).normalized;

            // Calculate the distance to the target position
            float distance = Vector2.Distance(puzzleSquare.position, targetPosition);

            // Calculate the force to be applied
            float forceMagnitude = Mathf.Clamp(distance, 0f, maxForce) * dragSpeed;
            Vector2 force = direction * forceMagnitude;

            // Apply the force to the puzzle square
            rb.AddForce(force, ForceMode2D.Force);
        }
        else
        {
            // Stop the puzzle square from moving
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}