using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Track
{
    public SoundManager.Sound ClipName;
    public SoundManager.SoundType ClipType;
    public AudioClip Clip;
    public AudioSource AudioSource;
    public float TrackVolume;
    public bool ShouldLoop;
}