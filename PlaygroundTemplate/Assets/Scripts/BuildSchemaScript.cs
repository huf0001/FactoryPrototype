using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSchemaScript : MonoBehaviour
{
    public enum BuildRole
    {
        zero,
        BaseComponent,
        AttachableComponent
    }

    [System.Serializable]
    public class ComponentRolePair
    {
        [SerializeField] private Identifier component;
        [SerializeField] private BuildRole role;

        public Identifier ObjectIdentifier
        {
            get
            {
                return component;
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

    [SerializeField] private string buildPointName = "BuildPoint";
    [SerializeField] private ComponentRolePair[] components;

    private List<Identifier> pendingComponents = new List<Identifier>();
    private Dictionary<Identifier, GameObject> loadedComponents = new Dictionary<Identifier, GameObject>();

    // Use this for initialization
    void Start()
    {
        foreach (ComponentRolePair p in components)
        {
            pendingComponents.Add(p.ObjectIdentifier);
        }
    }

    public bool BelongsToSchema(IdentifiableScript ids)
    {
        foreach (ComponentRolePair p in components)
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
        AttachScript attachBase = null;

        if (!itemIds.HasIdentifier(Identifier.Attached))
        {
            foreach (ComponentRolePair orp in components)
            {
                if (itemIds.HasIdentifier(orp.ObjectIdentifier))
                {
                    pendingComponents.Remove(orp.ObjectIdentifier);
                    loadedComponents.Add(orp.ObjectIdentifier, item);
                    itemIds.RemoveIdentifier(Identifier.HasNotBeenLoadedInBuildZoneYet);

                    if (itemIds.HasIdentifier(Identifier.AttachBase))
                    {
                        attachBase = itemIds.gameObject.GetComponent<AttachScript>();

                        if (attachBase != null)
                        {
                            foreach (KeyValuePair<Transform, GameObject> kvp in attachBase.Attached)
                            {
                                LoadAttachedObject(kvp.Value);
                            }
                        }
                    }

                    return;
                }
            }
        }
    }

    private void LoadAttachedObject(GameObject item)
    {
        IdentifiableScript itemIds = item.GetComponent<IdentifiableScript>();

        if (!CheckIsLoaded(item))
        {
            foreach (ComponentRolePair orp in components)
            {
                if (itemIds.HasIdentifier(orp.ObjectIdentifier))
                {
                    pendingComponents.Remove(orp.ObjectIdentifier);
                    loadedComponents.Add(orp.ObjectIdentifier, item);
                    itemIds.RemoveIdentifier(Identifier.HasNotBeenLoadedInBuildZoneYet);

                    return;
                }
            }
        }
    }

    private bool CheckIsLoaded(GameObject item)
    {
        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            if (p.Value == item)
            {
                return true;
            }
        }

        return false;
    }

    public void RemoveObject(GameObject item)
    {
        AttachableScript attached = null;
        AttachScript attachBase = null;
        
        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            if (p.Value == item)
            {
                pendingComponents.Add(p.Key);
                loadedComponents.Remove(p.Key);

                if (item.GetComponent<IdentifiableScript>().HasIdentifier(Identifier.Attached))
                {
                    attached = item.GetComponent<AttachableScript>();

                    if (attached != null)
                    {
                        RemoveAttachBaseObject(attached.AttachedTo.gameObject);
                    }
                }
                else if (item.GetComponent<IdentifiableScript>().HasIdentifier(Identifier.AttachBase))
                {
                    attachBase = item.GetComponent<AttachScript>();

                    if (attachBase != null)
                    {
                        foreach (KeyValuePair<Transform, GameObject> kvp in attachBase.Attached)
                        {
                            RemoveAttachedObject(kvp.Value);
                        }
                    }
                }
                
                return;
            }
        }
    }

    private void RemoveAttachBaseObject(GameObject item)
    {
        AttachScript attachBase = null;

        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            if (p.Value == item)
            {
                pendingComponents.Add(p.Key);
                loadedComponents.Remove(p.Key);

                if (item.GetComponent<IdentifiableScript>().HasIdentifier(Identifier.AttachBase))
                {
                    attachBase = item.GetComponent<AttachScript>();

                    if (attachBase != null)
                    {
                        foreach (KeyValuePair<Transform, GameObject> kvp in attachBase.Attached)
                        {
                            RemoveAttachedObject(kvp.Value);
                        }
                    }
                }

                return;
            }
        }
    }

    private void RemoveAttachedObject(GameObject item)
    {
        if (CheckIsLoaded(item))
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

    private void Build()
    {
        AttachScript baseAttacher = null;
        GameObject baseComponent = GetBaseComponentAsObjectForBuilding();

        if (baseComponent != null)
        {
            baseAttacher = baseComponent.GetComponent<AttachScript>();

            if (baseAttacher != null)
            {
                foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
                {
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
        ComponentRolePair orp = GetBaseComponentAsObjectRolePair();

        if (orp != null)
        {
            GameObject result = loadedComponents[orp.ObjectIdentifier];
            return result;
        }

        return null;
    }

    private ComponentRolePair GetBaseComponentAsObjectRolePair()
    {
        foreach (ComponentRolePair orp in components)
        {
            if (orp.Role == BuildRole.BaseComponent)
            {
                return orp;
            }
        }

        return null;
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
            Debug.Log("Warning: Couldn't find the game object '" + buildPointName + "'. " + buildPointName + " should be childed to the build " +
                "zone that this schema is attached to.");
        }
    }
}
