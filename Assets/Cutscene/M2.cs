using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Roll Scene
//Uses whip on spiders
public class M2 : MonoBehaviour
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
        yield return new WaitForSeconds(1.5f);
        p.GetComponent<Animator>().SetTrigger("Dive");
        yield return MoveToPosition(p, "Position", .7f);
        p.GetComponent<Animator>().SetTrigger("Roll");
        yield return MoveToPosition(p, "Position2", .3f);
        p.GetComponent<Animator>().SetTrigger("RollDone");
        yield return new WaitForSeconds(1f);
        yield return MoveToPosition(p, "Position3", .5f);
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
