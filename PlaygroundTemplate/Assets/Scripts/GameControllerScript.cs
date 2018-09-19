using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScript : MonoBehaviour
{
    [SerializeField] private BuildZoneScript buildZone = null;
    [SerializeField] private GameObject playerLeftHand = null;
    [SerializeField] private GameObject playerRightHand = null;

    // Use this for initialization
    void Start()
    {
        if (playerLeftHand == null)
        {
            Debug.Log("The game controller is missing the player's left hand");
        }

        if (playerRightHand == null)
        {
            Debug.Log("The game controller is missing the player's right hand");
        }

        if (buildZone == null)
        {
            Debug.Log("The game controller is missing the build zone");
        }
    }

    public GameObject LeftHand
    {
        get
        {
            return playerLeftHand;
        }
    }

    public GameObject RightHand
    {
        get
        {
            return playerRightHand;
        }
    }

    public BuildZoneScript BuildZone
    {
        get
        {
            return buildZone;
        }
    }
}