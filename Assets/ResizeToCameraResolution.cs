using UnityEngine;

public class ResizeToCameraResolution : MonoBehaviour
{
    BoxCollider2D b;
    // Start is called before the first frame update
    void Start()
    {
        b = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var cam = FindObjectOfType<Camera>();
        b.size = new Vector2(20, 12);
    }
}
