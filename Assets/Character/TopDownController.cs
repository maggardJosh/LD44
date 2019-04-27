using UnityEngine;

public class TopDownController : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Animator animator;

    public float xMove;
    public float yMove;

    public float speed = 2;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if (Mathf.Abs(xMove) > 0 || Mathf.Abs(yMove) > 0)
        {
            animator.SetFloat("lastXMove", xMove);
            animator.SetFloat("lastYMove", yMove);
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
