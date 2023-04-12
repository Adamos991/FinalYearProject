using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleCount : MonoBehaviour
{
    TMPro.TMP_Text text;
    int count = 0;

    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponent<TMPro.TMP_Text>();
    }
    void Start() {
        UpdateCount();
        //Collectible.OnCollected += OnCollectibleCollected;
    }
    void OnEnable() => Collectible.OnCollected += OnCollectibleCollected;
    void OnDisable() => Collectible.OnCollected -= OnCollectibleCollected;

    void OnCollectibleCollected() {
        count++;
        UpdateCount();
    }
    // Update is called once per frame
    void UpdateCount()
    {
        text.text = $"Cartons Collected {count} / {Collectible.total}";
    }
}
