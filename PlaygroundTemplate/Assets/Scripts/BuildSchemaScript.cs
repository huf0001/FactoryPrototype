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

    // Update is called once per frame
    void Update()
    {

    }

    public bool BelongsToSchema(IdentifiableScript ids)
    {
        foreach (Identifier i in pendingComponents)
        {
            if (ids.HasIdentifier(i))
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

        if (baseComponent != null)
        {
            baseAttacher = baseComponent.GetComponent<AttachScript>();

            if (baseAttacher != null)
            {
                if (baseAttacher.CheckCanAttach(baseComponent.GetComponent<IdentifiableScript>()))
                {
                    baseAttacher.Attach(baseComponent);
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
}
