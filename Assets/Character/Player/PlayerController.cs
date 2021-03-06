﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TopDownController))]
public class PlayerController : MonoBehaviour
{
    private TopDownController controller;
    public bool hasLeftWarp = false;

    public void SaveScenePositionForMemory()
    {
        lastHasLeftWarp = hasLeftWarp;
        lastPos = transform.position;
        sceneToWarpBackTo = SceneManager.GetActiveScene().name;
    }

    public string targetWarp = "";
    private Animator animController;
    private float normalSpeed;
    public float WhipStrafeSpeed = 2f;
    public float diveDistance = 2;
    public float diveTime = .5f;
    public float rollDistance = 2;
    public float rollTime = .5f;

    public string sceneToWarpBackTo = "";
    public Vector3 lastPos = Vector3.zero;
    public bool lastHasLeftWarp = false;

    public BoxCollider2D horizontalHitBoxLeft;
    public BoxCollider2D horizontalHitBoxRight;
    public BoxCollider2D hitBoxUp;
    public BoxCollider2D hitBoxDown;

    public bool CanWhip { get { return QuestSystem.Instance.CurrentState > QuestSystem.QuestState.Q5_RETRIEVE_WHIP; } }
    public bool CanDive { get { return QuestSystem.Instance.CurrentState > QuestSystem.QuestState.Q8_RESCUE_RALPH; } }

    void Start()
    {
        controller = GetComponent<TopDownController>();
        normalSpeed = controller.speed;
        animController = GetComponent<Animator>();
        SceneManager.sceneLoaded += LevelLoaded;
        DisableHitboxes();
        SpawnPlayerAtWarpPoint();
        damageableComponent = GetComponent<Damageable>();
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
    public Transform followTransform;

    private void LevelLoaded(Scene scene, LoadSceneMode arg1)
    {
        if (diveCoroutine != null)
            CancelDive();
        if (scene.name == sceneToWarpBackTo)
            SpawnPlayerBackToLastScene();
        else
            SpawnPlayerAtWarpPoint();

        FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = followTransform;
        float lastXMove = animController.GetFloat("lastXMove");
        float lastYMove = animController.GetFloat("lastYMove");
        GetComponent<Rigidbody2D>().velocity = new Vector2(lastXMove, lastYMove) * 10;
        animController.SetFloat("xMove", lastXMove);
        animController.SetFloat("yMove", lastYMove);
    }

    private void SpawnPlayerBackToLastScene()
    {
        hasLeftWarp = lastHasLeftWarp;
        transform.position = lastPos;
        sceneToWarpBackTo = "";
    }

    private void SpawnPlayerAtWarpPoint()
    {
        hasLeftWarp = false;
        List<WarpPoint> points = new List<WarpPoint>(FindObjectsOfType<WarpPoint>());
        WarpPoint result = GetPoint(points);
        if (result != null)
        {

            transform.position = new Vector3(result.transform.position.x, result.transform.position.y, 0);
            if (result.spawnDialogue != null)
                StartCoroutine(ShowDialogue(result.spawnDialogue));
        }
    }

    private IEnumerator ShowDialogue(Dialogue spawnDialogue)
    {
        while (FadeTransitionScreen.Instance.IsTransitioning)
            yield return null;
        DialogueManager.Instance.StartDialogue(spawnDialogue);
    }

    private WarpPoint GetPoint(List<WarpPoint> points)
    {
        WarpPoint defaultPoint = null;
        foreach (WarpPoint p in points)
        {
            if (p.gameObject.name == targetWarp)
            {
                return p;
            }
            if (p.isDefaultWarp)
                defaultPoint = p;
        }
        if (defaultPoint != null)
            return defaultPoint;
        return null;
    }

    private void CancelDive()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        StopCoroutine(diveCoroutine);
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Dive"))
        {
            animController.SetTrigger("Roll");
            animController.SetTrigger("RollDone");
        }
        else
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            animController.SetTrigger("RollDone");
        }
        controller.enabled = true;
        damageableComponent.enabled = true;
    }
    Coroutine diveCoroutine;

    private void Dead()
    {
        FadeTransitionScreen.Instance.Transition(() =>
        {
            damageableComponent.ResetHealth(6);
            SceneManager.LoadScene("Outside");
        });
    }

    void Update()
    {
        if (FadeTransitionScreen.Instance.IsTransitioning)
            return;
        if (Input.GetButtonDown("Pause"))
            PauseMenuManager.Instance.PressPause();
        if (PauseMenuManager.IsPaused)
            return;
        DisableHitboxes();
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("WhipHold"))
        {
            if (!Input.GetButton("Interact"))
            {
                animController.SetTrigger("WhipHoldDone");
                animController.SetFloat("lastXMove", animController.GetFloat("whipX"));
                animController.SetFloat("lastYMove", animController.GetFloat("whipY"));
                controller.yMove = Input.GetAxisRaw("Vertical");
            }
            else
            {
                controller.xMove = Input.GetAxisRaw("Horizontal");
                controller.yMove = Input.GetAxisRaw("Vertical");
            }
            return;
        }
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Whip"))
            return;
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Dive"))
            return;
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
            return;
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Stun"))
            return;
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
        {
            return;
        }

        controller.xMove = Input.GetAxisRaw("Horizontal");
        controller.yMove = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Interact"))
            TestInteractAndWhip();
        if (CanDive && Input.GetButtonDown("Dive"))
            diveCoroutine = StartCoroutine(StartDive());
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

    public Damageable damageableComponent;
    private IEnumerator StartDive()
    {
        gameObject.layer = LayerMask.NameToLayer("Rolling");
        controller.enabled = false;
        damageableComponent.enabled = false;
        animController.SetTrigger("Dive");
        yield return HandleRollOrDive(diveDistance, diveTime);
        animController.SetTrigger("Roll");
        yield return HandleRollOrDive(rollDistance, rollTime);
        animController.SetTrigger("RollDone");
        controller.enabled = true;
        damageableComponent.enabled = true;
        gameObject.StopTopDownController();
        gameObject.layer = LayerMask.NameToLayer("Default");
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
        SoundManager.Instance.PlaySound(SoundManager.Sound.SFX_LaserWhip);
        controller.speed = normalSpeed;
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
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        foreach (var dialogueComp in FindObjectsOfType<DialogueComponent>())
            if (dialogueComp.TryInteract())
                return;
        if (!CanWhip)
            return;
        controller.speed = WhipStrafeSpeed;
        animController.SetTrigger("Whip");
        float xMove = Input.GetAxisRaw("Horizontal");
        float yMove = Input.GetAxisRaw("Vertical");
        if (xMove == 0 && yMove == 0)
        {
            if (xMove != 0)
                GetComponent<SpriteRenderer>().flipX = xMove < 0;
            xMove = animController.GetFloat("lastXMove");
            yMove = animController.GetFloat("lastYMove");
        }
        animController.SetFloat("whipX", xMove);
        animController.SetFloat("whipY", yMove);
    }

    private void PlayDiveSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Sound.SFX_Player_DodgeRoll);
    }
}
