using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SoundManager>();
            return _instance;
        }
    }

    public enum Sound
    {
        Music_TownTheme,
        Music_MemoryTheme
    }

    public enum SoundType
    {
        Music,
        SFX
    }

    public int Priority;
    [SerializeField]
    public static List<Track> TrackList;
    public static AudioMixer MainMixer;

    private static bool keepFadingIn;
    private static bool keepFadingOut;

    public static void AddTracks(List<Track> tracks)
    {
        TrackList = new List<Track>();
        foreach (Track inputTrack in tracks)
        {
            TrackList.Add(inputTrack);
        }
    }



    public static void TrackSettings(Sound soundToPlay, AudioMixer mainMix, float trackVolume, bool loop = false)
    {
        Track settingTrack = GetTrack(soundToPlay);
        settingTrack.AudioSource.outputAudioMixerGroup = mainMix.FindMatchingGroups(GetMixerGroup(settingTrack.ClipType))[0];
        settingTrack.TrackVolume = trackVolume;
    }

    public static void PlaySound(Sound soundName)
    {
        Track trackToPlay = GetTrack(soundName);
        if (!trackToPlay.AudioSource.isPlaying)
        {
            trackToPlay.AudioSource.PlayOneShot(trackToPlay.Clip, trackToPlay.TrackVolume);
            
        }
    }

    private static AudioClip GetSound(Sound soundName)
    {
        return GetTrack(soundName).Clip;
    }

    private static Track GetTrack(Sound soundName)
    {
        return TrackList.Single(s => s.ClipName == soundName);
    }

    private AudioSource Source()
    {
        return gameObject.GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static void CallLoop(Sound soundToLoop, float clipLength)
    {
        Instance.StartCoroutine(Loop(soundToLoop, clipLength));
    }

    public static void CallChangeMusic(Sound soundToStop, Sound soundToStart, float speedToChange)
    {
        Instance.StartCoroutine(ChangeMusic(soundToStop, soundToStart, speedToChange));
    }

    private static  IEnumerator FadeIn (Sound soundToPlay, float speed, float maxVolume)
    {
        keepFadingIn = true;
        keepFadingOut = false;

        GetTrack(soundToPlay).AudioSource.volume = 0;
        float audioVolume = GetTrack(soundToPlay).AudioSource.volume;

        while (GetTrack(soundToPlay).AudioSource.volume < maxVolume && keepFadingIn)
        {
            audioVolume += speed;
            GetTrack(soundToPlay).AudioSource.volume = audioVolume;
            yield return new WaitForFixedUpdate();
        }
    }

    private static IEnumerator FadeOut(Sound sound, float speed)
    {
        keepFadingIn = false;
        keepFadingOut = true;
        Track soundToFade = GetTrack(sound);
        
        float audioVolume = GetTrack(sound).AudioSource.volume;

        while (soundToFade.AudioSource.volume >= speed && keepFadingOut)
        {
            audioVolume -= speed;
            soundToFade.AudioSource.volume = audioVolume;
            yield return null;
        }

        soundToFade.AudioSource.Stop();
    }

    private static IEnumerator Loop(Sound soundToLoop, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);

        PlaySound(soundToLoop);
    }

    private static IEnumerator ChangeMusic(Sound soundToStop, Sound soundToStart, float speedToChange)
    {
        yield return FadeOut(soundToStop, speedToChange);
        

        Track trackToPlay = GetTrack(soundToStart);
        if (!trackToPlay.AudioSource.isPlaying)
        {
            trackToPlay.AudioSource.PlayOneShot(trackToPlay.Clip, trackToPlay.TrackVolume);
            yield return FadeIn(soundToStart, speedToChange, trackToPlay.TrackVolume);
        }
    }

    private static string GetMixerGroup(SoundType type)
    {
        switch (type)
        {
            case SoundType.Music:
                return "Music";
            case SoundType.SFX:
                return "SFX";
            default:
                return "Music";
        }
    }
}
