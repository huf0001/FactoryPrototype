using UnityEngine;
using System.Collections;

public class MenuAudioScript : MonoBehaviour
{
    public AudioClip menuSound;
    private AudioSource menuSource;

    void Awake()
    {
        menuSource = this.gameObject.AddComponent<AudioSource>();

        menuSource.loop = true;
        menuSource.playOnAwake = true;

        if (menuSound != null)
        {
            menuSource.clip = menuSound;
            menuSource.Play();
        }
    }
}