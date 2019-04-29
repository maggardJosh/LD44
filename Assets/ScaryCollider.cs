using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaryCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            FadeTransitionScreen.Instance.Transition(() =>
            {
                var player = collision.GetComponent<PlayerController>();
                player.targetWarp = "ScarySpawn";
                SceneManager.LoadScene("Town");
            });
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
