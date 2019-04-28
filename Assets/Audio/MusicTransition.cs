using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicTransition : MonoBehaviour
{
    private static MusicTransition _instance;
    public static MusicTransition Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<MusicTransition>();
            return _instance;
        }
    }

    

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
