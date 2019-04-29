using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public Text CurrentQuestHint;
    private State CurrentState;

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

    public void SetQuestHint()
    {
        CurrentQuestHint.text = QuestSystem.GetCurrentQuestHint(QuestSystem.Instance.CurrentState);
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
        SetQuestHint();
        gameObject.SetActive(true);
        CurrentState = State.MenuOpen;
    }

    private void HideMenu()
    {
        gameObject.SetActive(false);
        CurrentState = State.MenuClosed;
    }
}
