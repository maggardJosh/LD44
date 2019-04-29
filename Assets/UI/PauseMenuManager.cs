using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public Text CurrentQuestHint;
    private State CurrentState;

    [SerializeField]
    private Image[] ItemImages;

    [SerializeField]
    private Button[] MemoryButtons;

    [SerializeField]
    private Sprite whipSprite;

    [SerializeField]
    private Sprite itemOneSprite;

    private enum State
    {
        MenuOpen,
        MenuClosed
    }

    private static PauseMenuManager _instance;
    public static PauseMenuManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PauseMenuManager>();
            return _instance;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        CurrentState = State.MenuClosed;
        Instance.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void HideAllItems()
    {
        foreach (var item in ItemImages)
            item.enabled = false;
    }

    private void HideAllMemories()
    {
        foreach (var mem in MemoryButtons)
            mem.GetComponent<Image>().enabled = false;
    }
    public void UpdateScreen()
    {
        var qLevel = QuestSystem.Instance.CurrentState;
        CurrentQuestHint.text = QuestSystem.GetCurrentQuestHint(qLevel);
        HideAllItems();
        HideAllMemories();
        switch (qLevel)
        {
            case QuestSystem.QuestState.A_JUST_STARTED:
                ShowMemoriesUpTo(1);
                break;
            case QuestSystem.QuestState.B_WHIP_GOT:
                ShowMemoriesUpTo(1);
                ShowImage(0, itemOneSprite);
                break;
            case QuestSystem.QuestState.C_DIVE_GOT:
                ShowMemoriesUpTo(2);
                ShowImage(0, whipSprite);

                break;
            default:
                ShowMemoriesUpTo(8);
                ShowImage(0, itemOneSprite);
                ShowImage(1, whipSprite);
                break;
        }
    }

    private void ShowImage(int i, Sprite image)
    {
        ItemImages[i].enabled = true;
        ItemImages[i].sprite = image;
    }

    private void ShowMemoriesUpTo(int i)
    {
        for (int j = 0; j < i; j++)
            MemoryButtons[j].GetComponent<Image>().enabled = true;
    }

    private void CheckAndSwitchState()
    {
        switch (CurrentState)
        {
            case State.MenuOpen:
                HideMenu();
                break;
            case State.MenuClosed:
                ShowMenu();
                break;
            default:
                HideMenu();
                break;
        }
    }

    public void PressPause()
    {
        CheckAndSwitchState();
    }

    private void ShowMenu()
    {
        UpdateScreen();
        gameObject.SetActive(true);
        CurrentState = State.MenuOpen;
    }

    private void HideMenu()
    {
        gameObject.SetActive(false);
        CurrentState = State.MenuClosed;
    }
}
