using System;
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

    public QuestState CurrentState { get; private set; } = QuestState.A_JUST_STARTED;
    public void CompleteQuest()
    {
        CurrentState++;
        if (!Enum.IsDefined(typeof(QuestState), CurrentState))
            CurrentState--;
        foreach (QuestAffectedItem item in Resources.FindObjectsOfTypeAll<QuestAffectedItem>())
            item.UpdateActiveBasedOnCurrentQuest();
    }

    public static string GetCurrentQuestHint(QuestState questState)
    {

        if (QuestHints.TryGetValue(questState, out string hint))
            return hint;
        else
            return "";
    }

    private static Dictionary<QuestState, string> QuestHints = new Dictionary<QuestState, string>
    {
        {QuestState.A_JUST_STARTED, "You should totally go talk to one of the dues.\nOne that isn't Fargoth or that Miner guy." },
        {QuestState.B_FIRST_QUEST_DONE, "NICE\nNow you gotta wait for us to add more stuff" }
    };

}
