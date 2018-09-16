using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSchemaScript : MonoBehaviour
{
    public enum BuildRole
    {
        Base,
        AttachableComponent
    }

    [System.Serializable]
    public class ObjectRolePair
    {
        [SerializeField] private Identifier primaryObjectIdentifier;
        [SerializeField] private Identifier secondaryObjectIdentifier;
        [SerializeField] private BuildRole role;

        public Identifier ObjectIdentifier
        {
            get
            {
                return primaryObjectIdentifier;
            }
        }

        public Identifier SecondaryIdentifier
        {
            get
            {
                return secondaryObjectIdentifier;
            }
        }

        public BuildRole Role
        {
            get
            {
                return role;
            }
        }
    }

    [SerializeField] private ObjectRolePair[] componentIdentifiers;
    private string buildPointName = "BuildPoint";
    private List<Identifier> pendingComponents = new List<Identifier>();
    private Dictionary<Identifier, GameObject> loadedComponents = new Dictionary<Identifier, GameObject>();

    // Use this for initialization
    void Start()
    {
        foreach (ObjectRolePair p in componentIdentifiers)
        {
            pendingComponents.Add(p.ObjectIdentifier);
        }
    }

    public bool BelongsToSchema(GameObject item)
    {
        IdentifiableScript ids = null;
        ids = item.GetComponent<IdentifiableScript>();

        if (ids != null)
        {
            return BelongsToSchema(ids);
        }

        return false;
    }

    public bool BelongsToSchema(IdentifiableScript ids)
    {
        foreach (ObjectRolePair p in componentIdentifiers)
        {
            if (ids.HasIdentifier(p.ObjectIdentifier))
            {
                return true;
            }
        }

        return false;
    }

    public void HandleValidObject(GameObject item)
    {
        Debug.Log("Handling valid object... Object is: " + item);

        LoadObject(item);

        Debug.Log("Pending components: " + pendingComponents.Count);

        if (pendingComponents.Count == 0)
        {
            Build();
        }
    }

    private void LoadObject(GameObject item)
    {
        IdentifiableScript itemIds = item.GetComponent<IdentifiableScript>();
        AttachScript attachBase = null;

        Debug.Log("Loading object. Object is " + item);

        foreach (ObjectRolePair orp in componentIdentifiers)
        {
            if (itemIds.HasIdentifier(orp.ObjectIdentifier))
            {
                pendingComponents.Remove(orp.ObjectIdentifier);
                loadedComponents.Add(orp.ObjectIdentifier, item);
                itemIds.RemoveIdentifier(Identifier.HasNotBeenLoadedInBuildZoneYet);

                Debug.Log("Item loaded is " + orp.ObjectIdentifier);

                if (itemIds.HasIdentifier(Identifier.AttachBase))
                {
                    attachBase = itemIds.gameObject.GetComponent<AttachScript>();

                    if (attachBase != null)
                    {
                        Debug.Log("Loading objects attached to base object...");
                        foreach (KeyValuePair<Transform, GameObject> kvp in attachBase.Attached)
                        {
                            Debug.Log("Attached object is " + kvp.Value);
                            LoadAttachedObject(kvp.Value);
                        }
                    }
                }

                return;
            }
        }
    }

    private void LoadAttachedObject(GameObject item)
    {
        IdentifiableScript itemIds = item.GetComponent<IdentifiableScript>();

        Debug.Log("Loading attached object. Object is " + item);

        foreach (ObjectRolePair orp in componentIdentifiers)
        {
            if (itemIds.HasIdentifier(orp.SecondaryIdentifier))
            {
                Debug.Log("Attached item has secondary identifier " + orp.SecondaryIdentifier);

                pendingComponents.Remove(orp.ObjectIdentifier);
                loadedComponents.Add(orp.ObjectIdentifier, item);
                itemIds.RemoveIdentifier(Identifier.HasNotBeenLoadedInBuildZoneYet);

                Debug.Log("Attached object loaded is " + orp.ObjectIdentifier);

                return;
            }
            else
            {
                Debug.Log("Attached item doesn't have identifier " + orp.ObjectIdentifier);
            }
        }
    }

    private void Build()
    {
        Debug.Log("Running BuildSchemaScript.Build()");

        AttachScript baseAttacher = null;
        GameObject baseComponent = GetBaseComponentAsObjectForBuilding();

        if (baseComponent != null)
        {
            baseAttacher = baseComponent.GetComponent<AttachScript>();

            if (baseAttacher != null)
            {
                foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
                {
                    // Do I want to check that p.Value != baseComponent?

                    if (baseAttacher.CheckCanAttach(p.Value.GetComponent<IdentifiableScript>()))
                    {
                        baseAttacher.Attach(p.Value);
                    }
                }

                CentreInBuildZone(baseComponent);
            }
            else
            {
                Debug.Log("Error: Base object in schema does not have an AttachScript component");
            }
        }
    }

    private GameObject GetBaseComponentAsObjectForBuilding()
    {
        ObjectRolePair orp = GetBaseComponentAsObjectRolePair();

        if (orp != null)
        {
            GameObject result = loadedComponents[orp.ObjectIdentifier];
            loadedComponents.Remove(orp.ObjectIdentifier);
            return result;
        }

        return null;
    }

    private ObjectRolePair GetBaseComponentAsObjectRolePair()
    {
        foreach (ObjectRolePair orp in componentIdentifiers)
        {
            if (orp.Role == BuildRole.Base)
            {
                return orp;
            }
        }

        return null;
    }

    public void HandleObjectRemoval(GameObject item)
    {
        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            if (p.Value == item)
            {
                pendingComponents.Add(p.Key);
                loadedComponents.Remove(p.Key);

                return;
            }
        }
    }

    private void CentreInBuildZone(GameObject item)
    {
        Transform buildPoint = this.transform.parent.Find(buildPointName);

        if (buildPoint != null)
        {
            item.transform.position = buildPoint.position;
            item.transform.rotation = buildPoint.rotation;
        }
        else
        {
            Debug.Log("Warning: Couldn't find the game object '" + buildPointName + "'. " + buildPointName + " should be childed to the build" +
                "zone that this schema is attached to.");
        }
    }
}
