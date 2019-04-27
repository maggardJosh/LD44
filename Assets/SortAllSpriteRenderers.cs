using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortAllSpriteRenderers : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        foreach (var c in FindObjectsOfType<SpriteRenderer>())
            c.sortingOrder = Mathf.CeilToInt(-c.transform.position.y * 100) - 300;
    }
}
