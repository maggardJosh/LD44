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
        player.SaveScenePositionForMemory();
        SoundManager.Instance.PlaySound(SoundManager.Sound.Music_Transition1);

        PauseMenuManager.Instance.PressPause();
        FadeTransitionScreen.Instance.Transition(() =>
        {
            SceneManager.LoadScene(memoryScene);
        });
    }
}
