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
        Crickets,
        Click,
        EndGame,
        Score
    }

    private Dictionary<SoundEffects, AudioClip> sounds;
    private const string FileExt = "";
    private readonly float backgroundVolume = .05f;
    private float effectsVolume = .25f;
    [SerializeField] private float cricketsVolume = .2f;
    private AudioSource BGMaudioSource;
    private AudioSource cricketsAudioSource;
    private AudioSource SFXAudioSource;


    // Start is called before the first frame update
    protected override void Awake()
    {
        BGMaudioSource = gameObject.AddComponent<AudioSource>();
        SFXAudioSource = gameObject.AddComponent<AudioSource>();
        cricketsAudioSource = gameObject.AddComponent<AudioSource>();
        BGMaudioSource.loop = true;
        cricketsAudioSource.loop = true;
        BGMaudioSource.volume = backgroundVolume;
        SFXAudioSource.volume = effectsVolume;
        cricketsAudioSource.volume = cricketsVolume;
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

    public void PlaySound(string soundName)
    {
        if (Enum.TryParse(soundName, true, out SoundEffects sound))
        {
            PlaySound(sound, 1f);
        }
        else
        {
            Debug.LogWarning("The sound " + soundName + " was not found!");
        }
    }

    public void PlayGameBGM()
    {
        var toPlay = sounds[SoundEffects.BGM];
        if (BGMaudioSource.clip != null && BGMaudioSource.clip == toPlay) return;
        BGMaudioSource.clip = toPlay;
        cricketsAudioSource.clip = sounds[SoundEffects.Crickets];
        BGMaudioSource.Play();
        cricketsAudioSource.Play();
    }

    public void PlayMenuBGM()
    {
        var toPlay = sounds[SoundEffects.BGM];
        if (BGMaudioSource.clip != null && BGMaudioSource.clip == toPlay) return;
        BGMaudioSource.clip = toPlay;
        BGMaudioSource.Play();
    }

    public void SetBGMVolume(Slider slider)
    {
        BGMaudioSource.volume = slider.value;
    }

    public void SetSFXVolume(Slider slider)
    {
        SFXAudioSource.volume = slider.value;
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