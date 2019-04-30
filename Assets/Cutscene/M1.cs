using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Whip Scene
//Uses whip on spiders
public class M1 : MonoBehaviour
{
    public Dialogue cutsceneDialogue;

    private void Start()
    {
        StartCoroutine(CutsceneLogic());
    }

    private IEnumerator CutsceneLogic()
    {
        TopDownController p = FindObjectOfType<PlayerController>().GetComponent<TopDownController>();
        p.FaceDirection(Vector3.down);
        while (FadeTransitionScreen.Instance.IsTransitioning)
            yield return null;
        FadeTransitionScreen.Instance.SetCinematic(true);
        p.FaceDirection(Vector3.down);
        p.GetComponent<Animator>().SetFloat("whipY", -1);
        p.GetComponent<Animator>().SetTrigger("Whip");
        p.GetComponent<Animator>().SetTrigger("WhipHoldDone");
        yield return new WaitForSeconds(1f);
        StartCoroutine(MoveToPosition(GameObject.Find("ThrowEnemy").GetComponent<TopDownController>(), Vector3.down, .3f, true));
        p.FaceDirection(Vector3.left);
        p.GetComponent<Animator>().SetFloat("whipY", 0);
        p.GetComponent<Animator>().SetFloat("whipX", -1);
        p.GetComponent<Animator>().SetTrigger("Whip");
        p.GetComponent<Animator>().SetTrigger("WhipHoldDone");
        StartCoroutine(MoveToPosition(GameObject.Find("ThrowEnemy (1)").GetComponent<TopDownController>(), Vector3.left, .3f, true));
        yield return new WaitForSeconds(1.5f);
        p.FaceDirection(Vector3.right);
        p.GetComponent<Animator>().SetFloat("whipX", -1);
        p.GetComponent<Animator>().SetTrigger("Whip");
        p.GetComponent<Animator>().SetTrigger("WhipHoldDone");
        StartCoroutine(MoveToPosition(GameObject.Find("ThrowEnemy (2)").GetComponent<TopDownController>(), Vector3.right, .3f, true));
        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(1f);

        SoundManager.Instance.PlaySound(SoundManager.Sound.Music_Transition2);
        FadeTransitionScreen.Instance.Transition(() =>
        {
            SceneManager.LoadScene(p.GetComponent<PlayerController>().sceneToWarpBackTo);
        });
    }

    private IEnumerator MoveToPosition(TopDownController t, string posName, float time)
    {
        yield return MoveToPosition(t, GameObject.Find(posName).transform.position, time);
    }
    private IEnumerator MoveToPosition(TopDownController t, Vector3 pos, float time, bool relative = false)
    {
        if (relative)
            pos += t.transform.position;
        Vector3 diff = pos - t.transform.position;
        diff.Normalize();
        Vector3 startPos = t.transform.position;
        float count = 0;
        while (count <= time)
        {
            t.xMove = diff.x;
            t.yMove = diff.y;
            t.UpdateAnimationOnly();
            t.transform.position = Vector3.Lerp(startPos, pos, count / time);
            count += Time.deltaTime;
            yield return null;
        }
        t.gameObject.StopTopDownController();
        t.transform.position = pos;
    }
}
