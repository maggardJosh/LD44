using System;
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

    public enum BattleState
    {
        PLAYING_SCENE,
        FADING_OUT_SCENE,
        FADING_IN_BATTLE,
        PLAYING_BATTLE,
        FADING_OUT_BATTLE,
        FADING_IN_SCENE
    }
    #endregion

    public BattleState currentState;

    public List<Track> TrackList;

    private static bool keepFadingIn;
    private static bool keepFadingOut;

    private static bool isMuted = false;

    private  float onHoldClipMarker;
    private  Sound onHoldSong;

    private MusicSate depricatedMachineState = MusicSate.Playing;

    private const float FADE_RATE = .05f;
    private const float BATTLE_FADE_RATE = 1;

    void Update()
    {
        //Handle ambiance fading here
        HandleBattleState();
    }

    private void HandleBattleState()
    {
        switch (currentState)
        {
            case BattleState.PLAYING_SCENE:
                PlayingScene();
                break;

            case BattleState.FADING_OUT_SCENE:
                
                FadeOut(GetSceneTrack().ClipName);

                break;

            case BattleState.FADING_IN_BATTLE:
                
                FadeIn(Sound.Music_Battle);

                break;

            case BattleState.PLAYING_BATTLE:
                PlayingBattle();
                break;

            case BattleState.FADING_OUT_BATTLE:

                FadeOut(Sound.Music_Battle);

                break;

            case BattleState.FADING_IN_SCENE:

                FadeIn(GetSceneTrack().ClipName);

                break;
        }
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

    public void PlayAmbient(Sound soundName)
    {
        if (SongIsAlreadyPlaying(soundName))
            return;

        Track track = GetTrack(soundName);
        track.AudioSource.clip = track.Clip;
        track.AudioSource.Play();
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
        if (depricatedMachineState == MusicSate.FadingIn || depricatedMachineState == MusicSate.FadingOut)
            return;
        if (SongIsAlreadyPlaying(soundToStart))
            return;
        StartCoroutine(ChangeMusic(soundToStart, FADE_RATE));
        currentState = BattleState.PLAYING_SCENE;
    }
    
    private AudioClip GetSound(Sound soundName)
    {
        return GetTrack(soundName).Clip;
    }

    private Track GetTrack(Sound soundName)
    {
        return TrackList.Single(s => s.ClipName == soundName);
    }

    private void PlayingBattle()
    {
    }

    private void PlayingScene()
    {
        onHoldClipMarker = 0;
    }

    //Will only be used during FADING_IN_BATTLE and FADING_IN_SCENE
    private void FadeIn(Sound sound)
    {
        Track track = GetTrack(sound);
        float currentVolume = track.AudioSource.volume;

        if (currentVolume == 0)
        {
            if (sound == Sound.Music_Battle)
                onHoldClipMarker = UnityEngine.Random.Range(0, GetSound(sound).length);
            PlayMusic(sound, onHoldClipMarker);
        }

        Mathf.Clamp((currentVolume += Time.deltaTime * BATTLE_FADE_RATE), 0f, 1f);

        track.AudioSource.volume = currentVolume;

        if (currentVolume < 1)
            return;

        switch (currentState)
        {
            case BattleState.FADING_IN_BATTLE:
                currentState = BattleState.PLAYING_BATTLE;
                break;
            case BattleState.FADING_IN_SCENE:
                currentState = BattleState.PLAYING_SCENE;
                break;
        }
        
    }

    //Will only be used during FADING_OUT_BATTLE and FADING_OUT_SCENE
    private void FadeOut(Sound sound)
    {
        Track track = GetTrack(sound);
        float currentVolume = track.AudioSource.volume;
        Mathf.Clamp((currentVolume -= Time.deltaTime * BATTLE_FADE_RATE), 0f, 1f);

        track.AudioSource.volume = currentVolume;

        if (currentVolume > 0)
            return;

        switch (currentState)
        {
            case BattleState.FADING_OUT_BATTLE:
                currentState = BattleState.FADING_IN_SCENE;
                break;
            case BattleState.FADING_OUT_SCENE:
                currentState = BattleState.FADING_IN_BATTLE;
                onHoldClipMarker = track.AudioSource.time;
                break;
        }
        
        track.AudioSource.Stop();
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

    public void SceneMusicPutOnHold()
    {
        //jump out if we're in a state that satisfies this trigger
        if (currentState == BattleState.FADING_OUT_SCENE || currentState == BattleState.FADING_IN_BATTLE || currentState == BattleState.PLAYING_BATTLE)
            return;

        switch (currentState)
        {
            case BattleState.FADING_OUT_BATTLE:
                currentState = BattleState.FADING_IN_BATTLE;
                break;
            case BattleState.FADING_IN_SCENE:
            case BattleState.PLAYING_SCENE:
                currentState = BattleState.FADING_OUT_SCENE;
                break;
        }
    }
    
    public void SceneMusicResume()
    {
        if (currentState == BattleState.FADING_OUT_BATTLE || currentState == BattleState.FADING_IN_SCENE || currentState == BattleState.PLAYING_SCENE)
            return;

        switch (currentState)
        {
            case BattleState.FADING_OUT_SCENE:
                currentState = BattleState.FADING_IN_SCENE;
                break;
            case BattleState.FADING_IN_BATTLE:
            case BattleState.PLAYING_BATTLE:
                currentState = BattleState.FADING_OUT_BATTLE;
                break;
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
            case SoundType.Ambient:
                return "Ambient";
            default:
                return "Music";
        }
    }

    private Track GetSceneTrack()
    {
        SceneMusicSettings settings = (SceneMusicSettings)FindObjectOfType(typeof(SceneMusicSettings));
        return GetTrack(settings.SceneSong);
    }

    private Track GetSceneAmbiance()
    {
        SceneMusicSettings settings = (SceneMusicSettings)FindObjectOfType(typeof(SceneMusicSettings));
        return GetTrack(settings.SceneAmbience);
    }

    private bool SongIsAlreadyPlaying(Sound song)
    {
        return (GetTrack(song).AudioSource.clip == GetSound(song));
    }

    private void SetState(MusicSate newState)
    {
        depricatedMachineState = newState;
    }

}
