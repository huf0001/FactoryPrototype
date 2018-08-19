using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectScript : MonoBehaviour
{
    private GameObject item;
    public GameObject tempParent;
    public Transform guide;
    private Rigidbody body;
    private List<string> identifiers = new List<string>();

    // Use this for initialization
    void Start ()
    {
        item = this.gameObject;
        body = item.GetComponent<Rigidbody>();
        body.useGravity = true;
    }

    public bool HasIdentifier(string id)
    {
        bool result = false;

        if (identifiers.Contains(id))
        {
            result = true;
        }

        return result;
    }

    public void AddIdentifier(string id)
    {
        if (!identifiers.Contains(id))
        {
            identifiers.Add(id);
        }
    }

    public void RemoveIdentifier(string id)
    {
        if (identifiers.Contains(id))
        {
            identifiers.Remove(id);
        }
    }

    void OnMouseDown()
    {
        body.useGravity = false;
        body.isKinematic = true;
        item.transform.position = guide.transform.position;
        item.transform.rotation = guide.transform.rotation;
        item.transform.parent = tempParent.transform;
        AddIdentifier("PlayerMoving");
    }

    void OnMouseUp()
    {
        body.useGravity = true;
        body.isKinematic = false;
        item.transform.parent = null;
        item.transform.position = guide.transform.position;
        RemoveIdentifier("PlayerMoving");
    }
}
