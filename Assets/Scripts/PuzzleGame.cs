using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleGame : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform puzzleSquare;
    public RectTransform targetRegion;
    public float dragSpeed = 100f;
    public float forceMagnitude = 100f;

    public GameObject canvas;
    public GameObject door;
    public GameObject playerCamRig;

    private Vector2 originalPosition;
    private Rigidbody2D rb;
    private Vector2 lastCollisionNormal;

    private Vector2 puzzleSquarePos;
    private Vector2 minSquarePos;
    private Vector2 maxSquarePos;

    // The minimum and maximum positions of the target region
    private Vector2 minTargetPos;
    private Vector2 maxTargetPos;

    // The size of the target region
    private Vector2 targetSize;

    private bool isCollidingWithBarrier = false;
    private bool isDragging = false;

    private void Start()
    {
        originalPosition = puzzleSquare.anchoredPosition;
        rb = puzzleSquare.GetComponent<Rigidbody2D>();
        isCollidingWithBarrier = false;
    }

    void Awake()
    {
        // Calculate the minimum and maximum positions of the target region
        minTargetPos = (Vector2)targetRegion.position - targetRegion.rect.size / 2;
        maxTargetPos = (Vector2)targetRegion.position + targetRegion.rect.size / 2;

        // Get the size of the target region
        targetSize = targetRegion.rect.size;
    }

    private void FixedUpdate()
    {
        rb.gravityScale = isDragging ? 0 : 5;
        if (isCollidingWithBarrier && isDragging)
        {
            rb.AddForce(lastCollisionNormal * forceMagnitude, ForceMode2D.Force);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        if (!isCollidingWithBarrier)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            Vector2 newPosition = puzzleSquare.anchoredPosition + eventData.delta;
            puzzleSquare.anchoredPosition = newPosition;
        }
    }

    private bool IsPuzzleSquareInTargetRegion()
    {
        // Get the position of the puzzle square image
        Vector2 puzzleSquarePos = puzzleSquare.position;

        // Calculate the minimum and maximum positions of the puzzle square image
        Vector2 minSquarePos = puzzleSquarePos - puzzleSquare.rect.size / 2;
        Vector2 maxSquarePos = puzzleSquarePos + puzzleSquare.rect.size / 2;

        // Check if the puzzle square is within the target region
        return (minSquarePos.x > minTargetPos.x &&
                maxSquarePos.x < maxTargetPos.x &&
                minSquarePos.y > minTargetPos.y &&
                maxSquarePos.y < maxTargetPos.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        if (!isCollidingWithBarrier)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (IsPuzzleSquareInTargetRegion())
        {
            Debug.Log("Puzzle solved!");
            // Reset the puzzle square image's position
            puzzleSquare.anchoredPosition = originalPosition;
            // Hide the puzzle panel
            canvas.SetActive(false);
            door.SetActive(false);
            playerCamRig.SetActive(true);
            //gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Barrier") && isDragging)
        {
            isCollidingWithBarrier = true;
            lastCollisionNormal = collision.contacts[0].normal;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Barrier") && isDragging)
        {
            isCollidingWithBarrier = false;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}