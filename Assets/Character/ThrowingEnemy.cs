using System.Collections.Generic;
using UnityEngine;

public class ThrowingEnemy : MonoBehaviour
{
    Animator animator;

    Vector3 startPos;
    private PlayerController player;
    public float ThrowSpeed = 1;
    public float ThrowHeight = 1;
    public float ThrowDist = 5;
    public float AggroDist = 7;
    public float ReEnterDist = 4;
    public float ThrowRandom = .3f;
    public Transform ThrowLaunchPoint;

    private enum State
    {
        IDLE,
        AGGRO_OUT_OF_DISTANCE,
        AGGRO_IN_DISTANCE,
        SHOOTING
    }
    private State currentState = State.IDLE;

    private TopDownController controller;
    public void Throw()
    {
        //TODO: Decide if this is where the throwing sound should be played
        SoundManager.Instance.PlaySound(SoundManager.Sound.SFX_Enemy_EnergyMortar);
        var item = Instantiate(GlobalPrefabs.Instance.ThrownItemPrefab);
        item.transform.position = ThrowLaunchPoint.position;
        var player = FindObjectOfType<PlayerController>();
        Vector3 randomDisp = Random.insideUnitCircle * ThrowRandom;
        Vector3 target = player.transform.position + new Vector3(player.GetComponent<Rigidbody2D>().velocity.x, player.GetComponent<Rigidbody2D>().velocity.y) * ThrowSpeed + randomDisp;

        item.GetComponent<ThrownItem>().Throw(gameObject, target, ThrowSpeed, ThrowHeight);
    }

    void Start()
    {
        controller = GetComponent<TopDownController>();
        animator = GetComponent<Animator>();
        startPos = transform.position;
        RandomWaitForThrow();
        player = FindObjectOfType<PlayerController>();
    }

    private void RandomWaitForThrow()
    {
        timeUntilThrow = Random.Range(minThrowTime, maxThrowTime);
    }

    public float minThrowTime = 4;
    public float maxThrowTime = 9;

    private float timeUntilThrow;

    private void Dead()
    {
        Instantiate(GlobalPrefabs.Instance.ExplosionPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    void Update()
    {

        if (controller.StunTimeLeft > 0)
        {
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
                if (playerDiff.magnitude < ReEnterDist)
                {
                    currentState = State.AGGRO_IN_DISTANCE;
                    gameObject.StopTopDownController();
                    startPos = transform.position;
                    RandomWaitForThrow();
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
                if (playerDiff.magnitude > ThrowDist)
                {
                    currentState = State.AGGRO_OUT_OF_DISTANCE;
                    break;
                }
                if (timeUntilThrow > 0)
                    JitterLogic();
                else
                    ThrowLogic();
                break;
            case State.SHOOTING:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Throw"))    //Are we throwing?
                    return;

                currentState = State.AGGRO_IN_DISTANCE;
                break;
        }

    }
    private const float MAX_TILE_DIST_FROM_START_SQR = 2 * 2;
    public float jitterRandom = .4f;
    private void JitterLogic()
    {
        controller.FacePosition(player.transform.position);
        Vector3 newTarget = startPos + new Vector3(Random.Range(-jitterRandom, jitterRandom), Random.Range(-jitterRandom, jitterRandom), 0);
        transform.position = Vector3.MoveTowards(transform.position, newTarget, 1f * Time.deltaTime);
    }

    private void ThrowLogic()
    {
        animator.SetTrigger("Throw");
        RandomWaitForThrow();
        currentState = State.SHOOTING;
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

    List<SoundManager.Sound> StepCollection = new List<SoundManager.Sound> { SoundManager.Sound.SFX_Enemy_EnergyWalk1, SoundManager.Sound.SFX_Enemy_EnergyWalk2 };
    private void PlayStepSound()
    {
        SoundManager.Instance.PlaySound(StepCollection[UnityEngine.Random.Range(0, StepCollection.Count)]);
    }
}
