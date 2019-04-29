using UnityEngine;

public class QuestAffectedItem : MonoBehaviour
{
    [Header("General Config")]
    [SerializeField]
    private QuestSystem.QuestState questState = QuestSystem.QuestState.B_WHIP_GOT;
    [SerializeField]
    private bool shouldHappenImmediately = false;
    
    [Header("Conditions")]
    public bool shouldHideBeforeComplete = false;
    public bool shouldHideAsCurrent = false;
    public bool shouldHideAfterComplete = false;
    
    public bool shouldShowBeforeComplete = false;
    public bool shouldShowAsCurrent = false;
    public bool shouldShowAfterComplete = false;


    // Start is called before the first frame update
    void Start()
    {
        UpdateActiveBasedOnCurrentQuest(true);
    }

    public void UpdateActiveBasedOnCurrentQuest(bool onStart = false)
    {
        if (!shouldHappenImmediately && !onStart)
            return;

        bool result = GetShouldShow();

        gameObject.SetActive(result);
    }

    private bool GetShouldShow()
    {
        if (shouldHideBeforeComplete && QuestSystem.Instance.CurrentState < questState)
            return false;
        if (shouldHideAsCurrent && QuestSystem.Instance.CurrentState == questState)
            return false;
        if (shouldHideAfterComplete && QuestSystem.Instance.CurrentState > questState)
            return false;

        if (shouldShowBeforeComplete && QuestSystem.Instance.CurrentState < questState)
            return true;
        if (shouldShowAsCurrent && QuestSystem.Instance.CurrentState == questState)
            return true;
        if (shouldShowAfterComplete && QuestSystem.Instance.CurrentState > questState)
            return true;

        return false;
    }
}
