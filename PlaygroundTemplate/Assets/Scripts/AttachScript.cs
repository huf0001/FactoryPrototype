using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachScript : MonoBehaviour
{
    Dictionary<Transform, GameObject> AttachedItems;
    List<Transform> AvailableGuides;

    public string attachableTag;

    private void Start()
    {
        AttachedItems = new Dictionary<Transform, GameObject>();
        AvailableGuides = new List<Transform>();

        for(int i = 0; i < transform.childCount; i++)
        {
            AvailableGuides.Add(transform.GetChild(i));
        }
    }

    //If an attachable object collides with this object, attach it
    private void OnTriggerStay(Collider other)
    {
        if ((other.tag == attachableTag) && (CheckCanAttach()))
        {
            Attach(other.gameObject);
        }
    }

    //Just checking that there's an available guide object for each new attached object
    private bool CheckCanAttach()
    {
        bool result = true;

        if (AvailableGuides.Count == 0)
        {
            result = false;
        }

        return result;
    }

    //Attaches attachable objects to this object
    private void Attach(GameObject attaching)
    {
        AssignAttaching(attaching);
        attaching.GetComponent<Rigidbody>().useGravity = false;
        attaching.GetComponent<Rigidbody>().isKinematic = true;
        attaching.transform.parent = this.gameObject.transform;
        attaching.tag = "Attached";
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
}
