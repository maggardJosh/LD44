using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class WarpPoint : MonoBehaviour
{
    public string warpPointToSpawnAt;
    public string sceneToWarpTo;
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToWarpTo);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Boop");
        //check to see if the object colliding with me is the player
        //warp the player to the new scene
        if (collision.gameObject.tag == "Player")
        {
            //
            LoadScene();
            List<WarpPoint> points = new List<WarpPoint>(FindObjectsOfType<WarpPoint>());
            foreach (WarpPoint p in points)
            {
                if (p.name == warpPointToSpawnAt)
                {
                    collision.gameObject.transform.position = p.transform.position;
                    return;
                }
            }
            //if not then send us somewhere
        }
    }
}
