using UnityEngine;
using System.Collections;

public class GameOverAudioScript : MonoBehaviour
{
    public AudioClip standardAudio;
    public AudioClip completionAudio;
    public AudioClip topScoreAudio;
    private AudioSource gameOverSource;

    private int playerProgress = 0;

    private bool audioStarted = false;

    void Awake()
    {
        gameOverSource = this.gameObject.AddComponent<AudioSource>();

        gameOverSource.loop = true;
        gameOverSource.playOnAwake = false;
    }

    void Update()
    {
        if (audioStarted)
        {
            if (!gameOverSource.isPlaying)
            {
                if (playerProgress == 100)
                {
                    gameOverSource.clip = completionAudio;
                }
                else
                {
                    gameOverSource.clip = standardAudio;
                }

                gameOverSource.loop = true;
                gameOverSource.Play();
            }
        }
        else
        {
            if (PlayerPrefs.GetInt("saved") == 1)
            {
                int position = PlayerPrefs.GetInt("position");
                playerProgress = PlayerPrefs.GetInt("progress");

                if (position == 1)
                {
                    gameOverSource.clip = topScoreAudio;
                    gameOverSource.loop = false;
                }
                else if (playerProgress == 100)
                {
                    gameOverSource.clip = completionAudio;
                }
                else
                {
                    gameOverSource.clip = standardAudio;
                }

                gameOverSource.Play();
                audioStarted = true;
            }
        }
    }
}