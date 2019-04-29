using UnityEngine;
using UnityEngine.SceneManagement;

public class MemoryLoadOnCollide : MonoBehaviour
{
    public string MemoryScene = "";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !string.IsNullOrWhiteSpace(MemoryScene))
        {
            PlayerController p = collision.GetComponent<PlayerController>();
            p.SaveScenePositionForMemory();
            SoundManager.Instance.PlaySound(SoundManager.Sound.Music_Transition1);
            FadeTransitionScreen.Instance.Transition(() =>
            {
                SceneManager.LoadScene(MemoryScene);
            });
        }
    }
}
