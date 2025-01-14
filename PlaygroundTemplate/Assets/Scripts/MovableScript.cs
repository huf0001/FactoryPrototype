﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableScript : IdentifiableScript
{
    /*[SerializeField]*/ private BuildZoneScript buildZone;
    /*[SerializeField]*/ private GameObject tempLeftParent;
    /*[SerializeField]*/ private GameObject tempRightParent;
    private Transform leftGuide;
    private Transform rightGuide;
    private PickUpScript hands;
    private Rigidbody body;

    // Use this for initialization
    void Start ()
    {
        HandleStart();   
    }

    protected virtual void HandleStart()
    {
        GameControllerScript gameController = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        body = this.gameObject.GetComponent<Rigidbody>();
        body.useGravity = true;

        if (tempLeftParent == null)
        {
            tempLeftParent = gameController.LeftHand;
        }

        if (tempRightParent == null)
        {
            tempRightParent = gameController.RightHand;
        }

        if (buildZone == null)
        {
            buildZone = gameController.BuildZone;
        }

        leftGuide = tempLeftParent.transform;
        rightGuide = tempRightParent.transform;
        hands = tempLeftParent.GetComponentInParent<PickUpScript>();
    }

    public virtual void HandlePickUp(Hand h)
    {
        body.useGravity = false;
        body.isKinematic = true;

        if (h == Hand.Left)
        {
            transform.position = leftGuide.transform.position;
            transform.rotation = leftGuide.transform.rotation;
            transform.parent = tempLeftParent.transform;
        }
        else
        {
            transform.position = rightGuide.transform.position;
            transform.rotation = rightGuide.transform.rotation;
            transform.parent = tempRightParent.transform;
        }

        AddIdentifier(Identifier.PlayerMoving);

        if (HasIdentifier(Identifier.AttachBase))
        {
            this.gameObject.GetComponent<AttachScript>().LayerChange(2);
        }
        else
        {
            this.gameObject.layer = 2;
        }
        
        buildZone.RemoveObject(this);
    }

    public virtual void HandleDrop(Hand h)
    {
        body.useGravity = true;
        body.isKinematic = false;
        transform.parent = null;

        if (h == Hand.Left)
        {
            transform.position = leftGuide.transform.position;
        }
        else
        {
            transform.position = rightGuide.transform.position;
        }

        RemoveIdentifier(Identifier.PlayerMoving);
        AddIdentifier(Identifier.Dropped);

        if (HasIdentifier(Identifier.AttachBase))
        {
            this.gameObject.GetComponent<AttachScript>().LayerChange(0);
        }
        else
        {
            this.gameObject.layer = 0;
        }
    }

    void OnTriggerStay(Collider other)
    {
        HandleOnTriggerStay(other);
    }

    protected virtual void HandleOnTriggerStay(Collider other)
    {
        if (HasIdentifier(Identifier.Dropped))
        {
            RemoveIdentifier(Identifier.Dropped);
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
