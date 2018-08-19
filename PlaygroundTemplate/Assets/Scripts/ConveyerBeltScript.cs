using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBeltScript : MonoBehaviour
{
    public GameObject belt;
    public Transform endpoint;
    public float speed;

    private void OnTriggerStay(Collider other)
    {
        if (!CheckPlayerMoving(other.gameObject))
        {
            other.transform.position = Vector3.MoveTowards(other.transform.position, endpoint.position, speed * Time.deltaTime);
        }
    }

    private bool CheckPlayerMoving(GameObject other)
    {
        bool result = false;
        MoveObjectScript movable = other.gameObject.GetComponent<MoveObjectScript>();

        if (movable != null)
        {
            if ((movable.HasIdentifier("PlayerMoving"))||(movable.HasIdentifier("Attached")))
            {
                result = true;
            }     
        }

        return result;
    }
}
