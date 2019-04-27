using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEssentials : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>().Length <= 0)
        {
            GameObject virtualCamera = GameObject.Instantiate(GlobalPrefabs.Instance.VirtualCamera);
            DontDestroyOnLoad(virtualCamera);
        }

        if(FindObjectsOfType<PlayerController>().Length <= 0)
        {
            GameObject player = GameObject.Instantiate(GlobalPrefabs.Instance.PlayerPrefab);
            DontDestroyOnLoad(player);
        }

        if(FindObjectsOfType<Camera>().Length <= 0)
        {
            GameObject camera = Instantiate(GlobalPrefabs.Instance.MainCamera);
            DontDestroyOnLoad(camera);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
