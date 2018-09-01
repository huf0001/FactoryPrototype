using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateJointOnCollision : MonoBehaviour {
    public GameObject Hand;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "climbable")
        {

            if (collision.gameObject.GetComponent<Rigidbody>())
            {
                Hand.AddComponent<FixedJoint>();
                FixedJoint joint = Hand.GetComponent<FixedJoint>();
                joint.connectedBody = collision.gameObject.GetComponent<Rigidbody>();
                Debug.Log("joint created");
            }
        }
    }
}
