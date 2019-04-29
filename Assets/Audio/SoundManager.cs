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

    #region enums
    public enum Sound
    {
        Music_Battle,
        Music_Boss,
        Music_GraveyardBossLair,
        Music_Memory,
        Music_Menu,
        Music_Town,
        Music_Transition1,
        Music_Transition2,
        SFX_Ambience_Other,
        SFX_Ambience_Town,
        SFX_Boss_Damage,
        SFX_Boss_Death,
        SFX_Boss_EnergyBlast,
        SFX_Boss_Roar,
        SFX_Boss_Swipe,
        SFX_Boss_Walk1,
        SFX_Boss_Walk2,
        SFX_Boss_Walk3,
        SFX_DialogueBlip1,
        SFX_DialogueBlip2,
        SFX_DialogueBlip3,
        SFX_Enemy_EnergyDamage,
        SFX_Enemy_EnergyDie,
        SFX_Enemy_EnergyExplosion,
        SFX_Enemy_EnergyMortar,
        SFX_Enemy_EnergyPunch,
        SFX_Enemy_EnergyWalk1,
        SFX_Enemy_EnergyWalk2,
        SFX_LaserWhip,
        SFX_Menu_Click,
        SFX_Menu_Select,
        SFX_Player_Damage,
        SFX_Player_Die,
        SFX_Player_DodgeRoll,
        SFX_Player_Walk1,
        SFX_Player_Walk2,
        SFX_Player_Walk3
    }

    public enum SoundType
    {
        Music,
        SFX,
        Ambient
    }

    public enum MusicSate
    {
        FadingIn,
        FadingOut,
        Playing,
        Stopped
    }
    #endregion
    
    public List<Track> TrackList;

    private static bool keepFadingIn;
    private static bool keepFadingOut;

    private static bool isMuted = false;

    private  float onHoldClipMarker;
    private  Sound onHoldSong;

    private MusicSate currentState = MusicSate.Playing;

    private const float FADE_RATE = .05f;

    public void AddTracks(List<Track> tracks)
    {
        TrackList = new List<Track>();
        foreach (Track inputTrack in tracks)
        {
            TrackList.Add(inputTrack);
        }
    }



    public void TrackSettings(Sound soundToPlay, AudioMixer mainMix, float trackVolume)
    {
        Track settingTrack = GetTrack(soundToPlay);
        settingTrack.AudioSource.outputAudioMixerGroup = mainMix.FindMatchingGroups(GetMixerGroup(settingTrack.ClipType))[0];
        settingTrack.TrackVolume = trackVolume;
    }
    
    public bool PlayMusic(Sound soundName, float startingTime = 0)
    {
        Track trackToPlay = GetTrack(soundName);
        if (!trackToPlay.AudioSource.isPlaying)
        {
            trackToPlay.AudioSource.time = startingTime;
            trackToPlay.AudioSource.clip = trackToPlay.Clip;
            trackToPlay.AudioSource.Play();
            return true;
        }
        return false;
    }

    public void PlaySound(Sound soundName)
    {
        Track trackToPlay = GetTrack(soundName);
        trackToPlay.AudioSource.PlayOneShot(trackToPlay.Clip);
    }

    public static void ToggleMute()
    {
        if (AudioListener.volume > 0)
            AudioListener.volume = 0;
        else
            AudioListener.volume = 1;
    }

    public void CallChangeMusic(Sound soundToStart)
    {
        if (currentState == MusicSate.FadingIn || currentState == MusicSate.FadingOut)
            return;
        if (SongIsAlreadyPlaying(soundToStart))
            return;
        StartCoroutine(ChangeMusic(soundToStart, FADE_RATE));
    }

    public void CallChangeMusicHold(Sound interruptingMusic)
    {
        if (currentState == MusicSate.FadingIn || currentState == MusicSate.FadingOut)
            return;
        if (SongIsAlreadyPlaying(interruptingMusic))
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
    
    private  IEnumerator FadeInMusic (float speed, Track track)
    {
        SetState(MusicSate.FadingIn);
        keepFadingIn = true;
        keepFadingOut = false;

        track.AudioSource.volume = 0;
        float audioVolume = track.AudioSource.volume;

        while (track.AudioSource.volume < track.TrackVolume && keepFadingIn)
        {
            audioVolume += speed;
            track.AudioSource.volume = audioVolume;
            yield return null;
        }
        SetState(MusicSate.Playing);
    }

    private IEnumerator FadeOutMusic(float speed, Track track)
    {
        SetState(MusicSate.FadingOut);
        keepFadingIn = false;
        keepFadingOut = true;
        
        float audioVolume = track.AudioSource.volume;

        while (track.AudioSource.volume >= speed && keepFadingOut)
        {
            audioVolume -= speed;
            track.AudioSource.volume = audioVolume;
            yield return null;
        }

        track.AudioSource.Stop();
        SetState(MusicSate.Stopped);
    }
    
    private IEnumerator ChangeMusic(Sound soundToStart, float speedToChange)
    {
        yield return FadeOutMusic(speedToChange, GetTrack(soundToStart));
        
        if (PlayMusic(soundToStart))
            yield return FadeInMusic(FADE_RATE, GetTrack(soundToStart));
        
    }

    private  IEnumerator ChangeMusicHold(Sound interruptingMusic)
    {
        Track track = GetTrack(interruptingMusic);
        onHoldClipMarker = 0;

        onHoldSong = TrackList.Single(s => s.Clip.name == track.AudioSource.clip.name).ClipName;

        onHoldClipMarker = track.AudioSource.time;

        yield return FadeOutMusic(FADE_RATE, track);

        if (PlayMusic(interruptingMusic))
            yield return FadeInMusic(FADE_RATE, GetTrack(interruptingMusic));
        
    }

    private IEnumerator ChangeMusicResume()
    {
        yield return FadeOutMusic(FADE_RATE, GetTrack(onHoldSong));

        if (PlayMusic(onHoldSong, onHoldClipMarker))
            yield return FadeInMusic(FADE_RATE, GetTrack(onHoldSong));
        
    }

    private static string GetMixerGroup(SoundType type)
    {
        switch (type)
        {
            case SoundType.Music:
                return "Music";
            case SoundType.SFX:
                return "SFX";
            case SoundType.Ambient:
                return "Ambient";
            default:
                return "Music";
        }
    }

    private bool SongIsAlreadyPlaying(Sound song)
    {
        return (GetTrack(song).AudioSource.clip == GetSound(song));
    }

    private void SetState(MusicSate newState)
    {
        currentState = newState;
    }
    
}
