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

    [SerializeField]
    private Sprite locketSprite;

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
            case QuestSystem.QuestState.Q0_FIRST_LOAD:
            case QuestSystem.QuestState.Q1_ACCESS_MEMORY:
            case QuestSystem.QuestState.Q2_GO_TO_GRAVEYARD:
            case QuestSystem.QuestState.Q3_RETRIEVE_NPC_ITEM:
                ShowMemoriesUpTo(1);
                break;
            case QuestSystem.QuestState.Q4_RETURN_NPC_ITEM:
                ShowMemoriesUpTo(1);
                ShowImage(0, itemOneSprite);
                break;
            case QuestSystem.QuestState.Q5_RETRIEVE_WHIP:
                ShowMemoriesUpTo(1);
                break;
            case QuestSystem.QuestState.Q6_WANDER_TOWN:
            case QuestSystem.QuestState.Q7_TALK_TO_JIM:
            case QuestSystem.QuestState.Q8_RESCUE_RALPH:
                ShowMemoriesUpTo(2);
                ShowImage(0, whipSprite);
                break;
            case QuestSystem.QuestState.Q9_RETURN_TO_JIM:
                ShowMemoriesUpTo(3);
                ShowImage(0, whipSprite);
                break;
            case QuestSystem.QuestState.Q10_GET_LOCKET:
                ShowMemoriesUpTo(4);
                ShowImage(0, whipSprite);
                break;
            case QuestSystem.QuestState.Q11_FIGHT_BOSS:
                ShowMemoriesUpTo(5);
                ShowImage(0, whipSprite);
                ShowImage(1, locketSprite);
                break;
            default:
                ShowMemoriesUpTo(8);
                ShowImage(0, whipSprite);
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
