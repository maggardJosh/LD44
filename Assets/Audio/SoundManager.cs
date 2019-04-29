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

    public enum MusicSate
    {
        FadingIn,
        FadingOut,
        Playing,
        Stopped
    }

    public AudioSource Music;
    public AudioSource SFX;

    public static List<Track> TrackList;
    public static AudioMixer MainMixer;

    private static bool keepFadingIn;
    private static bool keepFadingOut;

    private static bool isMuted = false;

    private  float onHoldClipMarker;
    private  Sound onHoldSong;

    private MusicSate currentState = MusicSate.Playing;

    private const float FADE_RATE = .05f;

    public static void AddTracks(List<Track> tracks)
    {
        TrackList = new List<Track>();
        foreach (Track inputTrack in tracks)
        {
            TrackList.Add(inputTrack);
        }
    }



    public void TrackSettings(Sound soundToPlay, AudioMixer mainMix, float trackVolume, bool loop = false)
    {
        Track settingTrack = GetTrack(soundToPlay);
        settingTrack.AudioSource.outputAudioMixerGroup = mainMix.FindMatchingGroups(GetMixerGroup(settingTrack.ClipType))[0];
        settingTrack.TrackVolume = trackVolume;
    }
    
    public bool PlaySound(Sound soundName, float startingTime = 0)
    {
        Track trackToPlay = GetTrack(soundName);
        if (!trackToPlay.AudioSource.isPlaying)
        {
            trackToPlay.AudioSource.time = startingTime;
            //trackToPlay.AudioSource.PlayOneShot(trackToPlay.Clip, trackToPlay.TrackVolume);
            trackToPlay.AudioSource.clip = trackToPlay.Clip;
            trackToPlay.AudioSource.Play();
            return true;
        }
        return false;
    }

    public static void ToggleMute()
    {
        Instance.Music.mute = !Instance.Music.mute;
        Instance.SFX.mute = !Instance.SFX.mute;
    }

    public void CallChangeMusic(Sound soundToStop, Sound soundToStart)
    {
        if (currentState == MusicSate.FadingIn || currentState == MusicSate.FadingOut)
            return;
        StartCoroutine(ChangeMusic(soundToStop, soundToStart, FADE_RATE));
    }

    public void CallChangeMusicHold(Sound interruptingMusic)
    {
        if (currentState == MusicSate.FadingIn || currentState == MusicSate.FadingOut)
            return;
        StartCoroutine(ChangeMusicHold(interruptingMusic));
    }

    public void CallChangeMusicResume()
    {
        if (currentState == MusicSate.FadingIn || currentState == MusicSate.FadingOut)
            return;
        StartCoroutine(ChangeMusicResume());
    }

    private AudioClip GetSound(Sound soundName)
    {
        return GetTrack(soundName).Clip;
    }

    private Track GetTrack(Sound soundName)
    {
        return TrackList.Single(s => s.ClipName == soundName);
    }
    
    private  IEnumerator FadeInMusic (float speed, float maxVolume)
    {
        SetState(MusicSate.FadingIn);
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
        SetState(MusicSate.Playing);
    }

    private IEnumerator FadeOutMusic(float speed)
    {
        SetState(MusicSate.FadingOut);
        keepFadingIn = false;
        keepFadingOut = true;
        
        float audioVolume = Music.volume;

        while (Music.volume >= speed && keepFadingOut)
        {
            audioVolume -= speed;
            Music.volume = audioVolume;
            yield return null;
        }

        Music.Stop();
        SetState(MusicSate.Stopped);
    }
    
    private IEnumerator ChangeMusic(Sound soundToStop, Sound soundToStart, float speedToChange)
    {
        yield return FadeOutMusic(speedToChange);
        
        if (PlaySound(soundToStart))
            yield return FadeInMusic(FADE_RATE, GetTrack(soundToStart).TrackVolume);
        
    }

    private  IEnumerator ChangeMusicHold(Sound interruptingMusic)
    {
        onHoldClipMarker = 0;

        onHoldSong = TrackList.Single(s => s.Clip.name == Music.clip.name).ClipName;

        onHoldClipMarker = Music.time;

        yield return FadeOutMusic(FADE_RATE);

        if (PlaySound(interruptingMusic))
            yield return FadeInMusic(FADE_RATE, GetTrack(interruptingMusic).TrackVolume);
        
    }

    private IEnumerator ChangeMusicResume()
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

    private void SetState(MusicSate newState)
    {
        currentState = newState;
    }
    
}
