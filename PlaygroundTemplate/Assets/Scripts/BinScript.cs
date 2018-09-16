using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinScript : MonoBehaviour
{

    // Should be physically impossible for anything to enter the bin except via falling in, so it can't be triggered from the sides.
    // Will check that the item in question is not the player though.
    private void OnTriggerStay(Collider other)
    {
        if (true)
        {
            Destroy(other.gameObject.GetComponent<MovableScript>());
            Destroy(other.gameObject.GetComponent<AttachScript>());
            Destroy(other.gameObject);
        }
    }
}
