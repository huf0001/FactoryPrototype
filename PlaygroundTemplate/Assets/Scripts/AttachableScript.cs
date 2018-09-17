using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachableScript : MovableScript
{
    [SerializeField] private Identifier uniqueID = Identifier.Attachable;
    [SerializeField] private Identifier compatibleBase = Identifier.AttachBase;
    private AttachScript attachedTo;

    // Use this for initialization
    void Start()
    {
        HandleStart();
    }

    protected override void HandleStart()
    {
        base.HandleStart();
        AddIdentifier(Identifier.Attachable);
        AddIdentifier(uniqueID);
    }

    //If this is attached to something, this passes the colliding object to the attached base object
    //to see if the colliding object can also be attached
    private void OnTriggerStay(Collider other)
    {
        HandleOnTriggerStay(other);
    }

    protected override void HandleOnTriggerStay(Collider other)
    {
        if (HasIdentifier(Identifier.Dropped))
        {
            HandleDropIDChange(other.gameObject);
        }
        else if (HasIdentifier(Identifier.Attached))
        {
            HandleCollisionWhileAttached(other);
        }
    }

    protected virtual void HandleDropIDChange(GameObject other)
    {
        IdentifiableScript identifiable = other.GetComponent<IdentifiableScript>();
        bool fallenOnAttachBase = false;

        if (identifiable != null)
        {
            if ((identifiable.HasIdentifier(compatibleBase)) || (identifiable.HasIdentifier(Identifier.Attached)))
            {
                fallenOnAttachBase = true;
            }
        }

        if (!fallenOnAttachBase)
        {
            RemoveIdentifier(Identifier.Dropped);
        }
    }

    protected virtual void HandleCollisionWhileAttached(Collider other)
    {
        if (other.gameObject.GetComponent<IdentifiableScript>() != null)
        {
            if (other.gameObject.GetComponent<IdentifiableScript>().HasIdentifier(Identifier.Dropped))
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

    public override void HandlePickUp(Hand h)
    {
        if (HasIdentifier(Identifier.Attached))
        {
            //pass to the object this one is attached to
            try
            {
                //attachedTo.CheckCollisionTrigger(other);
                transform.parent.GetComponent<MovableScript>().HandlePickUp(h);
            }

            catch
            {
                //Stop null reference exceptions that shouldn't happen but occur when you try to pick this up when it's attached
                //to something else; Also has the side effect of "loosening" the attachment it has to the attachment base object,
                //so the use of ReAttach() "re-tightens" the attachment so that it isn't "loose"

                AttachedTo.ReAttach(this.gameObject);
                AttachedTo.gameObject.GetComponent<MovableScript>().HandlePickUp(h);
            }
        }
        else
        {
            base.HandlePickUp(h);
        }
    }

    public override void HandleDrop(Hand h)
    {
        if (HasIdentifier(Identifier.Attached))
        {
            Body.useGravity = true;
            Body.isKinematic = false;
            transform.parent = AttachedTo.GetGuide(this.gameObject).transform;
            transform.position = AttachedTo.GetGuide(this.gameObject).transform.position;
            RemoveIdentifier(Identifier.PlayerMoving);
            AttachedTo.gameObject.GetComponent<MovableScript>().HandlePickUp(h);
            AttachedTo.gameObject.GetComponent<MovableScript>().HandleDrop(h);
        }
        else
        {
            base.HandleDrop(h);
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
