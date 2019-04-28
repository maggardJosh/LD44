﻿using System.Collections;
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
    public GameObject parent;

    public GameObject actualItem;
   
    public float DamageDistanceSquared = 1 * 1;
    
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

    private void OnDestroy()
    {
        foreach (var d in FindObjectsOfType<Damageable>())
            if (d.gameObject != parent && (d.transform.position - transform.position).sqrMagnitude <= DamageDistanceSquared)
            {
                d.TakeDamage(1);
                d.PushBack(transform.position, 2f, 1f);

            }
    }

    public void Throw(GameObject parent, Vector3 targetPos, float time, float height)
    {
        this.parent = parent;
        startPos = transform.position;
        endPos = targetPos;
        throwTime = time;
        maxHeight = height;
    }
}