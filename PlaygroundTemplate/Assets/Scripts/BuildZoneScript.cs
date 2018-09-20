using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildZoneScript : MonoBehaviour
{
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

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<IdentifiableScript>() != null)
        {
            IdentifiableScript ids = other.gameObject.GetComponent<IdentifiableScript>();

            foreach (BuildSchemaScript b in schemas)
            {
                if (b.BelongsToSchema(ids) && !b.IsLoaded(ids))
                {
                    if (!ids.HasIdentifier(Identifier.PlayerMoving) && !ids.HasIdentifier(Identifier.InBuildZone))
                    {
                        other.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        other.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                        ids.AddIdentifier(Identifier.InBuildZone);
                        b.HandleValidObject(other.gameObject);

                        return;
                    }
                }
            }
        }
    }

    public void RemoveObject(IdentifiableScript ids)
    {
        GameObject item = ids.gameObject;

        if (ids.HasIdentifier(Identifier.PlayerMoving) && ids.HasIdentifier(Identifier.InBuildZone))
        {
            foreach (BuildSchemaScript b in schemas)
            {
                if (b.BelongsToSchema(ids))
                {
                    ids.RemoveIdentifier(Identifier.InBuildZone);
                    b.RemoveObject(item);

                    return;
                }
            }
        }
        
    }
}
