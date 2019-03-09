using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageChanger : MonoBehaviour {


    public Sprite[] images;
    private int currentImg = 0;

    public GameObject buttonPlay;
	
    public void NextImage()
    {
        if (currentImg == images.Length - 1)
            currentImg = 0;
        else currentImg++;

        if (currentImg == images.Length - 1)
            buttonPlay.SetActive(true);
        else buttonPlay.SetActive(false);

        GetComponent<Image>().sprite = images[currentImg];


    }

}
