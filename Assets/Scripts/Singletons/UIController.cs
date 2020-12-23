using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : Singleton<UIController>
{
    public Slider BGMSlider, SFXSlider;

    protected override void Awake()
    {
        BGMSlider.value = MusicController.Instance.GetBGMVolume();
        SFXSlider.value = MusicController.Instance.GetSFXVolume();
        base.Awake();
    }

    private void OnEnable()
    {
        BGMSlider.value = MusicController.Instance.GetBGMVolume();
        SFXSlider.value = MusicController.Instance.GetSFXVolume();
    }
    
}
