using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Create Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    [Serializable]
    public class DialogueSet
    {
        public QuestSystem.QuestState MinQuestLevel = QuestSystem.QuestState.Q1_ACCESS_MEMORY;
        [TextArea(2,8)]
        public List<string> DialogueLines = new List<string>();
        public bool ShouldIncreaseQuest = false;
        public QuestSystem.QuestState QuestToComplete = QuestSystem.QuestState.Q0_FIRST_LOAD;
        public string MemoryToSpawn = "";
    }
    public string CharacterName = "Character";
    [Header("Dialogue Entries")]
    [SerializeField]
    public List<DialogueSet> dialogueEntries;
}
