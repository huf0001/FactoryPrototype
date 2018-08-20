using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectScript : IdentifiableScript
{
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
        body = this.gameObject.GetComponent<Rigidbody>();
        body.useGravity = true;
    }

    void OnMouseDown()
    {
        HandleOnMouseDown();
    }

    public virtual void HandleOnMouseDown()
    {
        body.useGravity = false;
        body.isKinematic = true;
        transform.position = guide.transform.position;
        transform.rotation = guide.transform.rotation;
        transform.parent = tempParent.transform;
        AddIdentifier("PlayerMoving");
    }

    void OnMouseUp()
    {
        HandleOnMouseUp();
    }

    public virtual void HandleOnMouseUp()
    {
        body.useGravity = true;
        body.isKinematic = false;
        transform.parent = null;
        transform.position = guide.transform.position;
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

    protected Rigidbody Body
    {
        get
        {
            return body;
        }
        set
        {
            body = value;
        }
    }
}
