using UnityEngine;

public class activatePuzzle : MonoBehaviour
{
    public GameObject puzzleCanvas;
    public GameObject playerCamRig;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            puzzleCanvas.gameObject.SetActive(true);
            gameObject.SetActive(false);
            playerCamRig.SetActive(false);
        }
    }
}
