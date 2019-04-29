using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Jukebox : MonoBehaviour
{
    //public AudioMixer MainMixer;
    public List<Track> Tracks;

    private void Awake()
    {
        //SoundManager.Instance.AddTracks(Tracks);
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
