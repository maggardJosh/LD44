using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DialogueComponent : MonoBehaviour
{
    public Dialogue Dialogue;
    public bool canInteract = false;
    private InteractIndicator interactIndicator;

    private void Start()
    {
        interactIndicator = Instantiate(GlobalPrefabs.Instance.InteractIndicatorPrefab, transform).GetComponent<InteractIndicator>();
        interactIndicator.transform.localPosition = Vector3.zero;
        interactIndicator.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: Stop characters from walking around when we are near them, they should also turn towards us
        if (collision.CompareTag("Player"))
        {
            canInteract = true;
            interactIndicator.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
            interactIndicator.gameObject.SetActive(false);
        }
    }

    public bool TryInteract()
    {
        if (canInteract && Input.GetButtonDown("Interact"))
        {
            ShowDialogue();
            return true;
        }
        return false;
    }

    private void ShowDialogue()
    {
        var set = GetCurrentDialogue();
        //TODO: Dan - Actually show dialogue here and handle increasing quest at the end of dialogue
        foreach (string message in set.DialogueLines)
            Debug.Log(message);
        if (set.ShouldIncreaseQuest)
            QuestSystem.Instance.CompleteQuest();
    }

    private Dialogue.DialogueSet GetCurrentDialogue()
    {
        foreach (var set in Dialogue.dialogueEntries.OrderBy(s => (int)s.MinQuestLevel))
            if (set.MinQuestLevel >= QuestSystem.Instance.CurrentState)
                return set;

        return Dialogue.dialogueEntries.LastOrDefault();
    }
}
