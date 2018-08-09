using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using System.Collections.Generic;

public class MenuScript : MonoBehaviour 
{
    public Text placeholder;
    public Text textDisplay;

    public void LoadScene(string sceneName)
	{
        if (textDisplay.text == "")
        {
            placeholder.color = new Color(255, 0, 0, 255);
        }
        else
        {
            PlayerPrefs.SetString("name", textDisplay.text);

            if (sceneName == null)
            {
                Debug.Log("<color=orange>" + gameObject.name + ": No Scene Name Was given for LoadScene function!</color>");
            }
            SceneManager.LoadScene(sceneName); //load a scene
        }
    }
}
