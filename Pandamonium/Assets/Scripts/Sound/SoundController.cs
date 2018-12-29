using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour{

    public Sound[] sounds;
    //public AudioSource audioSource;

    private void Awake()
    {
        foreach(Sound sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = 1;
            sound.audioSource.loop = false;
            sound.audioSource.playOnAwake = false;

        }
    }

    public void PlaySoundByName(string name)
    {
        Sound sound = SearchSoundbyName(name);
        if (sound != null)
        {
            sound.audioSource.Play();
        }
    }

    public void StopSoundByName(string name)
    {
        Sound sound = SearchSoundbyName(name);
        if (sound != null)
        {
            sound.audioSource.Stop();
        }
    }

    private Sound SearchSoundbyName(string name)
    {
        Sound sound = null; 
    
        for(int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name.Equals(name))
                sound = sounds[i];
        }

        if (sound == null)
            Debug.LogError("Nije nasao zvuk");

        return sound;
    }

}
