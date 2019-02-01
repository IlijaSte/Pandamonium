﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PopupManager : MonoBehaviour {

    private Stack<GameObject> activeMenus = new Stack<GameObject>();

    private static PopupManager instance;

    public GameObject pauseMenu;
    public GameObject deathMenu;
    public GameObject gameEndMenu;

    public static PopupManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PopupManager>();
            }
            return instance;
        }
    }

    public void ShowMenu(GameObject menu)
    {

        activeMenus.Push(Instantiate(menu));

    }

    public void CloseMenu()
    {
        Destroy(activeMenus.Pop());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // MENJATI

            if (SceneManager.GetActiveScene().name.StartsWith("Level"))
            {
                if (activeMenus.Count == 0)
                    ShowMenu(pauseMenu);
                else CloseMenu();

            }
        }
    }
}