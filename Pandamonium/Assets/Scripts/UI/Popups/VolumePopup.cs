using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumePopup : Popup {

    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider sfxSlider;

    [SerializeField]
    private Slider musicSlider;

    public void OnMasterVolumeChanged(float volume)
    {
        /*if (_dataManager != null)
        {
            _dataManager.MasterVolume = volume;
        }*/
        Debug.Log("Master volume = " + volume);
        //PlayerPrefs.SetFloat("MasterVolume", volume); //key value pair
    }

    public void OnSFXVolumeChanged(float volume)
    {
        /*
        Debug.Log("SFX volume = " + volume);
        if (_dataManager != null)
        {
            _dataManager.SfxVolume = volume;
        }*/
        //PlayerPrefs.SetFloat("SFXVolume", volume); //key value pair
    }

    public void OnMusicVolumeChanged(float volume)
    {
        /*
        Debug.Log("Music volume = " + volume);
        if (_dataManager != null)
        {
            _dataManager.MusicVolume = volume;
        }*/
        //PlayerPrefs.SetFloat("MusicVolume", volume); //key value pair
    }
}
