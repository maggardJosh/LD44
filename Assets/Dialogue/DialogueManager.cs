using System.Collections;
using System.Collections.Generic;
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDialogueName(string dialogueName)
    {
        CharacterName.text = dialogueName;
    }

    public void StartDialogue(Dialogue.DialogueSet dialogueSet)
    {
        //DialogueLines.text = dialogueSet.DialogueLines[0];
        //display the queue of text and advance if we have more lines if the player hits interact
        //dialogueQueue.Clear();
        gameObject.SetActive(true);

        foreach (string sentence in dialogueSet.DialogueLines)
        {
            dialogueQueue.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLines.text = dialogueQueue.Dequeue();

    }

    public void EndDialogue()
    {
        gameObject.SetActive(false);
    }
}
