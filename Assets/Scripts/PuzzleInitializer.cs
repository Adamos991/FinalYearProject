using UnityEngine;
using UnityEngine.UI;

public class PuzzleInitializer : MonoBehaviour
{
    public RectTransform cursor;
    public CursorController cursorController;
    [SerializeField] private GameObject cursorImage;
    public void InitializePuzzle()
    {
        //RectTransform buttonRectTransform = GetComponent<RectTransform>();
        //cursorImage.GetComponent<RectTransform>().anchoredPosition = buttonRectTransform.anchoredPosition;
        cursorImage.SetActive(true);
        
        //cursorImage.SetActive(true);
        //cursor.anchoredPosition = buttonRectTransform.anchoredPosition;

        // Enable the CursorController script
        cursorController.enabled = true;

        // Add more puzzle initialization logic here later
    }

}