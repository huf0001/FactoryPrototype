using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectScript : IdentifiableScript
{
    [SerializeField] private GameObject tempLeftParent;
    [SerializeField] private GameObject tempRightParent;
    private Transform leftGuide;
    private Transform rightGuide;
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
        leftGuide = tempLeftParent.transform;
        rightGuide = tempRightParent.transform;
    }

    void OnMouseDown()
    {
        HandleOnMouseDown();
    }

    public virtual void HandleOnMouseDown()
    {
        body.useGravity = false;
        body.isKinematic = true;
        transform.position = leftGuide.transform.position;
        transform.rotation = leftGuide.transform.rotation;
        transform.parent = tempLeftParent.transform;
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
        transform.position = leftGuide.transform.position;
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
