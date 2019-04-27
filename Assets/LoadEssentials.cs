﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEssentials : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(FindObjectOfType<Cinemachine.CinemachineVirtualCamera>().gameObject);
        DontDestroyOnLoad(FindObjectOfType<PlayerController>().gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
