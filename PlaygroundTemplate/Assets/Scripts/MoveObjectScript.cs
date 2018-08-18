using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectScript : MonoBehaviour
{
    public GameObject item;
    public GameObject tempParent;
    public Transform guide;
    private string storedTag;

	// Use this for initialization
	void Start ()
    {
        item.GetComponent<Rigidbody>().useGravity = true;
        storedTag = "";
	}

    void OnMouseDown()
    {
        item.GetComponent<Rigidbody>().useGravity = false;
        item.GetComponent<Rigidbody>().isKinematic = true;
        item.transform.position = guide.transform.position;
        item.transform.rotation = guide.transform.rotation;
        item.transform.parent = tempParent.transform;
        storedTag = this.tag;
        this.tag = "PlayerMoving";
    }

    void OnMouseUp()
    {
        item.GetComponent<Rigidbody>().useGravity = true;
        item.GetComponent<Rigidbody>().isKinematic = false;
        item.transform.parent = null;
        item.transform.position = guide.transform.position;
        this.tag = "DroppedByPlayer";
    }

    private void OnCollisionTrigger(Collision collision)
    {
        if (collision.gameObject.tag != "AttachBase")
        {
            this.tag = storedTag;
            storedTag = "";
        }
    }
}
