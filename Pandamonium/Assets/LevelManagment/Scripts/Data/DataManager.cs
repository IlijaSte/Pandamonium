using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelManagment.Data
{
    public class DataManager : MonoBehaviour
    {
        private SaveData _saveDataObject;
        private JsonSaver _jsonSaver;

        public float MasterVolume
        {
            get { return _saveDataObject.masterVolume; }
            set { _saveDataObject.masterVolume = value; }
        }

        public float SfxVolume
        {
            get { return _saveDataObject.sfxVolume; }
            set { _saveDataObject.sfxVolume = value; }
        }

        public float MusicVolume
        {
            get { return _saveDataObject.musicVolume; }
            set { _saveDataObject.musicVolume = value; }
        }

        public string PlayeName
        {
            get { return _saveDataObject.playerName; }
            set { _saveDataObject.playerName = value; }
        }

        private void Awake()
        {
            _saveDataObject = new SaveData();
            _jsonSaver = new JsonSaver();
        }

        public void Save()
        {
            _jsonSaver.Save(_saveDataObject);
        }

        public void Load()
        {
            _jsonSaver.Load(_saveDataObject);
        }
    } 
}
