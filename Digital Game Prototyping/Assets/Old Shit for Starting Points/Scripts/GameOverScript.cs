using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System;

public class GameOverScript : MonoBehaviour 
{
    public Text endText;
    public Text namesParent;
    public Text progressesParent;
    public Text timesParent;

    private string playerName;
    private int playerProgress;
    private int playerTime;

    private Dictionary<int, string> names = new Dictionary<int, string>();
    private Dictionary<int, int> progresses = new Dictionary<int, int>();
    private Dictionary<int, int> times = new Dictionary<int, int>();
    private Text[] nameDisplays;
    private Text[] progressDisplays;
    private Text[] timeDisplays;

    private string filename = "Scoreboard/Scoreboard.txt";
    private TimeConverter converter = new TimeConverter();

    public GameObject avatar;
    public Material asteroidMaterial;
    public Material earthMaterial;
    
    void Awake()
	{
        //Debug.Log("<color=orange>" + gameObject.name + ": Running GameOverScript.</color>");

        Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true; // make the cursor useable again.

        LoadScoreboard();
        SaveScoreboard();

        UpdatePlayerAvatar();
    }

    private void LoadScoreboard()
    {
        LoadScoreboardObjects();
        ReadScoreboardData();

        //For some reason, GameOverScript.cs seems to be running twice when you play through, despite running normally
        //when you load it straight from the GameOver scene without playing through the arena. I have no idea how to 
        //stop it running twice, as I have no idea how it is running twice. It's only attached to the camera object,
        //and only once. This if statement shouldstop the score the player gets from being loaded into the scoreboard 
        //twice at least, so players don't notice this weirdness behind the scenes. Don't put this if statement around
        //LoadScoreboard() and SaveScoreboard() in Awake(), as that just results in nothing being printed at all,
        //and I'm guessing that GameOverScript.cs at least resets the Text objects' .text fields when it loads the
        //second time.
        if (PlayerPrefs.GetInt("saved") == 0)
        {
            UpdateScoreboardData();
        }
        PrintScoreboardData();
    }

    private void LoadScoreboardObjects()
    {
        nameDisplays = namesParent.GetComponentsInChildren<Text>();
        progressDisplays = progressesParent.GetComponentsInChildren<Text>();
        timeDisplays = timesParent.GetComponentsInChildren<Text>();
    }

    private void ReadScoreboardData()
    {
        StreamReader reader = new StreamReader(filename);
        int n = 0;

        try
        {
            if (Int32.TryParse(reader.ReadLine(), out n))
            {
                for (int i = 0; i < 8; i++)
                {
                    names.Add(i + 1, reader.ReadLine());
                    progresses.Add(i + 1, Convert.ToInt32(reader.ReadLine()));
                    times.Add(i + 1, Convert.ToInt32(reader.ReadLine()));
                }
            }
            else
            {
                Debug.Log("<color=orange>" + gameObject.name + ": Error reading " + filename + ".</color>");
            }
        }
        finally
        {
            reader.Close();
        }
    }

    private void UpdateScoreboardData()
    {
        //Debug.Log("<color=orange>" + gameObject.name + ": Running UpdateScoreboardData().</color>");
        playerName = PlayerPrefs.GetString("name");
        playerProgress = PlayerPrefs.GetInt("progress");
        playerTime = PlayerPrefs.GetInt("time");

        int position = -1;

        if (playerName != "")
        {
            foreach (KeyValuePair<int, int> p in progresses)
            {
                if (playerProgress > p.Value)
                {
                    for (int i = progresses.Count; i > p.Key; i--)
                    {
                        names[i] = names[i - 1];
                        progresses[i] = progresses[i - 1];
                        times[i] = times[i - 1];
                    }

                    names[p.Key] = playerName;
                    progresses[p.Key] = playerProgress;
                    times[p.Key] = playerTime;
                    position = p.Key;
                    break;
                }
                else if (playerProgress == p.Value)
                {
                    if (playerTime <= times[p.Key])
                    {
                        for (int i = progresses.Count; i > p.Key; i--)
                        {
                            names[i] = names[i - 1];
                            progresses[i] = progresses[i - 1];
                            times[i] = times[i - 1];
                        }

                        names[p.Key] = playerName;
                        progresses[p.Key] = playerProgress;
                        times[p.Key] = playerTime;
                        position = p.Key;
                        break;
                    }
                }
            }
        }

        PlayerPrefs.SetInt("position", position);
        PlayerPrefs.SetInt("saved", 1);
    }

    private void PrintScoreboardData()
    {
        foreach (KeyValuePair<int, string> p in names)
        {
            nameDisplays[p.Key].text = p.Value;
        }

        foreach (KeyValuePair<int, int> p in progresses)
        {
            progressDisplays[p.Key].text = "" + p.Value + "%";
        }

        foreach (KeyValuePair<int, int> p in times)
        {
            timeDisplays[p.Key].text = "" + converter.SecondsToDigitalDisplay(p.Value);
        }

        endText.text = "GAME OVER! " + converter.SecondsToDigitalDisplay(PlayerPrefs.GetInt("time")) + " - " + PlayerPrefs.GetInt("progress") + "%";
    }

    private void SaveScoreboard()
    {
        StreamWriter writer = new StreamWriter(filename);

        try
        {
            writer.WriteLine("1");

            foreach (KeyValuePair<int, string> p in names)
            {
                writer.WriteLine(p.Value);
                writer.WriteLine(progresses[p.Key]);
                writer.WriteLine(times[p.Key]);
            }
        }
        finally
        {
            writer.Close();
        }
    }

    private void UpdatePlayerAvatar()
    {
        MeshRenderer mesh = avatar.GetComponent<MeshRenderer>();
        int progress = PlayerPrefs.GetInt("progress");

        if (progress == 100)
        {
            mesh.material = earthMaterial;
        }
        else 
        {
            mesh.material = asteroidMaterial;
        }
    }

	public void StartAgain(string levelName)
	{
        if (levelName == null)
        {
            Debug.Log("<color=orange>" + gameObject.name + ": No Scene Name Was given for the StartAgain function!</color>");
        }
        SceneManager.LoadScene(levelName);
	}

    public void QuitGame()
    {
        Application.Quit();
    }
}
