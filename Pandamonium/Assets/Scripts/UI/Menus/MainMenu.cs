using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LevelManagment.Data;
using UnityEngine.UI;

namespace LevelManagment
{
    // inheritance from generic class Menu<T> making each instance singleton
    // inheritance methods: Awake() and OnDestory()
    public class MainMenu : Menu<MainMenu>
    {
        [SerializeField]
        private float _playDelay = 0.5f;

        [SerializeField]
        private TransitionFader startTransitionPrefab;

        private DataManager _dataManager;

        [SerializeField]
        private InputField _playerNameInput;

        protected override void Awake()
        {
            base.Awake();
            _dataManager = FindObjectOfType<DataManager>();
        }

        private void Start()
        {
            LoadData();
        }

        public void OnPlayPressed()
        {

            //if (GameManager.Instance != null)
            //{
            //    GameManager.Instance.LoadNextLevel();
            //}

            //if (MenuManager.Instance != null && GameMenu.Instance != null)
            //{
            //    MenuManager.Instance.OpenMenu(GameMenu.Instance);
            //}
            StartCoroutine(OnPlayPressRoutine());
        }

        private IEnumerator OnPlayPressRoutine()
        {
            TransitionFader.PlayTransition(startTransitionPrefab);
            LevelLoader.LoadNextLevel();
            yield return new WaitForSeconds(_playDelay);
            GameMenu.Open();
        }

        public void OnSettingsPressed()
        {
            //if (MenuManager.Instance != null && SettingsMenu.Instance != null)
            //{
            //    MenuManager.Instance.OpenMenu(SettingsMenu.Instance);
            //}
            SettingsMenu.Open();
        }

        public void OnCreditsPressed()
        {
            //if (MenuManager.Instance != null && CreditsMenu.Instance != null)
            //{
            //    MenuManager.Instance.OpenMenu(CreditsMenu.Instance);
            //}
            CreditsMenu.Open();
        }

        // overriding the back method with application quit
        public override void OnBackPressed()
        {
            //base.OnBackPressed();
            Application.Quit();
        }

        public void OnValueChanged(string name)
        {
            if (_dataManager != null)
            {
                Debug.Log(name);
                //_dataManager.PlayeName = _playerNameInput.text; 
                _dataManager.PlayeName = name; // this will work only with dynamic parameters selcted in UNity editor
            }
        }

        public void OnEndEdit()
        {
            Debug.Log("editing ended");
            _dataManager.Save();
        }

        private void LoadData()
        {
            if (_dataManager != null && _playerNameInput != null)
            {
                _dataManager.Load();
                _playerNameInput.text = _dataManager.PlayeName;
            }
        }
    }
}

