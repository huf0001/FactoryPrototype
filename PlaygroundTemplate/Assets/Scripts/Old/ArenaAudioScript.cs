using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaAudioScript : MonoBehaviour
{
    public AudioClip arenaMusic1;
    public AudioClip arenaMusic2;
    public AudioClip arenaMusic3;

    public AudioClip defaultGravMusic;
    public AudioClip gravMusic1;
    public AudioClip gravMusic2;
    public AudioClip gravMusic3;
    public AudioClip gravMusic4;
    public AudioClip gravMusic5;
    public AudioClip gravMusic6;
    public AudioClip gravMusic7;
    public AudioClip gravMusic8;
    public AudioClip gravMusic9;
    public AudioClip gravMusic10;
    public AudioClip gravMusic11;
    public AudioClip gravMusic12;

    private AudioSource arenaSource;
    private AudioSource gravMusicSource;

    private List<AudioClip> arenaSounds = new List<AudioClip>();
    private List<AudioClip> gravMusicSounds = new List<AudioClip>();

    public AudioClip thudSound;
    public AudioClip breakSound;
    public AudioClip hitSound;
    private AudioSource thudSource;
    private AudioSource breakSource;
    private AudioSource hitSource;

    int index = 1;

    private bool gravOn = false;
    private bool anyDestructible = false;
    private float destructiblePeriod = 0f;
    public float destructibleLimit = 3f;

    void Awake()
    {
        arenaSource = this.gameObject.AddComponent<AudioSource>();
        gravMusicSource = this.gameObject.AddComponent<AudioSource>();
        breakSource = this.gameObject.AddComponent<AudioSource>();
        thudSource = this.gameObject.AddComponent<AudioSource>();
        hitSource = this.gameObject.AddComponent<AudioSource>();


        arenaSource.loop = false;
        arenaSource.playOnAwake = true;
        gravMusicSource.loop = true;
        arenaSource.playOnAwake = false;

        AddSoundToList(arenaMusic1, arenaSounds, "arenaSound1");
        AddSoundToList(arenaMusic2, arenaSounds, "arenaSound2");
        AddSoundToList(arenaMusic3, arenaSounds, "arenaSound3");

        AddSoundToList(gravMusic1, gravMusicSounds, "gravMusicSound1");
        AddSoundToList(gravMusic2, gravMusicSounds, "gravMusicSound2");
        AddSoundToList(gravMusic3, gravMusicSounds, "gravMusicSound3");
        AddSoundToList(gravMusic4, gravMusicSounds, "gravMusicSound4");
        AddSoundToList(gravMusic5, gravMusicSounds, "gravMusicSound5");
        AddSoundToList(gravMusic6, gravMusicSounds, "gravMusicSound6");
        AddSoundToList(gravMusic7, gravMusicSounds, "gravMusicSound7");
        AddSoundToList(gravMusic8, gravMusicSounds, "gravMusicSound8");
        AddSoundToList(gravMusic9, gravMusicSounds, "gravMusicSound9");
        AddSoundToList(gravMusic10, gravMusicSounds, "gravMusicSound10");
        AddSoundToList(gravMusic11, gravMusicSounds, "gravMusicSound11");
        AddSoundToList(gravMusic12, gravMusicSounds, "gravMusicSound12");

        LoadSound(breakSound, breakSource, "breakSound");
        LoadSound(thudSound, thudSource, "thudSound");
        LoadSound(hitSound, hitSource, "hitSound");

        for (int i = 0; i < arenaSounds.Count; i++)
        {
            if (arenaSounds[i] != null)
            {
                arenaSource.clip = arenaSounds[i];
                arenaSource.Play();
                break;
            }
        }

        if (!arenaSource.isPlaying)
        {
            Debug.Log("<color=orange>" + gameObject.name + ": No background Audio is being run.</color>");
        }
    }

    private void LoadSound(AudioClip sound, AudioSource source, string name)
    {
        if (sound != null)
        {
            source.clip = sound;
            source.playOnAwake = false;
            source.loop = false;
        }
        else
        {
            Debug.Log("<color=orange>" + gameObject.name + ": Error loading " + name + ". It's value is null.</color>");
        }
    }

    private void AddSoundToList(AudioClip musicSound, List<AudioClip> musicList, string name)
    {
        if (musicSound != null)
        {
            musicList.Add(musicSound);
        }
        else
        {
            Debug.Log("<color=orange>" + gameObject.name + ": Error loading " + name + ". Audio clip assignment missing in Unity GUI.</color>");
        }
    }

    public void PingGravAudio(bool grav, bool destructible, float period)
    {
        gravOn = grav;
        anyDestructible = destructible;
        destructiblePeriod = period;
    }

    void Update()
    {
        if (!arenaSource.isPlaying)
        {
            arenaSource.clip = arenaSounds[index];
            arenaSource.Play();

            index++;

            if (index >= arenaSounds.Count)
            {
                index = 0;
            }
        }

        if (gravOn && !gravMusicSource.isPlaying)
        {
            arenaSource.volume = arenaSource.volume * 0.5f;
            gravMusicSource.clip = SelectGravAudio();
            gravMusicSource.Play();
        }

        if (!gravOn && gravMusicSource.isPlaying)
        {
            if ((!anyDestructible) || (destructiblePeriod > destructibleLimit))
            {
                gravMusicSource.Stop();
                arenaSource.volume = arenaSource.volume * 2f;
            }
            else 
            {
                destructiblePeriod += Time.deltaTime;
            }
        }
    }

    private AudioClip SelectGravAudio()
    {
        switch (gravMusicSounds.Count)
        {
            case 0:
                return defaultGravMusic;
            case 1:
                return gravMusicSounds[0];
            default:
                return gravMusicSounds[Random.Range(0, gravMusicSounds.Count)];
        }
    }

    public void PlayCollisionSFX(string name)
    {
        switch (name)
        {
            case "thud":
                thudSource.Play();
                break;
            case "break":
                breakSource.Play();
                break;
            case "hit":
                hitSource.Play();
                break;
        }
    }
}
