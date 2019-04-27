using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue/Create Dialogue", order = 1)]
public class Dialogue : ScriptableObject
{
    [Serializable]
    public class DialogueSet
    {
        public QuestSystem.QuestState MinQuestLevel = QuestSystem.QuestState.A_JUST_STARTED;
        [TextArea(2,8)]
        public List<string> DialogueLines = new List<string>();
        public bool ShouldIncreaseQuest = false;
    }
    [SerializeField]
    public List<DialogueSet> dialogueEntries;
    public string CharacterName = "Character";
}
