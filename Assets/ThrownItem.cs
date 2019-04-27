using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownItem : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;
    public AnimationCurve heightCurve;
    private float count = 0;
    public float throwTime = 2;
    public float maxHeight = 2;

    public GameObject actualItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        count += Time.deltaTime;
        float t = count / throwTime;
        if(t>= 1)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.Lerp(startPos, endPos, t);
        Vector3 localPos = actualItem.transform.localPosition;
        localPos.y = heightCurve.Evaluate(t) * maxHeight;
        actualItem.transform.localPosition = localPos;
    }

    public void Throw(Vector3 targetPos, float time, float height)
    {
        startPos = transform.position;
        endPos = targetPos;
        throwTime = time;
        maxHeight = height;
    }
}
