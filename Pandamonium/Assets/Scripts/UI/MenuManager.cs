using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    private Stack<GameObject> activeMenus = new Stack<GameObject>();

    private static MenuManager instance;

    public GameObject pauseMenu;
    public GameObject deathMenu;

    public static MenuManager I
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MenuManager>();
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

            if (SceneManager.GetActiveScene().name.Equals("TestScene"))
            {
                if (activeMenus.Count == 0)
                    ShowMenu(pauseMenu);
                else CloseMenu();

            }else if (SceneManager.GetActiveScene().name.Equals("CharacterSelection"))
            {
                SceneManager.LoadScene("MainMenu");
            }
            else if (SceneManager.GetActiveScene().name.Equals("MainMenu"))
            {
                if (activeMenus.Count == 0)
                    Application.Quit();
                else CloseMenu();
            }
        }
    }
}
