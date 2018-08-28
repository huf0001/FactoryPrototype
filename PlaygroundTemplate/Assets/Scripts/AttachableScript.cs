﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachableScript : MoveObjectScript
{
    public string attachableIdentifier;
    private AttachScript attachedTo;

    // Use this for initialization
    void Start()
    {
        HandleStart();
    }

    protected override void HandleStart()
    {
        base.HandleStart();
        AddIdentifier("Attachable");
    }

    //If this is attached to something, this passes the colliding object to the attached base object
    //to see if the colliding object can also be attached
    private void OnTriggerStay(Collider other)
    {
        HandleOnTriggerStay(other);
    }

    protected override void HandleOnTriggerStay(Collider other)
    {
        if (HasIdentifier("Dropped"))
        {
            HandleDropped(other.gameObject);
        }
        else if (HasIdentifier("Attached"))
        {
            HandleCollisionWhileAttached(other);
        }
    }

    protected virtual void HandleDropped(GameObject other)
    {
        IdentifiableScript identifiable = other.GetComponent<IdentifiableScript>();
        bool fallenOnAttachBase = false;

        if (identifiable != null)
        {
            if ((identifiable.HasIdentifier(attachableIdentifier)) || (identifiable.HasIdentifier("Attached")))
            {
                fallenOnAttachBase = true;
            }
        }

        if (!fallenOnAttachBase)
        {
            RemoveIdentifier("Dropped");
        }
    }

    protected virtual void HandleCollisionWhileAttached(Collider other)
    {
        if (other.gameObject.GetComponent<IdentifiableScript>() != null)
        {
            if (other.gameObject.GetComponent<IdentifiableScript>().HasIdentifier("Dropped"))
            {
                try
                {
                    transform.parent.GetComponent<AttachScript>().HandleOnTriggerStay(other);
                }

                catch
                {
                    //Stop null reference exceptions that shouldn't happen but occur when you try to pick this up when it's attached
                    //to something else; Also has the side effect of "loosening" the attachment it has to the attachment base object,
                    //so the use of ReAttach() "re-tightens" the attachment so that it isn't "loose"

                    AttachedTo.ReAttach(this.gameObject);
                    AttachedTo.HandleOnTriggerStay(other);
                }
            }
        }
    }

    public override void HandleOnMouseDown()
    {
        if (HasIdentifier("Attached"))
        {
            //pass to the object this one is attached to
            try
            {
                //attachedTo.CheckCollisionTrigger(other);
                transform.parent.GetComponent<MoveObjectScript>().HandleOnMouseDown();
            }

            catch
            {
                //Stop null reference exceptions that shouldn't happen but occur when you try to pick this up when it's attached
                //to something else; Also has the side effect of "loosening" the attachment it has to the attachment base object,
                //so the use of ReAttach() "re-tightens" the attachment so that it isn't "loose"

                AttachedTo.ReAttach(this.gameObject);
                AttachedTo.gameObject.GetComponent<MoveObjectScript>().HandleOnMouseDown();
            }
        }
        else
        {
            base.HandleOnMouseDown();
        }
    }

    public override void HandleOnMouseUp()
    {
        if (HasIdentifier("Attached"))
        {
            Body.useGravity = true;
            Body.isKinematic = false;
            transform.parent = AttachedTo.GetGuide(this.gameObject).transform;
            transform.position = AttachedTo.GetGuide(this.gameObject).transform.position;
            RemoveIdentifier("PlayerMoving");
            AttachedTo.gameObject.GetComponent<MoveObjectScript>().HandleOnMouseDown();
            AttachedTo.gameObject.GetComponent<MoveObjectScript>().HandleOnMouseUp();
        }
        else
        {
            base.HandleOnMouseUp();
        }
    }

    public AttachScript AttachedTo
    {
        get
        {
            return attachedTo;
        }
        set
        {
            attachedTo = value;
        }
    }
}