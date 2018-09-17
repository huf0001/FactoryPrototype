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
        // [SerializeField] private Identifier secondaryObjectIdentifier;
        [SerializeField] private BuildRole role;

        public Identifier ObjectIdentifier
        {
            get
            {
                return objectIdentifier;
            }
        }

        /*public Identifier SecondaryIdentifier
        {
            get
            {
                return secondaryObjectIdentifier;
            }
        }*/

        public BuildRole Role
        {
            get
            {
                return role;
            }
        }
    }

    [SerializeField] private ObjectRolePair[] componentIdentifiers;
    [SerializeField] private string buildPointName = "BuildPoint";

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
        // AttachableScript attached = null;

        Debug.Log("Loading object. Object is " + item);

        if (itemIds.HasIdentifier(Identifier.Attached))
        {
            Debug.Log("Not loading " + item + " yet because it is attached. Will load when it's attach base loads it.");
        }
        else
        { 
            foreach (ObjectRolePair orp in componentIdentifiers)
            {
                if (itemIds.HasIdentifier(orp.ObjectIdentifier))
                {
                    pendingComponents.Remove(orp.ObjectIdentifier);
                    loadedComponents.Add(orp.ObjectIdentifier, item);
                    itemIds.RemoveIdentifier(Identifier.HasNotBeenLoadedInBuildZoneYet);

                    Debug.Log("Item loaded is " + orp.ObjectIdentifier);

                    /*if (itemIds.HasIdentifier(Identifier.Attached))
                    {
                        attached = itemIds.gameObject.GetComponent<AttachableScript>();

                        if (attached != null)
                        {
                            LoadAttachBaseObject(attached.AttachedTo.gameObject);
                        }
                    }
                    else*/
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
    }

    /*private void LoadAttachBaseObject(GameObject item)
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
    }*/

    private void LoadAttachedObject(GameObject item)
    {
        IdentifiableScript itemIds = item.GetComponent<IdentifiableScript>();

        Debug.Log("Running LoadAttachedObject(). Object is " + item);

        if (!CheckIsLoaded(item))
        {
            Debug.Log("CheckIsLoaded() returned false for " + item);

            foreach (ObjectRolePair orp in componentIdentifiers)
            {
                if (itemIds.HasIdentifier(orp.ObjectIdentifier))
                {
                    Debug.Log("Attached item has unique identifier " + orp.ObjectIdentifier);

                    pendingComponents.Remove(orp.ObjectIdentifier);
                    loadedComponents.Add(orp.ObjectIdentifier, item);
                    itemIds.RemoveIdentifier(Identifier.HasNotBeenLoadedInBuildZoneYet);

                    Debug.Log("Attached object loaded is " + orp.ObjectIdentifier);

                    return;
                }
                else
                {
                    Debug.Log("Attached item doesn't have unique identifier " + orp.ObjectIdentifier);
                }
            }
        }
        else
        {
            Debug.Log(item + " is already loaded.");
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
        Debug.Log("Running RemoveObject() on " + item + "...");

        AttachableScript attached = null;
        AttachScript attachBase = null;

        Debug.Log("Loaded components are:");

        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            Debug.Log(p.Value);
        }
        
        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            Debug.Log("item is " + item + ". p.Value is " + p.Value + ".");

            if (p.Value == item)
            {
                Debug.Log("Removing " + item + "...");

                pendingComponents.Add(p.Key);
                loadedComponents.Remove(p.Key);

                if (item.GetComponent<IdentifiableScript>().HasIdentifier(Identifier.Attached))
                {
                    attached = item.GetComponent<AttachableScript>();

                    if (attached != null)
                    {
                        Debug.Log("Removing base object that " + item + " is attached to...");

                        RemoveAttachBaseObject(attached.AttachedTo.gameObject);
                    }
                }
                else if (item.GetComponent<IdentifiableScript>().HasIdentifier(Identifier.AttachBase))
                {
                    attachBase = item.GetComponent<AttachScript>();

                    if (attachBase != null)
                    {
                        Debug.Log("Removing objects attached to " + item + "...");

                        foreach (KeyValuePair<Transform, GameObject> kvp in attachBase.Attached)
                        {
                            Debug.Log("Attached object is " + kvp.Value);
                            RemoveAttachedObject(kvp.Value);
                        }
                    }
                }

                Debug.Log(item + " has been removed.");
                
                return;
            }
        }
    }

    private void RemoveAttachBaseObject(GameObject item)
    {
        Debug.Log("Running RemoveAttachBaseObject() on " + item + "...");

        AttachScript attachBase = null;

        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            if (p.Value == item)
            {
                Debug.Log("Removing " + item + "...");

                pendingComponents.Add(p.Key);
                loadedComponents.Remove(p.Key);

                if (item.GetComponent<IdentifiableScript>().HasIdentifier(Identifier.AttachBase))
                {
                    attachBase = item.GetComponent<AttachScript>();

                    if (attachBase != null)
                    {
                        Debug.Log("Removing objects attached to " + item + "...");

                        foreach (KeyValuePair<Transform, GameObject> kvp in attachBase.Attached)
                        {
                            Debug.Log("Attached object is " + kvp.Value);
                            RemoveAttachedObject(kvp.Value);
                        }
                    }
                }

                Debug.Log(item + " has been removed.");

                return;
            }
        }
    }

    private void RemoveAttachedObject(GameObject item)
    {
        Debug.Log("Running RemoveAttachedObject() on " + item + "...");

        if (CheckIsLoaded(item))
        {
            foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
            {
                if (p.Value == item)
                {
                    Debug.Log("Removing " + item + "...");

                    pendingComponents.Add(p.Key);
                    loadedComponents.Remove(p.Key);

                    Debug.Log(item + " has been removed.");

                    return;
                }
            }
        }
        else
        {
            Debug.Log("Cannot remove " + item + ". It is not loaded to begin with.");
        }
    }

    private void Build()
    {
        Debug.Log("Running BuildSchemaScript.Build()");

        Debug.Log("Loaded components before building are:");

        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            Debug.Log(p.Value);
        }

        AttachScript baseAttacher = null;
        GameObject baseComponent = GetBaseComponentAsObjectForBuilding();

        Debug.Log("Loaded components after getting the base component for building but before building are:");

        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            Debug.Log(p.Value);
        }

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

                Debug.Log("Loaded components after building and before centering are:");

                foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
                {
                    Debug.Log(p.Value);
                }

                CentreInBuildZone(baseComponent);
            }
            else
            {
                Debug.Log("Error: Base object in schema does not have an AttachScript component");
            }
        }

        Debug.Log("Loaded components after building are:");

        foreach (KeyValuePair<Identifier, GameObject> p in loadedComponents)
        {
            Debug.Log(p.Value);
        }

        Debug.Log("Finished running Build().");
    }

    private GameObject GetBaseComponentAsObjectForBuilding()
    {
        ObjectRolePair orp = GetBaseComponentAsObjectRolePair();

        if (orp != null)
        {
            GameObject result = loadedComponents[orp.ObjectIdentifier];
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
