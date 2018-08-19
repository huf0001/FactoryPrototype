using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachableScript: MonoBehaviour
{
    private MoveObjectScript movable;
    
    // Use this for initialization
	void Start()
    {
        movable = this.gameObject.GetComponent<MoveObjectScript>();
        movable.AddIdentifier("Attachable");
    }

    //If this is attached to something, this passes the colliding object to the attached base object
    //to see if the colliding object can also be attached
    private void OnTriggerStay(Collider other)
    {
        if (movable.HasIdentifier("Attached"))
        {
            transform.GetComponentInParent<AttachScript>().CheckCollisionTrigger(other);
        }
    }

    //These two methods I wouldn't include if I could avoid it; having AttachableScript.cs inherit from MoveObjectScript.cs
    //would be more elegant. However, for whatever reason, doing it that way renders the objects with this script unmovable
    //due to the inherited MoveObjectScript.cs being unable to find the rigidbody of the game object this script is attached
    //to for whatever reason.
    //
    //These methods just let AttachScript.cs go through this script to change identifiers, so that it can't do that to
    //movable objects that aren't also attachable.
    public void AddIdentifier(string id)
    {
        movable.AddIdentifier(id);
    }

    public void RemoveIdentifier(string id)
    {
        movable.RemoveIdentifier(id);
    }
}
