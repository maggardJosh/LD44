using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneEffectManager : MonoBehaviour
{
    private static CutsceneEffectManager _instance;
    public static CutsceneEffectManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<CutsceneEffectManager>();
            return _instance;
        }
    }


    public Animator Animator;
    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Animator.SetBool("IsCutscene", FadeTransitionScreen.Instance.currentState == FadeTransitionScreen.FadeState.CINEMATIC);
    }


    
}
