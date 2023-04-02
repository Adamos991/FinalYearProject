using UnityEngine;
using System.Collections.Generic;

public class CursorConstraint : MonoBehaviour
{
    public List<RectTransform> gridLines;

    public Vector2 GetConstrainedPosition(Vector2 currentPosition, Vector2 targetPosition)
    {
        Vector2 newPosition = targetPosition;

        foreach (RectTransform gridLine in gridLines)
        {
            Rect gridLineRect = GetWorldRect(gridLine);

            if (gridLineRect.Contains(targetPosition))
            {
                newPosition = currentPosition;
                break;
            }
        }

        float nearestDistance = float.MaxValue;
        Vector2 nearestGridPosition = Vector2.zero;
        foreach (RectTransform gridLine in gridLines)
        {
            Rect gridLineRect = GetWorldRect(gridLine);

            if (direction.x != 0 && gridLineRect.width > gridLineRect.height)
            {
                float distance = Mathf.Abs(gridLineRect.center.y - newPosition.y);

                if (distance < nearestDistance && Mathf.Abs(gridLineRect.center.x - newPosition.x) < gridLineRect.width / 2)
                {
                    nearestDistance = distance;
                    nearestGridPosition = new Vector2(currentPosition.x, gridLineRect.center.y);
                }
            }
            else if (direction.y != 0 && gridLineRect.height > gridLineRect.width)
            {
                float distance = Mathf.Abs(gridLineRect.center.x - newPosition.x);

                if (distance < nearestDistance && Mathf.Abs(gridLineRect.center.y - newPosition.y) < gridLineRect.height / 2)
                {
                    nearestDistance = distance;
                    nearestGridPosition = new Vector2(gridLineRect.center.x, currentPosition.y);
                }
            }
        }

        return nearestGridPosition;
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector2 min = new Vector2(corners[0].x, corners[0].y);
        Vector2 max = new Vector2(corners[2].x, corners[2].y);
        return new Rect(min, max - min);
    }

    private Vector2 direction;
    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }
}