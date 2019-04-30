using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestSystem : MonoBehaviour
{
    public enum QuestState
    {
        Q0_FIRST_LOAD = -1,
        Q1_ACCESS_MEMORY = 0,
        Q2_GO_TO_GRAVEYARD = 1,
        Q3_RETRIEVE_NPC_ITEM = 2,
        Q4_RETURN_NPC_ITEM = 3,
        Q5_RETRIEVE_WHIP = 4,
        Q6_WANDER_TOWN = 5,
        Q7_TALK_TO_JIM = 6,
        Q8_RESCUE_RALPH = 7,
        Q9_RETURN_TO_JIM = 8,
        Q10_GET_LOCKET = 9,
        Q11_FIGHT_BOSS = 10
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
    private QuestState _currentState = QuestState.Q0_FIRST_LOAD;
    public QuestState CurrentState
    {
        get { return _currentState; }
    }
    public void CompleteQuest(QuestState s)
    {
        if (_currentState > s)
            return;
        else
            _currentState = s;
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
        {QuestState.Q1_ACCESS_MEMORY, "Access Memory" },
        {QuestState.Q2_GO_TO_GRAVEYARD, "Go to graveyard" },
        {QuestState.Q3_RETRIEVE_NPC_ITEM, "Retrieve Maranda's ITEM" },
        {QuestState.Q4_RETURN_NPC_ITEM, "Return Maranda's ITEM" },
        {QuestState.Q5_RETRIEVE_WHIP, "Go to the hut east of town" },
        {QuestState.Q6_WANDER_TOWN, "Talk to people in the town" },
        {QuestState.Q7_TALK_TO_JIM, "Talk to Jim (Nort-West of town)" },
        {QuestState.Q8_RESCUE_RALPH, "SAVE RALPH! (North of town)" },
        {QuestState.Q9_RETURN_TO_JIM, "Return back to Jim (North-West of town)" },
        {QuestState.Q10_GET_LOCKET, "Enter the graveyard" },
        {QuestState.Q11_FIGHT_BOSS, "Fight the BOSS!" }
    };

}
