using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravityBody : MonoBehaviour
{
	public FauxGravityAttractor attractor;
	private Transform myTransform;
    private Rigidbody rb;
    
    // This runs once an object has finished being 'made' (i.e. instantiated) by unity.
    // Kinda like it 'wakes up' before doing 'Update()' (if it exists)
    private void Awake()
    {
        GameObject playerObject = GameObject.Find("PhysicsPlayer");
        if (playerObject == null)
        {
            Debug.Log("Couldnt find a PhysicsPlayer in the scene! :(");
        }
        else
        {
            attractor = playerObject.GetComponent<FauxGravityAttractor>();
        }
    }

    // This Start() function/code runs at 'inital' play button pressing
    void Start ()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;

		myTransform = transform;
        attractor.AddFauxGravBody(myTransform);
    }

    public void Die()
    {
        attractor.RemoveFauxGravBody(myTransform);
    }
}
