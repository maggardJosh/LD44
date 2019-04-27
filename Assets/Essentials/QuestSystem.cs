using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public enum QuestState
    {
        A_JUST_STARTED = 0,
        B_FIRST_QUEST_DONE = 1
    }
    private static QuestSystem _instance;
    public static QuestSystem Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<QuestSystem>();
            return _instance;
        }
    }

    public QuestState CurrentState = QuestState.A_JUST_STARTED;
    public void CompleteQuest()
    {
        CurrentState++;
    }

}
