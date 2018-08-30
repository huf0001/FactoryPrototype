using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBeltScript : MonoBehaviour
{
    // public GameObject belt;
    public Transform endpoint;
    public float speed;

    private void OnTriggerStay(Collider other)
    {
        if (!CheckPlayerMoving(other.gameObject) && !CheckPlayerAvatar(other.gameObject))
        {
            other.transform.position = Vector3.MoveTowards(other.transform.position, endpoint.position, speed * Time.deltaTime);
        }

        //Note: attachment base objects seem to move extra fast;  might need to add a secondary endpoint object
        //for that object to move towards so that it gets kicked off the conveyor belt and doesn't knock other objects off 
    }

    private bool CheckPlayerMoving(GameObject other)
    {
        bool result = false;
        MovableScript movable = other.gameObject.GetComponent<MovableScript>();

        if (movable != null)
        {
            if ((movable.HasIdentifier(Identifier.PlayerMoving))||(movable.HasIdentifier(Identifier.Attached)))
            {
                result = true;
            }     
        }

        return result;
    }

    private bool CheckPlayerAvatar(GameObject other)
    {
        bool result = false;
        if (other.tag == "PlayerAvatar")
        {
            result = true;
        }

        return result;
    }
}
