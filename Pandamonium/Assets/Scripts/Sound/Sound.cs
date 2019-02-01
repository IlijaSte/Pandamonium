using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{ 

    public AudioClip audioClip;

    [HideInInspector]
    public AudioSource audioSource;

    public string name;

    public float volume;


}
