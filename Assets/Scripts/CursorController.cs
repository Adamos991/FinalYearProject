using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CursorController : MonoBehaviour
{
    public List<RectTransform> gridLines;
    private RectTransform rectTransform;
    private Vector2 previousMovementDirection;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = true;
        previousMovementDirection = Vector2.zero;
    }

    private void Update()
    {
        if (!this.enabled)
        {
            Cursor.visible = true;
            return;
        }

        Cursor.visible = false;

        Vector2 newPosition = rectTransform.anchoredPosition;
        Vector2 cursorCenter = newPosition;
        bool moved = false;
        Vector2 movementDirection = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            movementDirection = Vector2.left;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            movementDirection = Vector2.right;
        }
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            movementDirection = Vector2.up;
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            movementDirection = Vector2.down;
        }

        if (movementDirection != Vector2.zero)
        {
            previousMovementDirection = movementDirection;
        }

        foreach (RectTransform gridLine in gridLines)
        {
            Rect gridLineRect = GetWorldRect(gridLine);

            if (gridLineRect.Contains(cursorCenter))
            {
                if (movementDirection == Vector2.left || movementDirection == Vector2.right)
                {
                    if(gridLineRect.width > gridLineRect.height) {
                        newPosition += movementDirection * 1f;
                        if(gridLineRect.Contains(newPosition)) {
                            moved = true;
                        }
                    } else {
                        for(int i = 6; i < 12; i++) {
                            Debug.Log(newPosition.y + ", " + gridLines[i].anchoredPosition.y);
                            if(Mathf.Abs(newPosition.y - gridLines[i].anchoredPosition.y) <= 10) {
                                Vector2 targetPosition = new Vector2(newPosition.x, gridLines[i].anchoredPosition.y);
                                Vector2 direction = targetPosition - newPosition;
                                direction = direction.normalized;
                                newPosition += direction * Time.deltaTime;
                                i--;
                            }
                        }
                        continue;
                    }
                }
                else if (movementDirection == Vector2.up || movementDirection == Vector2.down)
                {
                    if(gridLineRect.height > gridLineRect.width) {
                        newPosition += movementDirection * 1f;
                        if(gridLineRect.Contains(newPosition)) {
                            moved = true;
                        }
                    } else {
                        for(int i = 0; i < 6; i++) {
                            if(Mathf.Abs(newPosition.x - gridLines[i].anchoredPosition.x) <= 10) {
                                Vector2 targetPosition = new Vector2(gridLines[i].anchoredPosition.x, newPosition.y);
                                Vector2 direction = targetPosition - newPosition;
                                direction = direction.normalized;
                                newPosition += direction * Time.deltaTime;
                                i--;
                            }
                        }
                        continue;
                    }
                }
                if(moved) {
                    rectTransform.anchoredPosition = newPosition;
                    break;
                }
            }
        }
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Vector2 min = new Vector2(corners[0].x, corners[0].y);
        Vector2 max = new Vector2(corners[2].x, corners[2].y);
        return new Rect(min, max - min);
    }
}

// public class CursorController : MonoBehaviour
// {
//     private RectTransform rectTransform;

//     private void Start()
//     {
//         rectTransform = GetComponent<RectTransform>();
//         Cursor.visible = true;
//     }

//     private void Update()
//     {
//         if (!this.enabled)
//         {
//             Cursor.visible = true;
//             return;
//         }

//         Cursor.visible = false;
//         Vector2 mousePosition = Input.mousePosition;
//         rectTransform.anchoredPosition = mousePosition;
//     }
// }