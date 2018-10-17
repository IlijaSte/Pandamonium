using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void loadCharachterSelectionScene()
    {
        SceneManager.LoadScene("CharacterSelection");
    }

    public void loadTestScene()
    {
        SceneManager.LoadScene("TestScene");
    }
    public void loadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
