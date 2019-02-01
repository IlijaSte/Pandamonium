using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundController : MonoBehaviour {

    public Sound[] sounds;
    //public AudioSource audioSource;

    private void Awake()
    {
        foreach (Sound sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;
            sound.audioSource.volume = 1;
            sound.audioSource.loop = false;
            sound.audioSource.playOnAwake = false;

        }
    }

    private void Start()
    {
        if (!PlaySoundByNameOnLoop("Ambient"))
            Debug.LogWarning("nema ambijent");

    }

    public bool PlaySoundByNameOnLoop(string name)
    {
        Sound sound = SearchSoundbyName(name);
        if (sound != null)
        {
            sound.audioSource.loop = true;
            sound.audioSource.Play();
            return true;
        }
        else return false;
    }

    public bool StopSoundByNameOnLoop(string name)
    {
        Sound sound = SearchSoundbyName(name);
        if (sound != null)
        {
            sound.audioSource.loop = false;
            sound.audioSource.Stop();
            return true;
        } else return false;
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
            Debug.LogWarning("Nije nasao zvuk");

        return sound;

    }

}
