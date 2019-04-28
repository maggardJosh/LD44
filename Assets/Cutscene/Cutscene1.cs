using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene1 : MonoBehaviour
{
    public ParticleSystem partSyst;
    public Dialogue cutsceneDialogue;

    private void Start()
    {
        FindObjectOfType<Camera>().backgroundColor = new Color(0, 0, .2f);
        
    }
    public void StartCutscene()
    {
        FadeTransitionScreen.Instance.SetCinematic(true);
        StartCoroutine(CutsceneLogic());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            StopTopDownController(collision.gameObject);
            StartCutscene();
        }
    }

    private static void StopTopDownController(GameObject collision)
    {
        var playerController = collision.GetComponent<TopDownController>();
        playerController.xMove = 0;
        playerController.yMove = 0;
        playerController.UpdateAnimationOnly();
    }

    private IEnumerator CutsceneLogic()
    {
        yield return new WaitForSeconds(3f);

        GameObject fargoth = GameObject.Find("Fargoth");

        TopDownController p = FindObjectOfType<PlayerController>().GetComponent<TopDownController>();
        
        yield return MoveToPosition(p, GameObject.Find("CinemaStartPos").transform.position, 1.5f);
        p.FacePosition(fargoth.transform.position);
        yield return new WaitForSeconds(1f);

        yield return MoveToPosition(fargoth.GetComponent<TopDownController>(), p.transform.position + Vector3.left * 4f, 1f);
        yield return new WaitForSeconds(.5f);

        yield return MoveToPosition(fargoth.GetComponent<TopDownController>(), p.transform.position + Vector3.left * 2f, 3f);

        yield return new WaitForSeconds(1f);
        yield return DialogueManager.Instance.StartDialogueThreaded(cutsceneDialogue);

        yield return MoveToPosition(fargoth.GetComponent<TopDownController>(), p.transform.position + Vector3.left * 2f + Vector3.up * 20f, 1f);
        if (partSyst != null)
            partSyst.Stop();
        yield return new WaitForSeconds(3f);

        FadeTransitionScreen.Instance.SetCinematic(false);
    }

    private IEnumerator MoveToPosition(TopDownController t, Vector3 pos, float time)
    {
        Vector3 diff = pos - t.transform.position;
        Vector3 startPos = t.transform.position;
        float count = 0;
        while(count <= time)
        {
            t.xMove = diff.x;
            t.yMove = diff.y;
            t.UpdateAnimationOnly();
            t.transform.position = Vector3.Lerp(startPos, pos, count / time);
            count += Time.deltaTime;
            yield return null;
        }
        StopTopDownController(t.gameObject);
        t.transform.position = pos;
    }
}
