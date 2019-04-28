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
        //gameObject.SetActive(true);
        animator.SetBool("IsOpen", true);


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

        StopAllCoroutines();
        StartCoroutine(TypeSentence(dialogueQueue.Dequeue()));

    }

    public void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        //gameObject.SetActive(false);
    }

    private IEnumerator TypeSentence(string sentence)
    {
        DialogueLines.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            DialogueLines.text += letter;
            yield return null;
        }
    }
}
