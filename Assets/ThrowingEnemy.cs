using System.Collections;
using UnityEngine;

public class ThrowingEnemy : MonoBehaviour
{
    Animator animator;

    Vector3 startPos;
    private PlayerController player;
    public float ThrowSpeed = 1;
    public float ThrowHeight = 1;

    private TopDownController controller;
    public void Throw()
    {
        var item = Instantiate(GlobalPrefabs.Instance.ThrownItemPrefab);
        item.transform.position = transform.position;

        item.GetComponent<ThrownItem>().Throw(gameObject, FindObjectOfType<PlayerController>().transform.position, ThrowSpeed, ThrowHeight);
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

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Throw"))    //Are we throwing?
            return;

        timeUntilThrow -= Time.deltaTime;
        if (timeUntilThrow > 0)
            JitterLogic();
        else
            ThrowLogic();
    }
    private const float MAX_TILE_DIST_FROM_START_SQR = 2 * 2;
    public float jitterRandom = .4f;
    private void JitterLogic()
    {
        controller.FacePosition(player.transform.position);
        Vector3 newTarget = startPos + new Vector3(Random.Range(-jitterRandom, jitterRandom), Random.Range(-jitterRandom, jitterRandom), 0);
        transform.position = Vector3.MoveTowards(transform.position, newTarget, .05f);
    }

    private void ThrowLogic()
    {
        animator.SetTrigger("Throw");
        RandomWaitForThrow();
    }
}
