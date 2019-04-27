using UnityEngine;

public class LoadEssentials : MonoBehaviour
{
    void Awake()
    {
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
        if (FindObjectOfType<DialogueManager>() == null)
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
            GameObject player = GameObject.Instantiate(GlobalPrefabs.Instance.PlayerPrefab);
            var virtualCam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().Follow = player.transform;
            DontDestroyOnLoad(player);
        }

        if (FindObjectsOfType<Camera>().Length <= 0)
        {
            GameObject camera = Instantiate(GlobalPrefabs.Instance.MainCamera);
            DontDestroyOnLoad(camera);
        }
    }
}
