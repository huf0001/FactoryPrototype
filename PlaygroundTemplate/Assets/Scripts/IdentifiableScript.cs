﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentifiableScript : MonoBehaviour
{
    private List<Identifier> identifiers = new List<Identifier>();

    void Start()
    {
        AddIdentifier(Identifier.HasNotBeenLoadedInBuildZoneYet);
    }

    public bool HasIdentifier(Identifier id)
    {
        bool result = false;

        if (identifiers.Contains(id))
        {
            result = true;
        }

        return result;
    }

    public void AddIdentifier(Identifier id)
    {
        if (!identifiers.Contains(id))
        {
            identifiers.Add(id);
        }
    }

    public void RemoveIdentifier(Identifier id)
    {
        if (identifiers.Contains(id))
        {
            identifiers.Remove(id);
        }
    }

    /*protected*/ public List<Identifier> Identifiers
    {
        get
        {
            return identifiers;
        }
    }
}
