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

    public AudioSource Music;
    public AudioSource SFX;

    public static List<Track> TrackList;
    public static AudioMixer MainMixer;

    private static bool keepFadingIn;
    private static bool keepFadingOut;

    private static bool isMuted = false;

    private static float onHoldClipMarker = 0;
    private static Sound onHoldSong = new Sound();

    private const float FADE_RATE = .05f;

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
    
    public static bool PlaySound(Sound soundName, float startingTime = 0)
    {
        Track trackToPlay = GetTrack(soundName);
        if (!trackToPlay.AudioSource.isPlaying)
        {
            trackToPlay.AudioSource.time = startingTime;
            trackToPlay.AudioSource.PlayOneShot(trackToPlay.Clip, trackToPlay.TrackVolume);
            return true;
        }
        return false;
    }

    public static void ToggleMute()
    {
        Instance.Music.mute = !Instance.Music.mute;
        Instance.SFX.mute = !Instance.SFX.mute;
    }

    public static void CallChangeMusic(Sound soundToStop, Sound soundToStart)
    {
        //Putting in the FADE_RATE for now
        Instance.StartCoroutine(ChangeMusic(soundToStop, soundToStart, FADE_RATE));
    }

    public static void CallChangeMusicHold(Sound interruptingMusic)
    {
        Instance.StartCoroutine(ChangeMusicHold(interruptingMusic));
    }

    public static void CallChangeMusicResume()
    {
        Instance.StartCoroutine(ChangeMusicResume());
    }

    private static AudioClip GetSound(Sound soundName)
    {
        return GetTrack(soundName).Clip;
    }

    private static Track GetTrack(Sound soundName)
    {
        return TrackList.Single(s => s.ClipName == soundName);
    }
    
    private static  IEnumerator FadeInMusic (float speed, float maxVolume)
    {
        keepFadingIn = true;
        keepFadingOut = false;

        Instance.Music.volume = 0;
        float audioVolume = Instance.Music.volume;

        while (Instance.Music.volume < maxVolume && keepFadingIn)
        {
            audioVolume += speed;
            Instance.Music.volume = audioVolume;
            yield return null;
        }
    }

    private static IEnumerator FadeOutMusic(float speed)
    {
        keepFadingIn = false;
        keepFadingOut = true;
        
        float audioVolume = Instance.Music.volume;

        while (Instance.Music.volume >= speed && keepFadingOut)
        {
            audioVolume -= speed;
            Instance.Music.volume = audioVolume;
            yield return null;
        }

        Instance.Music.Stop();
    }
    
    private static IEnumerator ChangeMusic(Sound soundToStop, Sound soundToStart, float speedToChange)
    {
        yield return FadeOutMusic(speedToChange);
        
        if (PlaySound(soundToStart))
            yield return FadeInMusic(FADE_RATE, GetTrack(soundToStart).TrackVolume);
        
    }

    private static IEnumerator ChangeMusicHold(Sound interruptingMusic)
    {
        onHoldClipMarker = 0;

        onHoldSong = TrackList.Single(s => s.Clip.name == Instance.Music.clip.name).ClipName;

        onHoldClipMarker = Instance.Music.time;

        yield return FadeOutMusic(FADE_RATE);

        if (PlaySound(interruptingMusic))
            yield return FadeInMusic(FADE_RATE, GetTrack(interruptingMusic).TrackVolume);
        
    }

    private static IEnumerator ChangeMusicResume()
    {
        yield return FadeOutMusic(FADE_RATE);

        if (PlaySound(onHoldSong, onHoldClipMarker))
            yield return FadeInMusic(FADE_RATE, GetTrack(onHoldSong).TrackVolume);
        
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
