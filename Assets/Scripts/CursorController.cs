// using UnityEngine;
// using UnityEngine.UI;

// public class CursorController : MonoBehaviour
// {
//     public CursorConstraint cursorConstraint;
//     private RectTransform rectTransform;
//     private Canvas canvas;

//     private void Start()
//     {
//         rectTransform = GetComponent<RectTransform>();
//         canvas = GetComponentInParent<Canvas>();
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

//         // Get the mouse position in canvas space
//         Vector2 mousePosition = Input.mousePosition;
//         RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out mousePosition);

//         // Constrain the cursor position and set it
//         Vector2 newPosition = cursorConstraint.GetConstrainedPosition(rectTransform.anchoredPosition, mousePosition);
//         rectTransform.anchoredPosition = newPosition;
//     }
// }
// using UnityEngine;
// using UnityEngine.UI;

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

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class CursorController : MonoBehaviour
{
    public List<RectTransform> gridLines;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Cursor.visible = true;
    }

    private void Update()
    {
        if (!this.enabled)
        {
            Cursor.visible = true;
            return;
        }

        Cursor.visible = false;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 newPosition = rectTransform.anchoredPosition;
        Vector2 cursorCenter = newPosition + new Vector2(rectTransform.rect.width / 2f, -rectTransform.rect.height / 2f);

        bool moved = false;

        foreach (RectTransform gridLine in gridLines)
        {
            Rect gridLineRect = GetWorldRect(gridLine);

            if (gridLineRect.Contains(cursorCenter))
            {
                Debug.Log(horizontal + " ||| " + gridLineRect.width);
                if (horizontal != 0 && gridLineRect.width > gridLineRect.height)
                {
                    Debug.Log("bloop5");
                    newPosition.x += horizontal * gridLineRect.width;
                    moved = true;
                }
                else if (vertical != 0 && gridLineRect.height > gridLineRect.width)
                {
                    Debug.Log("bloop6");
                    newPosition.y += vertical * gridLineRect.height;
                    moved = true;
                }

                if (horizontal != 0 && vertical != 0 && !moved)
                {
                    Debug.Log("bloop3");
                    float horizontalDistance = Mathf.Min(Mathf.Abs(cursorCenter.x - gridLineRect.xMin), Mathf.Abs(cursorCenter.x - gridLineRect.xMax));
                    float verticalDistance = Mathf.Min(Mathf.Abs(cursorCenter.y - gridLineRect.yMin), Mathf.Abs(cursorCenter.y - gridLineRect.yMax));

                    if (horizontalDistance < verticalDistance && gridLineRect.width > gridLineRect.height)
                    {
                        Debug.Log("bloop1");
                        newPosition.x += horizontal * gridLineRect.width;
                    }
                    else if (verticalDistance < horizontalDistance && gridLineRect.height > gridLineRect.width)
                    {
                        Debug.Log("bloop2");
                        newPosition.y += vertical * gridLineRect.height;
                    }
                }

                break;
            }
        }
        //Debug.Log(newPosition);
        rectTransform.anchoredPosition = newPosition;
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