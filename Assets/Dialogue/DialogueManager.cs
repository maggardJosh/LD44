using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> dialogueQueue;
    public Dialogue currentDialogue;
    public Text DialogueLines;
    public Text CharacterName;
    public Animator animator;

    private static DialogueManager _instance;
    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<DialogueManager>();
            return _instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        dialogueQueue = new Queue<string>();
        Instance.gameObject.SetActive(true);
    }

    private Dialogue.DialogueSet GetCurrentDialogue(Dialogue dialogue)
    {
        foreach (var set in dialogue.dialogueEntries.OrderBy(s => (int)s.MinQuestLevel))
            if (set.MinQuestLevel >= QuestSystem.Instance.CurrentState)
                return set;

        return dialogue.dialogueEntries.LastOrDefault();
    }

    public void StartDialogue(Dialogue dialogue, bool setCinematic = true)
    {
        if (FadeTransitionScreen.Instance.IsTransitioning)
            return;
        if (setCinematic)
            FadeTransitionScreen.Instance.SetCinematic(true);
        currentDialogue = dialogue;
        CharacterName.text = dialogue.CharacterName;

        foreach (string sentence in GetCurrentDialogue(dialogue).DialogueLines)
        {
            dialogueQueue.Enqueue(sentence);
        }

        StartCoroutine(RunDialogue(setCinematic));
    }

    public IEnumerator StartDialogueThreaded(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        CharacterName.text = dialogue.CharacterName;
        foreach (string sentence in GetCurrentDialogue(dialogue).DialogueLines)
        {
            dialogueQueue.Enqueue(sentence);
        }
        yield return RunDialogue(false);
    }

    private IEnumerator RunDialogue(bool setCinematic)
    {
        DialogueLines.text = "";
        animator.SetBool("IsOpen", true);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("DialogueBox_Open"))
            yield return null;
        Time.timeScale = 0;
        while (dialogueQueue.Count > 0)
        {
            yield return TypeSentence(dialogueQueue.Dequeue());
            while (!Input.GetButtonDown("Interact"))
                yield return null;
        }
        animator.SetBool("IsOpen", false);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("DialogueBox_Close"))
            yield return null;

        Time.timeScale = 1;

        var dialogueSet = GetCurrentDialogue(currentDialogue);
        if (dialogueSet.ShouldIncreaseQuest)
            QuestSystem.Instance.CompleteQuest(dialogueSet.QuestToComplete);

        if (!string.IsNullOrWhiteSpace(dialogueSet.MemoryToSpawn))
        {
            SoundManager.Instance.PlaySound(SoundManager.Sound.Music_Transition1);
            FindObjectOfType<PlayerController>().SaveScenePositionForMemory();
            FadeTransitionScreen.Instance.Transition(() =>
            {
                SceneManager.LoadScene(dialogueSet.MemoryToSpawn);
            });
        }
        else
        if (setCinematic)
            FadeTransitionScreen.Instance.SetCinematic(false);
    }

    public float TypeSpeed = .05f;
    private IEnumerator TypeSentence(string sentence)
    {
        DialogueLines.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            DialogueLines.text += letter;
            float count = 0;
            while (count < TypeSpeed)
            {
                count += Time.unscaledDeltaTime;
                yield return null;
                if (Input.GetButtonDown("Interact"))
                {
                    DialogueLines.text = sentence;
                    yield return null;
                    yield break;
                }
            }
        }
    }
}
