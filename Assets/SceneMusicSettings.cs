using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMusicSettings : MonoBehaviour
{
    public SoundManager.Sound SceneSong;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.CallChangeMusic(SceneSong);
        SoundManager.Instance.PlayMusic(SoundManager.Sound.SFX_Ambience_Other);
    }
    
}
