using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Queue<string> dialogueQueue;
    public Dialogue currentDialogue;
    public Text DialogueLines;
    public Text CharacterName;

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
        Instance.gameObject.SetActive(false);
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
        gameObject.SetActive(true);
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
        gameObject.SetActive(true);
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
        if (setCinematic)
            FadeTransitionScreen.Instance.SetCinematic(true);
        DisplayNextSentence();
        yield return null;
        while (dialogueQueue.Count >= 0)
        {
            if (Input.GetButtonDown("Interact"))
                if (!DisplayNextSentence())
                    break;
            yield return null;
        }
        if (GetCurrentDialogue(currentDialogue).ShouldIncreaseQuest)
            QuestSystem.Instance.CompleteQuest();
        gameObject.SetActive(false);
        if (setCinematic)
            FadeTransitionScreen.Instance.SetCinematic(false);
    }

    private bool DisplayNextSentence()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return false;
        }

        DialogueLines.text = dialogueQueue.Dequeue();
        return true;
    }

    private void EndDialogue()
    {
        gameObject.SetActive(false);
    }
}
