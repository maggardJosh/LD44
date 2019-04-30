using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Cemetery Scene
//Player leaves cemetery
//Dialogue "I barely remember that"
public class M0 : MonoBehaviour
{
    public Dialogue cutsceneDialogue;

    private void Start()
    {
        StartCoroutine(CutsceneLogic());
    }
  
    private IEnumerator CutsceneLogic()
    {
        TopDownController p = FindObjectOfType<PlayerController>().GetComponent<TopDownController>();
        p.FaceDirection(Vector3.right);
        while (FadeTransitionScreen.Instance.IsTransitioning)
            yield return null;
        FadeTransitionScreen.Instance.SetCinematic(true);
        p.FaceDirection(Vector3.right);
        yield return new WaitForSeconds(1f);
        
        
        yield return MoveToPosition(p, "PositionOne", 1.5f);
        yield return MoveToPosition(p, "PositionOne (1)", 1.5f);
        yield return new WaitForSeconds(1f);
        p.FaceDirection(Vector3.left);
        yield return new WaitForSeconds(1f);

        yield return MoveToPosition(p, "PositionTwo", 1f);
        yield return new WaitForSeconds(1.5f);
        p.FaceDirection(Vector3.left);
        yield return new WaitForSeconds(2f);
        yield return MoveToPosition(p, "PositionThree", .3f);
        yield return MoveToPosition(p, "PositionThree (1)", .2f);
        yield return MoveToPosition(p, "PositionThree (2)", .2f);
        yield return MoveToPosition(p, "PositionThree (3)", .2f);
        yield return MoveToPosition(p, "PositionThree (4)", .3f);

        yield return new WaitForSeconds(1f);
        yield return DialogueManager.Instance.StartDialogueThreaded(cutsceneDialogue);

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
    private IEnumerator MoveToPosition(TopDownController t, Vector3 pos, float time)
    {
        Vector3 diff = pos - t.transform.position;
        diff.Normalize();
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
        t.gameObject.StopTopDownController();
        t.transform.position = pos;
    }
}
