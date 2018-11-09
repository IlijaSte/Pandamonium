using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {

    private Stack<GameObject> activeMenus = new Stack<GameObject>();

    private static MenuManager instance;

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
}
