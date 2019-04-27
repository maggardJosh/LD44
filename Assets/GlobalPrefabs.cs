using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPrefabs : MonoBehaviour
{
    private static GlobalPrefabs _instance;
    public static GlobalPrefabs Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GlobalPrefabs>();
            return _instance;
        }
    }
    public GameObject ShadowPrefab;
    public GameObject VirtualCamera;
    public GameObject PlayerPrefab;
    public GameObject MainCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
