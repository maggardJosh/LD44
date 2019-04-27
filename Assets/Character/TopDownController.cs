using System;
using UnityEngine;

public class TopDownController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer sRend;

    public float xMove;
    public float yMove;

    public float speed = 2;

    public float StunTimeLeft = 0;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sRend = GetComponent<SpriteRenderer>();
        GameObject shadow = GameObject.Instantiate(GlobalPrefabs.Instance.ShadowPrefab, transform);
        shadow.transform.localPosition = Vector3.zero;
        animator.SetFloat("xMove", 0);
        animator.SetFloat("yMove", -1);
    }

    public void PushBack(Vector3 position, float pushBackValue, float stunTime)
    {
        Vector3 diffNormalized = (transform.position - position).normalized;
        rigidBody.velocity = diffNormalized  * pushBackValue;
        animator.SetFloat("lastXMove", -diffNormalized.x);
        animator.SetFloat("lastYMove", -diffNormalized.y);
        sRend.flipX = -diffNormalized.x < 0;

        animator.SetFloat("xMove", 0);
        animator.SetFloat("yMove", 0);
        this.StunTimeLeft = stunTime;
    }

    public void FacePosition(Vector3 position)
    {
        Vector3 diffNormalized = (transform.position - position).normalized;
        animator.SetFloat("lastXMove", diffNormalized.x);
        animator.SetFloat("lastYMove", -diffNormalized.y);
        sRend.flipX = -diffNormalized.x < 0;
    }

    void LateUpdate()
    {
        if(StunTimeLeft > 0)
        {
            StunTimeLeft -= Time.deltaTime;
            StunLogic();
            return;
        }

        if (Mathf.Abs(xMove) > 0 || Mathf.Abs(yMove) > 0)
        {
            animator.SetFloat("lastXMove", xMove);
            animator.SetFloat("lastYMove", yMove);

            if (Mathf.Abs(xMove) > 0)
                sRend.flipX = xMove < 0;
            rigidBody.velocity = (new Vector2(xMove, yMove) * speed);
        }
        else
        {
            rigidBody.velocity = Vector2.zero;
        }
        animator.SetFloat("xMove", xMove);
        animator.SetFloat("yMove", yMove);
        xMove = 0;
        yMove = 0;
    }

    private void StunLogic()
    {
        //TODO: get rid of magic number
        rigidBody.velocity *= .9f;
    }
}
