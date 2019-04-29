using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MemoryButton : MonoBehaviour
{
    public string memoryScene = "Cutscene1";
    
    public void LoadMemory()
    {
        var player = FindObjectOfType<PlayerController>();
        player.lastPos = player.transform.position;
        player.sceneToWarpBackTo = SceneManager.GetActiveScene().name;
        Debug.Log(SceneManager.GetActiveScene().name);

        PauseMenuManager.Instance.PressPause();
        FadeTransitionScreen.Instance.Transition(() =>
        {
            SceneManager.LoadScene(memoryScene);
        });
    }
}
