using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class MazePuzzleController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Image backgroundImage;
    public RectTransform pathImage;
    public GameObject startPoint;
    public GameObject endPoint;
    public int gridSize = 5;
    public Color lineColor = Color.black;
    public float lineThickness = 5f;

    private RectTransform pathRectTransform;
    private Vector2Int startGridPoint;
    private Vector2Int endGridPoint;
    private List<Vector2Int> playerPath = new List<Vector2Int>();

    private void Start()
    {
        pathRectTransform = pathImage.GetComponent<RectTransform>();
        startGridPoint = WorldToGrid(startPoint.transform.position);
        endGridPoint = WorldToGrid(endPoint.transform.position);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Vector2Int gridPoint = WorldToGrid(eventData.position);
        if (gridPoint == startGridPoint)
        {
            playerPath.Clear();
            playerPath.Add(gridPoint);
            DrawLine();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2Int gridPoint = WorldToGrid(eventData.position);
        if (playerPath.Count > 0 && IsValidMove(playerPath[playerPath.Count - 1], gridPoint))
        {
            playerPath.Add(gridPoint);
            DrawLine();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (playerPath.Count > 0 && playerPath[playerPath.Count - 1] == endGridPoint)
        {
            if (IsSolutionCorrect())
            {
                Debug.Log("Puzzle solved!");
            }
            else
            {
                Debug.Log("Incorrect solution.");
            }
        }

        playerPath.Clear();
        DrawLine();
    }

    private Vector2Int WorldToGrid(Vector2 worldPos)
    {
        float x = (worldPos.x + backgroundImage.rectTransform.rect.width / 2f) / backgroundImage.rectTransform.rect.width;
        float y = (worldPos.y + backgroundImage.rectTransform.rect.height / 2f) / backgroundImage.rectTransform.rect.height;
        return new Vector2Int(Mathf.FloorToInt(x * gridSize), Mathf.FloorToInt(y * gridSize));
    }

    private bool IsValidMove(Vector2Int from, Vector2Int to)
    {
        Vector2Int delta = to - from;
        return (Mathf.Abs(delta.x) == 1 && delta.y == 0) || (delta.x == 0 && Mathf.Abs(delta.y) == 1);
    }

    private void DrawLine()
    {
        // Clear all existing lines
        foreach (Transform child in pathRectTransform)
        {
            Destroy(child.gameObject);
        }

        if (playerPath.Count == 0)
        {
            return;
        }

        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, lineColor);
        texture.Apply();

        Vector2 previousPoint = GridToCanvas(playerPath[0]);
        for (int i = 1; i < playerPath.Count; i++)
        {
            Vector2 currentPoint = GridToCanvas(playerPath[i]);
            DrawLineOnCanvas(previousPoint, currentPoint, lineThickness, texture);
            previousPoint = currentPoint;
        }
    }

    private Vector2 GridToCanvas(Vector2Int gridPos)
    {
        float x = gridPos.x * (backgroundImage.rectTransform.rect.width / gridSize) - backgroundImage.rectTransform.rect.width / 2f;
        float y = gridPos.y * (backgroundImage.rectTransform.rect.height / gridSize) - backgroundImage.rectTransform.rect.height / 2f;
        return new Vector2(x, y);
    }

    private void DrawLineOnCanvas(Vector2 pointA, Vector2 pointB, float thickness, Texture2D texture)
    {
        Vector2 differenceVector = pointB - pointA;
        float rotation = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        float distance = Vector2.Distance(pointA, pointB);

        RectTransform line = new GameObject("Line", typeof(RectTransform)).GetComponent<RectTransform>();
        line.SetParent(pathRectTransform);
        line.SetAsFirstSibling(); // Optional: place the line below other UI elements
        line.anchorMin = new Vector2(0.5f, 0.5f);
        line.anchorMax = new Vector2(0.5f, 0.5f);
        line.pivot = new Vector2(0, 0.5f);
        line.sizeDelta = new Vector2(distance, thickness);
        line.anchoredPosition = pointA + differenceVector * 0.5f;
        line.localEulerAngles = new Vector3(0, 0, rotation);

        RawImage rawImage = line.gameObject.AddComponent<RawImage>(); 
        rawImage.texture = texture;
    }

    private bool IsSolutionCorrect()
    {
        // Define your solution here as a list of Vector2Int grid coordinates
        // Example: new List<Vector2Int> { new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) };
        List<Vector2Int> solution = new List<Vector2Int>();

        return playerPath.SequenceEqual(solution);
    }
}