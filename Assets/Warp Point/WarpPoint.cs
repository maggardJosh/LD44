﻿using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class WarpPoint : MonoBehaviour
{
    public string sceneToWarpTo;
    public string warpPointToSpawnAt;
    public bool isDefaultWarp;
    public Dialogue spawnDialogue;

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToWarpTo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check to see if the object colliding with me is the player
        //warp the player to the new scene
        if (!string.IsNullOrWhiteSpace(sceneToWarpTo) && collision.gameObject.tag == "Player")
        {
            if (!collision.GetComponent<PlayerController>().hasLeftWarp)
                return;
            FadeTransitionScreen.Instance.Transition(() =>
            {
                LoadScene();
                var p = collision.GetComponent<PlayerController>();
                p.targetWarp = warpPointToSpawnAt;
                p.hasLeftWarp = false;
            });
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            collision.GetComponent<PlayerController>().hasLeftWarp = true;
    }
}
