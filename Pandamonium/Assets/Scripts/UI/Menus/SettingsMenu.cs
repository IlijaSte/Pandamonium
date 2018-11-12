using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LevelManagment.Data;

namespace LevelManagment
{
    public class SettingsMenu : Menu<SettingsMenu>
    {
        [SerializeField]
        private Slider _masterVolumeSlider;

        [SerializeField]
        private Slider _sfxSlider;

        [SerializeField]
        private Slider _musicSlider;

        private DataManager _dataManager;

        protected override void Awake()
        {
            base.Awake();
            _dataManager = Object.FindObjectOfType<DataManager>();
        }

        private void Start()
        {
            LoadData();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            //PlayerPrefs.Save();
            if (_dataManager != null)
            {
                _dataManager.Save();
            }
        }

        public void OnMasterVolumeChanged(float volume)
        {
            if (_dataManager != null)
            {
                _dataManager.MasterVolume = volume;
            }
            Debug.Log("Master volume = " + volume);
            //PlayerPrefs.SetFloat("MasterVolume", volume); //key value pair
        }

        public void OnSFXVolumeChanged(float volume)
        {
            Debug.Log("SFX volume = " + volume);
            if (_dataManager != null)
            {
                _dataManager.SfxVolume = volume;
            }
            //PlayerPrefs.SetFloat("SFXVolume", volume); //key value pair
        }

        public void OnMusicVolumeChanged(float volume)
        {
            Debug.Log("Music volume = " + volume);
            if (_dataManager != null)
            {
                _dataManager.MusicVolume = volume;
            }
            //PlayerPrefs.SetFloat("MusicVolume", volume); //key value pair
        }


        public void LoadData()
        {
            if (_dataManager == null || _masterVolumeSlider == null || _sfxSlider == null || _musicSlider == null)
            {
                return;
            }

            _dataManager.Load();

            _masterVolumeSlider.value = _dataManager.MasterVolume;
            _sfxSlider.value = _dataManager.SfxVolume;
            _musicSlider.value = _dataManager.MusicVolume;
            //_masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
            //_sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            //_musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        }
    }
}

