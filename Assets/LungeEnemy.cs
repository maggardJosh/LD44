using System.Collections;
using UnityEngine;

public class LungeEnemy : MonoBehaviour
{
    Animator animator;

    Vector3 startPos;
    Damageable damageableComponent;
    private PlayerController player;
    public float LungeSpeed = 5;
    public float ChaseSpeed = 1;
    public float LungeDist = 2;
    public float AggroDist = 7;
    public float LungeTime = 2;

    private enum State
    {
        IDLE,
        AGGRO_OUT_OF_DISTANCE,
        AGGRO_IN_DISTANCE,
        LUNGING
    }
    private State currentState = State.IDLE;

    private TopDownController controller;
    public void Throw()
    {

    }
    void Start()
    {
        controller = GetComponent<TopDownController>();
        animator = GetComponent<Animator>();
        startPos = transform.position;
        RandomWaitForLunge();
        player = FindObjectOfType<PlayerController>();
        damageableComponent = GetComponent<Damageable>();
    }

    private void RandomWaitForLunge()
    {
        timeUntilThrow = Random.Range(minLungeTime, maxLungeTime);
    }

    public float minLungeTime = 4;
    public float maxLungeTime = 9;

    private float timeUntilThrow;

    void Update()
    {

        if (controller.StunTimeLeft > 0)
        {
            animator.SetTrigger("LungeDone");
            StopCoroutine(lungeCo);
            controller.enabled = true;
            damageableComponent.enabled = true;
            currentState = State.IDLE;
            return;
        }
        Vector3 playerDiff;
        switch (currentState)
        {
            case State.IDLE:
                playerDiff = (player.transform.position - transform.position);
                if (playerDiff.magnitude < AggroDist)
                {
                    currentState = State.AGGRO_OUT_OF_DISTANCE;
                    break;
                }
                break;
            case State.AGGRO_OUT_OF_DISTANCE:

                playerDiff = (player.transform.position - transform.position);
                if (playerDiff.magnitude < LungeDist * .9f)
                {
                    currentState = State.AGGRO_IN_DISTANCE;
                    gameObject.StopTopDownController();
                    startPos = transform.position;
                    RandomWaitForLunge();
                    break;
                }
                if (playerDiff.magnitude > AggroDist)
                {
                    gameObject.StopTopDownController();
                    currentState = State.IDLE;
                    break;
                }
                Vector3 move = playerDiff.normalized;
                controller.xMove = move.x;
                controller.yMove = move.y;
                break;
            case State.AGGRO_IN_DISTANCE:
                timeUntilThrow -= Time.deltaTime;

                playerDiff = (player.transform.position - transform.position);
                if (playerDiff.magnitude > LungeDist)
                {
                    currentState = State.AGGRO_OUT_OF_DISTANCE;
                    break;
                }
                if (timeUntilThrow > 0)
                    JitterLogic();
                else
                    LungeLogic();
                break;
            case State.LUNGING:
               
                break;
        }

    }
    private Vector2 lungeVelocity;
    private const float MAX_TILE_DIST_FROM_START_SQR = 2 * 2;
    public float jitterRandom = .4f;
    private void JitterLogic()
    {
        controller.FacePosition(player.transform.position);
        Vector3 newTarget = startPos + new Vector3(Random.Range(-jitterRandom, jitterRandom), Random.Range(-jitterRandom, jitterRandom), 0);
        transform.position = Vector3.MoveTowards(transform.position, newTarget, 1f * Time.deltaTime);
    }

    Coroutine lungeCo;
    private void Lunge()
    {
        lungeCo = StartCoroutine(LungeAtPlayer());
    }

    public AnimationCurve velLungeCurve;
    public IEnumerator LungeAtPlayer()
    {
        Vector2 playerDiff = player.transform.position - transform.position;
        lungeVelocity = playerDiff.normalized * LungeSpeed;
        controller.xMove = lungeVelocity.x;
        controller.yMove = lungeVelocity.y;
        controller.UpdateAnimationOnly();

        controller.enabled = false;
        float count = 0;
        while(count < LungeTime)
        {
            Debug.Log(count);
            Debug.Log(lungeVelocity);
            count += Time.deltaTime;
            GetComponent<Rigidbody2D>().velocity = lungeVelocity * velLungeCurve.Evaluate(count/LungeTime);
            lungeVelocity *= .9f;
            yield return null;
        }

        damageableComponent.enabled = true;
        controller.enabled = true;
        currentState = State.AGGRO_IN_DISTANCE;
        animator.SetTrigger("LungeDone");

        RandomWaitForLunge();
    }
    private void LungeLogic()
    {
        animator.SetTrigger("Lunge");
        damageableComponent.enabled = false;
        currentState = State.LUNGING;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var damageComp = collision.gameObject.GetComponent<Damageable>();
            if (damageComp != null)
            {
                damageComp.TakeDamage(1, transform.position, 2);
            }
        }
    }
}
