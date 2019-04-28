using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TopDownController))]
public class PlayerController : MonoBehaviour
{
    private TopDownController controller;
    public bool hasLeftWarp = false;
    public string targetWarp = "";
    private Animator animController;
    private float normalSpeed;
    public float diveDistance = 2;
    public float diveTime = .5f;
    public float rollDistance = 2;
    public float rollTime = .5f;

    public BoxCollider2D horizontalHitBoxLeft;
    public BoxCollider2D horizontalHitBoxRight;
    public BoxCollider2D hitBoxUp;
    public BoxCollider2D hitBoxDown;

    void Start()
    {
        controller = GetComponent<TopDownController>();
        normalSpeed = controller.speed;
        animController = GetComponent<Animator>();
        SceneManager.sceneLoaded += LevelLoaded;
        DisableHitboxes();
    }

    private void DisableHitboxes()
    {
        horizontalHitBoxLeft.enabled = false;
        horizontalHitBoxRight.enabled = false;
        hitBoxUp.enabled = false;
        hitBoxDown.enabled = false;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= LevelLoaded;
    }

    private void LevelLoaded(Scene arg0, LoadSceneMode arg1)
    {
        List<WarpPoint> points = new List<WarpPoint>(FindObjectsOfType<WarpPoint>());
        foreach (WarpPoint p in points)
        {
            if (p.gameObject.name == targetWarp)
            {
                transform.position = p.transform.position;
                break;
            }
        }
        FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = transform;
    }

    void Update()
    {
        if (FadeTransitionScreen.Instance.IsTransitioning)
            return;
        DisableHitboxes();
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Whip"))
            return;
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Dive"))
            return;
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
            return;

        controller.xMove = Input.GetAxisRaw("Horizontal");
        controller.yMove = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Interact"))
            TestInteractAndWhip();
        if (Input.GetButtonDown("Dive"))
            StartCoroutine(StartDive());
        if (Input.GetButtonDown("Pause"))
            PauseMenuManager.Instance.PressPause();
        if (Input.GetButtonDown("Mute"))
            SoundManager.ToggleMute();
    }

    public void EnterRoll()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");
        if (xMove != 0 || yMove != 0)
        {
            controller.xMove = xMove;
            controller.yMove = yMove;
        }
    }

    private IEnumerator StartDive()
    {
        controller.enabled = false;
        animController.SetTrigger("Dive");
        yield return HandleRollOrDive(diveDistance, diveTime);
        animController.SetTrigger("Roll");
        yield return HandleRollOrDive(rollDistance, rollTime);
        animController.SetTrigger("RollDone");
        controller.enabled = true;
        gameObject.StopTopDownController();
    }

    private IEnumerator HandleRollOrDive(float dist, float time)
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");
        if (xMove == 0 && yMove == 0)
        {
            xMove = animController.GetFloat("lastXMove");
            yMove = animController.GetFloat("lastYMove");
        }
        else
        {
            animController.SetFloat("lastXMove", xMove);
            animController.SetFloat("lastYMove", yMove);
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + new Vector3(xMove, yMove).normalized * dist;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float count = 0;
        while (count < time)
        {
            //Todo: don't use physics to do this, it makes collision pretty broken
            count += Time.deltaTime;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, count / time);
            rb.MovePosition(newPos);
            yield return null;
        }
    }

    public void WhipHit(string hitboxToEnable)
    {
        switch (hitboxToEnable)
        {
            case "WhipHorizontalHit":
                if (GetComponent<SpriteRenderer>().flipX)
                    horizontalHitBoxLeft.enabled = true;
                else
                    horizontalHitBoxRight.enabled = true;
                break;
            case "WhipUp":
                hitBoxUp.enabled = true;
                break;
            case "WhipDown":
                hitBoxDown.enabled = true;
                break;
        }
    }

    private void TestInteractAndWhip()
    {
        foreach (var dialogueComp in FindObjectsOfType<DialogueComponent>())
            if (dialogueComp.TryInteract())
                return;
        animController.SetTrigger("Whip");
    }
}
