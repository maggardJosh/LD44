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

    public PolygonCollider2D horizontalHitBoxLeft;
    public PolygonCollider2D horizontalHitBoxRight;

    void Start()
    {
        controller = GetComponent<TopDownController>();
        animController = GetComponent<Animator>();
        SceneManager.sceneLoaded += LevelLoaded;
        DisableHitboxes();
    }

    private void DisableHitboxes()
    {
        horizontalHitBoxLeft.enabled = false;
        horizontalHitBoxRight.enabled = false;
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
        DisableHitboxes();
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("Whip"))
            return;
        controller.xMove = Input.GetAxisRaw("Horizontal");
        controller.yMove = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Interact"))
            TestInteractAndWhip();
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
