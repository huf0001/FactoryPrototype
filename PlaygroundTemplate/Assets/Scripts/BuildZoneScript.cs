using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildZoneScript : MonoBehaviour
{
    // [SerializeField] private Identifier[] buildIdentifiers;
    [SerializeField] private GameObject[] buildSchemaObjects;

    private List<BuildSchemaScript> schemas = new List<BuildSchemaScript>();

    // Use this for initialization
	void Start ()
    {
        foreach (GameObject o in buildSchemaObjects)
        {
            schemas.Add(o.GetComponent<BuildSchemaScript>());
        }
	}
	
	/*// Update is called once per frame
	void Update ()
    {
		
	}*/

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<IdentifiableScript>() != null)
        {
            IdentifiableScript ids = other.gameObject.GetComponent<IdentifiableScript>();

            foreach (BuildSchemaScript b in schemas)
            {
                if (b.BelongsToSchema(ids))
                {
                    other.gameObject.GetComponent<Rigidbody>().useGravity = false;
                    other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    b.HandleValidObject(other.gameObject);

                    return;
                }
            }
        }
    }

    /*private bool HasValidIdentifier(IdentifiableScript ids)
    {
        foreach (Identifier i in buildIdentifiers)
        {
            if (ids.HasIdentifier(i))
            {
                return true;
            }
        }

        return false;
    }

    private void HandleValidObject(GameObject valid)
    {
        
    }*/
}
