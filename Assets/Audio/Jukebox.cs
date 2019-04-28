using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Jukebox : MonoBehaviour
{
    public AudioMixer MainMixer;
    public List<Track> Tracks;

    private void Awake()
    {
        SoundManager.AddTracks(Tracks);
        SoundManager.TrackSettings(SoundManager.Sound.Music_MemoryTheme, MainMixer, 1, true);
        SoundManager.TrackSettings(SoundManager.Sound.Music_TownTheme, MainMixer, 1, true);
    }

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.PlaySound(SoundManager.Sound.Music_TownTheme);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
