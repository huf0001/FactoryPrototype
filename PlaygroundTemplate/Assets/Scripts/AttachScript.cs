﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachScript : MonoBehaviour
{
    Dictionary<Transform, GameObject> AttachedItems;
    List<Transform> AvailableGuides;

    private Identifier compatibleAttachableObjectID;

    private void Start()
    {
        compatibleAttachableObjectID = Identifier.Attachable;
        AttachedItems = new Dictionary<Transform, GameObject>();
        AvailableGuides = new List<Transform>();

        for(int i = 0; i < transform.childCount; i++)
        {
            AvailableGuides.Add(transform.GetChild(i));
        }

        if (this.gameObject.GetComponent<IdentifiableScript>() != null)
        {
            this.gameObject.GetComponent<IdentifiableScript>().AddIdentifier(Identifier.AttachBase);
        }
        else
        {
            Debug.Log("For " + this.gameObject.name + ", this.gameObject.GetComponent<IdentifiableScript>() doesn't exist.");
        }
    }

    //If an attachable object collides with this object, attach it
    private void OnTriggerStay(Collider other)
    {
        HandleOnTriggerStay(other);
    }

    //For use if a dropped object collides with this object or an attached object
    public void HandleOnTriggerStay(Collider other)
    {
        IdentifiableScript ids = other.gameObject.GetComponent<IdentifiableScript>();

        if (ids != null)
        {
            if (!ids.HasIdentifier(Identifier.Attached))
            {
                if (CheckCanAttach(ids))
                {
                    Attach(other.gameObject);
                }
            }
        }
    }

    //Just checking that there's an available guide object for each new attached object, and that said object
    //is attachable in the first place
    public bool CheckCanAttach(IdentifiableScript temp, BuildSchemaScript schema)
    {
        bool result = true;

        if (
                (AvailableGuides.Count == 0) ||
                (!schema.BelongsToSchema(temp)) ||
                (
                    (!temp.HasIdentifier(Identifier.Dropped)) &&
                    (!temp.HasIdentifier(Identifier.InBuildZone))
                )
           )
        {
            result = false;
        }

        return result;
    }

    //Just checking that there's an available guide object for each new attached object, and that said object
    //is attachable in the first place
    public bool CheckCanAttach(IdentifiableScript temp)
    {
        bool result = true;

        if (
                (AvailableGuides.Count == 0)||
                (!temp.HasIdentifier(compatibleAttachableObjectID))||
                (
                    (!temp.HasIdentifier(Identifier.Dropped))&&
                    (!temp.HasIdentifier(Identifier.InBuildZone))
                )
           )
        {
            result = false;
        }

        return result;
    }

    //Attaches attachable objects to this object
    public void Attach(GameObject attaching)
    {
        AssignAttaching(attaching);
        attaching.GetComponent<Rigidbody>().useGravity = false;
        attaching.GetComponent<Rigidbody>().isKinematic = true;
        attaching.transform.parent = this.gameObject.transform;
        attaching.GetComponent<IdentifiableScript>().AddIdentifier(Identifier.Attached);
        attaching.GetComponent<IdentifiableScript>().RemoveIdentifier(Identifier.Attachable);
        attaching.GetComponent<IdentifiableScript>().RemoveIdentifier(Identifier.Dropped);
        attaching.GetComponent<AttachableScript>().AttachedTo = this;
    }

    //Assigns the object being attached to an appropriate attached object slot (and guide object?)
    private void AssignAttaching(GameObject attaching)
    {
        Transform tempGuide = AvailableGuides[0];
        AvailableGuides.Remove(tempGuide);
        AttachedItems.Add(tempGuide, attaching);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<Transform, GameObject> p in AttachedItems)
        {
            UpdateAttached(p.Value, p.Key);
        }
    }

    //Updates position and location for the specified attached object
    private void UpdateAttached(GameObject attachedItem, Transform attachedGuide)
    {
        attachedItem.transform.position = attachedGuide.transform.position;
        attachedItem.transform.rotation = attachedGuide.transform.rotation;
    }

    public void ReAttach(GameObject reAttaching)
    {
        reAttaching.GetComponent<Rigidbody>().useGravity = false;
        reAttaching.GetComponent<Rigidbody>().isKinematic = true;
        reAttaching.transform.parent = this.gameObject.transform;
        reAttaching.GetComponent<IdentifiableScript>().AddIdentifier(Identifier.Attached);
        reAttaching.GetComponent<IdentifiableScript>().RemoveIdentifier(Identifier.Attachable);
        reAttaching.GetComponent<IdentifiableScript>().RemoveIdentifier(Identifier.Dropped);
        reAttaching.GetComponent<AttachableScript>().AttachedTo = this;

        foreach (KeyValuePair<Transform, GameObject> p in AttachedItems)
        {
            if (p.Value == reAttaching)
            {
                Transform k = p.Key;
                GameObject v = p.Value;

                AttachedItems.Remove(k);
                AttachedItems.Add(k, v);
            }

            return;
        }
    }

    public Transform GetGuide(GameObject attached)
    {
        foreach (KeyValuePair<Transform, GameObject> p in AttachedItems)
        {
            if (p.Value == attached)
            {
                return p.Key;
            }
        }

        return null;
    }

    public void LayerChange(int layer)
    {
        if (layer == 2)
        {
            this.gameObject.layer = 2;

            foreach (KeyValuePair<Transform, GameObject> p in AttachedItems)
            {
                p.Value.layer = 2;
            }
        }
        else
        {
            this.gameObject.layer = 0;

            foreach (KeyValuePair<Transform, GameObject> p in AttachedItems)
            {
                p.Value.layer = 0;
            }
        }
    }
}
