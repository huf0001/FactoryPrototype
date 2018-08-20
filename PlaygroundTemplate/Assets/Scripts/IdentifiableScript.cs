using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentifiableScript : MonoBehaviour
{
    private List<string> identifiers = new List<string>();

    public bool HasIdentifier(string id)
    {
        bool result = false;

        if (identifiers.Contains(id))
        {
            result = true;
        }

        return result;
    }

    public void AddIdentifier(string id)
    {
        if (!identifiers.Contains(id))
        {
            identifiers.Add(id);
        }
    }

    public void RemoveIdentifier(string id)
    {
        if (identifiers.Contains(id))
        {
            identifiers.Remove(id);
        }
    }
}
