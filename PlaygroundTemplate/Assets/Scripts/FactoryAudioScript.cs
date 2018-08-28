using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactoryAudioScript : MonoBehaviour
{
    [SerializeField] private AudioClip factoryMusic1;

    private AudioSource factorySource;

    private List<AudioClip> factorySounds = new List<AudioClip>();

    /*
        [SerializeField] private AudioClip sfxSound;

        private AudioSource sfxSource;

        // Include as many sfx sounds and sources above as necessary.
    */

    private int index = 0;

    void Awake()
    {
        /*
            sfxSource = this.gameObject.AddComponent<AudioSource>();
        
            LoadSFX(sfxSound, sfxSource, "sfxSound");
        */

        factorySource = this.gameObject.AddComponent<AudioSource>();
        factorySource.loop = false;
        factorySource.playOnAwake = true;

        AddSoundToList(factoryMusic1, factorySounds, "factorySound1");

        for ( int i = 0; i < factorySounds.Count; i++ )
        {
            if ( factorySounds[i] != null )
            {
                factorySource.clip = factorySounds[i];
                factorySource.Play();
                break;
            }
        }

        if ( !factorySource.isPlaying )
        {
            Debug.Log("<color=orange>" + gameObject.name + ": No background Audio is being run.</color>");
        }
    }

    /*private void LoadSFX (AudioClip sound, AudioSource source, string name)
    {
        if ( sound != null )
        {
            source.clip = sound;
            source.playOnAwake = false;
            source.loop = false;
        }
        else
        {
            Debug.Log("<color=orange>" + gameObject.name + ": Error loading " + name + ". It's value is null.</color>");
        }
    }*/

    private void AddSoundToList(AudioClip musicSound, List<AudioClip> musicList, string name)
    {
        if ( musicSound != null )
        {
            musicList.Add(musicSound);
        }
        else
        {
            Debug.Log("<color=orange>" + gameObject.name + ": Error loading " + name + ". Audio clip assignment missing in Unity GUI.</color>");
        }
    }

    void Update()
    {
        if ( !factorySource.isPlaying )
        {
            index++;

            if ( index >= factorySounds.Count )
            {
                index = 0;
            }

            factorySource.clip = factorySounds[index];
            factorySource.Play();
        }
    }

    /*public void PlaySFX(string name)
    {
        switch (name)
        {
            case "sfx":
                sfxSource.Play();
                break;
        }
    }*/
}