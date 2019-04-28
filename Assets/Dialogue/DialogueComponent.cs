using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DialogueComponent : MonoBehaviour
{
    public Dialogue Dialogue;
    public bool canInteract = false;
    private InteractIndicator interactIndicator;
    private GameObject player;

    private void Start()
    {
        interactIndicator = Instantiate(GlobalPrefabs.Instance.InteractIndicatorPrefab, transform).GetComponent<InteractIndicator>();
        interactIndicator.transform.localPosition = Vector3.zero;
        interactIndicator.gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = true;
            interactIndicator.gameObject.SetActive(true);
            SoundManager.CallChangeMusic(SoundManager.Sound.Music_TownTheme, SoundManager.Sound.Music_MemoryTheme, .05f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.gameObject;
            GetComponentInParent<Npc>().Interact(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null;
            canInteract = false;
            interactIndicator.gameObject.SetActive(false);
            Npc npc = GetComponentInParent<Npc>();
            if (npc != null) //Check just in case they were removed based on quest this frame
                npc.StopInteracting();
            SoundManager.CallChangeMusic(SoundManager.Sound.Music_MemoryTheme, SoundManager.Sound.Music_TownTheme, .05f);
        }
    }

    public bool TryInteract()
    {
        if (canInteract && Input.GetButtonDown("Interact"))
        {
            player.StopTopDownController();
            DialogueManager.Instance.StartDialogue(Dialogue);
            return true;
        }
        return false;
    }
}
