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
        [SerializeField] private Identifier objectIdentifier;
        [SerializeField] private BuildRole role;

        public Identifier ObjectIdentifier
        {
            get
            {
                return objectIdentifier;
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
        LoadObject(item);

        if (pendingComponents.Count == 0)
        {
            Build();
        }
    }

    private void LoadObject(GameObject item)
    {
        IdentifiableScript itemIds = item.GetComponent<IdentifiableScript>();

        foreach (ObjectRolePair p in componentIdentifiers)
        {
            if (itemIds.HasIdentifier(p.ObjectIdentifier))
            {
                pendingComponents.Remove(p.ObjectIdentifier);
                loadedComponents.Add(p.ObjectIdentifier, item);

                return;
            }
        }
    }

    private void Build()
    {
        AttachScript baseAttacher = null;
        GameObject baseComponent = GetBaseComponentAsObjectForBuilding();
        Transform buildPoint = null;

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

                buildPoint = this.transform.parent.Find(buildPointName);

                if (buildPoint != null)
                {
                    baseComponent.transform.position = buildPoint.position;
                    baseComponent.transform.rotation = buildPoint.rotation;
                }
                else
                {
                    Debug.Log("Warning: Couldn't find the game object '" + buildPointName + "'. " + buildPointName + " should be childed to the build" +
                        "zone that this schema is attached to.");
                }
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
}
