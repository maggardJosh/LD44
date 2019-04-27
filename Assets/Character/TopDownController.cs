using UnityEngine;

public class TopDownController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;
    private SpriteRenderer sRend;

    public float xMove;
    public float yMove;

    public float speed = 2;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sRend = GetComponent<SpriteRenderer>();
        GameObject shadow = GameObject.Instantiate(GlobalPrefabs.Instance.ShadowPrefab, transform);
        shadow.transform.localPosition = Vector3.zero;

    }

    void LateUpdate()
    {
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
}
