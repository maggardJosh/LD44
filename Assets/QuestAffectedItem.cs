using UnityEngine;

public class QuestAffectedItem : MonoBehaviour
{
    [SerializeField]
    private bool activeSettingWhenQuestComplete = false;
    [SerializeField]
    private bool shouldHappenImmediately = false;
    [SerializeField]
    private QuestSystem.QuestState questState = QuestSystem.QuestState.B_FIRST_QUEST_DONE;
    // Start is called before the first frame update
    void Start()
    {
        UpdateActiveBasedOnCurrentQuest(true);
    }

    public void UpdateActiveBasedOnCurrentQuest(bool onStart = false)
    {
        if (!shouldHappenImmediately && !onStart)
            return;
        if (QuestSystem.Instance.CurrentState > questState)
            gameObject.SetActive(activeSettingWhenQuestComplete);
        else
            gameObject.SetActive(!activeSettingWhenQuestComplete);
    }
}
