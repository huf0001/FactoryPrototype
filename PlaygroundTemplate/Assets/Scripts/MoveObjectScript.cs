using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectScript : IdentifiableScript
{
    private GameObject item;
    public GameObject tempParent;
    public Transform guide;
    private Rigidbody body;

    // Use this for initialization
    void Start ()
    {
        HandleStart();   
    }

    protected virtual void HandleStart()
    {
        item = this.gameObject;
        body = item.GetComponent<Rigidbody>();
        body.useGravity = true;
    }

    void OnMouseDown()
    {
        HandleOnMouseDown();
    }

    protected virtual void HandleOnMouseDown()
    {
        /*if (HasIdentifier("Attached"))
        {
            MoveObjectScript movable = this.transform.parent.GetComponent<MoveObjectScript>();

            if (movable != null)
            {
                movable.OnMouseDown();
            }
        }
        else
        {*/
            body.useGravity = false;
            body.isKinematic = true;
            item.transform.position = guide.transform.position;
            item.transform.rotation = guide.transform.rotation;
            item.transform.parent = tempParent.transform;
            AddIdentifier("PlayerMoving");
        //}
    }

    void OnMouseUp()
    {
        HandleOnMouseUp();
    }

    protected virtual void HandleOnMouseUp()
    {
        body.useGravity = true;
        body.isKinematic = false;
        item.transform.parent = null;
        item.transform.position = guide.transform.position;
        RemoveIdentifier("PlayerMoving");
        AddIdentifier("Dropped");
    }

    void OnTriggerStay(Collider other)
    {
        HandleOnTriggerStay(other);
    }

    protected virtual void HandleOnTriggerStay(Collider other)
    {
        if (HasIdentifier("Dropped"))
        {
            RemoveIdentifier("Dropped");
        }
    }
}
