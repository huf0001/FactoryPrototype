using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Identifier
{
    // Generic values for movable and attachable objects
    Attachable,
    Attached,
    PlayerMoving,
    Dropped,

    // Generic values for attachment base objects
    AttachBase,

    // Values that can be used to check if a hand is holding something or if it is empty instead
    HandHolding,
    HandEmpty,

    // Value denoting that an object is in the build zone and belongs to a schema in the build zone
    InBuildZone
}

