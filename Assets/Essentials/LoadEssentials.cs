using UnityEngine;

public class LoadEssentials : MonoBehaviour
{
    void Awake()
    {
        if (TutorialTextManager.Instance == null)
        {
            GameObject tutorialText = Instantiate(GlobalPrefabs.Instance.TutorialTextManager);
            DontDestroyOnLoad(tutorialText);
        }
        if (CutsceneEffectManager.Instance == null)
        {
            GameObject cutsceneBorder = Instantiate(GlobalPrefabs.Instance.CutsceneBorder);
            DontDestroyOnLoad(cutsceneBorder);
        }
        if (SoundManager.Instance == null)
        {
            GameObject soundManager = Instantiate(GlobalPrefabs.Instance.SoundManager);
            DontDestroyOnLoad(soundManager);
        }
        if (PauseMenuManager.Instance == null)
        {
            GameObject pauseMenu = Instantiate(GlobalPrefabs.Instance.PauseCanvas);
            DontDestroyOnLoad(pauseMenu);
        }
        if (FindObjectOfType<FadeTransitionScreen>() == null)
        {
            GameObject transScreen = Instantiate(GlobalPrefabs.Instance.TransitionCanvas);
            DontDestroyOnLoad(transScreen);
        }
        if (FindObjectOfType<QuestSystem>() == null)
        {
            GameObject questSystem = GameObject.Instantiate(GlobalPrefabs.Instance.QuestSystem);
            DontDestroyOnLoad(questSystem);
        }
        if (FindObjectOfType<HealthGUI>() == null)
        {
            GameObject healthUI = Instantiate(GlobalPrefabs.Instance.HealthCanvas);
            DontDestroyOnLoad(healthUI);
        }
        if (DialogueManager.Instance == null)
        {
            GameObject dialogueManager = Instantiate(GlobalPrefabs.Instance.DialogueCanvas);
            DontDestroyOnLoad(dialogueManager);
        }
        if (FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>().Length <= 0)
        {
            GameObject virtualCamera = GameObject.Instantiate(GlobalPrefabs.Instance.VirtualCamera);
        }

        if (FindObjectsOfType<PlayerController>().Length <= 0)
        {
            PlayerController player = GameObject.Instantiate(GlobalPrefabs.Instance.PlayerPrefab).GetComponent<PlayerController>();
            var virtualCam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = player.followTransform;
            DontDestroyOnLoad(player.gameObject);
        }

        if (FindObjectsOfType<Camera>().Length <= 0)
        {
            GameObject camera = Instantiate(GlobalPrefabs.Instance.MainCamera);
            DontDestroyOnLoad(camera);
        }
    }
}
