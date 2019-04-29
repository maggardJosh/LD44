using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public enum QuestState
    {
        A_JUST_STARTED = 0,
        B_WHIP_GOT = 1,
        C_DIVE_GOT = 2,
        D_UNKNOWN = 3
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

    [SerializeField]
    private QuestState _currentState = QuestState.A_JUST_STARTED;
    public QuestState CurrentState
    {
        get { return _currentState; }
    }
    public void CompleteQuest()
    {
        _currentState++;
        if (!Enum.IsDefined(typeof(QuestState), CurrentState))
            _currentState--;
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
        {QuestState.B_WHIP_GOT, "You should go find your whip bro" },
        {QuestState.C_DIVE_GOT, "You should go find your dive bruv" },
        {QuestState.D_UNKNOWN, "All done for now" }
    };

}
