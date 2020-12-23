using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class MusicController : Singleton<MusicController>
{
    public enum SoundEffects
    {
        MainMenu,
        BGM,
        Hit,
        Walk,
        Jump,
        Click, 
        Hover,
        Victory,
    }

    private Dictionary<SoundEffects, AudioClip> sounds;
    private const string FileExt = "";
    private readonly float backgroundVolume = .8f;
    private float effectsVolume = .8f;
    public AudioSource BGMaudioSource;
    private AudioSource SFXAudioSource;
    

    // Start is called before the first frame update
    protected override void Awake()
    {
        BGMaudioSource = gameObject.AddComponent<AudioSource>();
        SFXAudioSource = gameObject.AddComponent<AudioSource>();
        
        BGMaudioSource.name = "Background Music";
        BGMaudioSource.loop = true;
        BGMaudioSource.volume = backgroundVolume;
        SFXAudioSource.volume = effectsVolume;
        
        sounds = new Dictionary<SoundEffects, AudioClip>();
        LoadSoundClips();
        base.Awake();
    }

    private void LoadSoundClips()
    {
        foreach (SoundEffects sound in Enum.GetValues(typeof(SoundEffects)))
        {
            string soundClipName = Enum.GetName(typeof(SoundEffects), sound);
            var audioClip = Resources.Load<AudioClip>(soundClipName + FileExt);
            sounds[sound] = audioClip;
        }
    }
    
    public void PlaySound(SoundEffects soundEffects, float volume = 1)
    {
        var soundToPlay = sounds[soundEffects];
        SFXAudioSource.PlayOneShot(soundToPlay, volume);
    }

    public void PlaySound(string soundName, float volume = 1)
    {
        if (Enum.TryParse(soundName, true, out SoundEffects sound))
        {
            PlaySound(sound, volume);
        }
        else
        {
            Debug.LogWarning("The sound " + soundName + " was not found!");
        }

    }

    public void ChangeSoundEffectsVolume(Slider slider)
    {
        SFXAudioSource.volume = slider.value;
    }

    public void PlayGameBGM()
    {
        var toPlay = sounds[SoundEffects.BGM];
        if (BGMaudioSource.clip == toPlay) return;
        BGMaudioSource.clip = toPlay;
        BGMaudioSource.Play();
    }

    public void PlayMenuBGM()
    {
        var toPlay = sounds[SoundEffects.MainMenu];
        if (BGMaudioSource.clip == toPlay) return;
        BGMaudioSource.clip = toPlay;
        BGMaudioSource.Play();
    }

    public float GetSFXVolume()
    {
        return SFXAudioSource.volume;
    }

    public float GetBGMVolume()
    {
        return BGMaudioSource.volume;
    }
    
}