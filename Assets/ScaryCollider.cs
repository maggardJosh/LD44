using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaryCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FadeTransitionScreen.Instance.Transition(() =>
            {
                var player = collision.GetComponent<PlayerController>();
                player.targetWarp = "ScarySpawn";
                SceneManager.LoadScene("Town");
            });
        }
    }
}
