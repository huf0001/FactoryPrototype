using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PickUpScript: MonoBehaviour
{
    // [SerializeField] private GameObject leftHandObject;
    // [SerializeField] private GameObject rightHandObject;
    [SerializeField] private GameObject leftHandGuide;
    [SerializeField] private GameObject rightHandGuide;
    [SerializeField] private Camera myCamera;
    [SerializeField] private bool throwOn = false;

    private IdentifiableScript leftIDs;
    private IdentifiableScript rightIDs;

    private GameObject movingInLeft = null;
    private GameObject movingInRight = null;

    private bool leftHandInput = false;
    private bool rightHandInput = false;
    private bool throwWhenDropped = false;

    [SerializeField] private float throwForce = 5f;
    [SerializeField] private Vector3 throwOffset;
    [SerializeField] private Transform headDirection;

    // Use this for initialization
    void Start ()
    {
        leftIDs = leftHandGuide.GetComponent<IdentifiableScript>();
        rightIDs = rightHandGuide.GetComponent<IdentifiableScript>();

        leftIDs.AddIdentifier(Identifier.HandEmpty);
        rightIDs.AddIdentifier(Identifier.HandEmpty);
	}

    private void ChangeIDsFromPickUp(Hand h)
    {
        if (h == Hand.Left)
        {
            leftIDs.RemoveIdentifier(Identifier.HandEmpty);
            leftIDs.AddIdentifier(Identifier.HandHolding);
        }
        else if (h == Hand.Right)
        {
            rightIDs.RemoveIdentifier(Identifier.HandEmpty);
            rightIDs.AddIdentifier(Identifier.HandHolding);
        }
    }

    private void ChangeIDsFromDrop(Hand h)
    {
        if (h == Hand.Left)
        {
            leftIDs.RemoveIdentifier(Identifier.HandHolding);
            leftIDs.AddIdentifier(Identifier.HandEmpty);
        }
        else if (h == Hand.Right)
        {
            rightIDs.RemoveIdentifier(Identifier.HandHolding);
            rightIDs.AddIdentifier(Identifier.HandEmpty);
        }
    }

    public bool IsEmpty(Hand h)
    {
        if (h == Hand.Left)
        {
            return leftIDs.HasIdentifier(Identifier.HandEmpty);
        }
        else 
        {
            return rightIDs.HasIdentifier(Identifier.HandEmpty);
        }
    }

    public bool IsHolding(Hand h)
    {
        if (h == Hand.Left)
        {
            return leftIDs.HasIdentifier(Identifier.HandHolding);
        }
        else 
        {
            return rightIDs.HasIdentifier(Identifier.HandHolding);
        }
    }

    private void Update()
    {
        if (leftHandInput)
        {
            HandleHandInput(Hand.Left);            
            leftHandInput = false;
        }
        else
        {
            leftHandInput = CrossPlatformInputManager.GetButtonDown("LeftHandInput");
        }

        if (rightHandInput)
        {
            HandleHandInput(Hand.Right);
            rightHandInput = false;
        }
        else
        {
            rightHandInput = CrossPlatformInputManager.GetButtonDown("RightHandInput");
        }

        if (throwWhenDropped)
        {
            throwOn = !throwOn;
            throwWhenDropped = false;
        }
        else
        {
            throwWhenDropped = CrossPlatformInputManager.GetButtonDown("ThrowWhenDropped");
        }
    }

    private void HandleHandInput(Hand hand)
    {
        if (IsEmpty(hand))
        {
            HandlePickUp(hand);
        }
        else if (IsHolding(hand))
        {
            HandleDrop(hand);
        }
    }

    private void HandlePickUp(Hand hand)
    {
        Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject item = hit.transform.gameObject;
            bool handledClick = false;

            if (item.GetComponent<AttachableScript>() != null)
            {
                item.GetComponent<AttachableScript>().HandlePickUp(hand);
                handledClick = true;
            }
            else if (item.GetComponent<MovableScript>() != null)
            {
                item.GetComponent<MovableScript>().HandlePickUp(hand);
                handledClick = true;
            }

            if (handledClick)
            {
                if (hand == Hand.Left)
                {
                    movingInLeft = item;
                }
                else
                {
                    movingInRight = item;
                }

                ChangeIDsFromPickUp(hand);
            }
        }
    }

    private void HandleDrop(Hand hand)
    {
        bool handledClick = false;
        AttachableScript attachable = null;
        MovableScript movable = null;
        GameObject item = null;

        if (hand == Hand.Left)
        {
            if (movingInLeft.GetComponent<AttachableScript>() != null)
            {
                attachable = movingInLeft.GetComponent<AttachableScript>();
            }
            else if (movingInLeft.GetComponent<MovableScript>() != null)
            {
                movable = movingInLeft.GetComponent<MovableScript>();
            }
        }
        else
        {
            if (movingInRight.GetComponent<AttachableScript>() != null)
            {
                attachable = movingInRight.GetComponent<AttachableScript>();
            }
            else if (movingInRight.GetComponent<MovableScript>() != null)
            {
                movable = movingInRight.GetComponent<MovableScript>();
            }
        }

        if (attachable != null)
        {
            attachable.HandleDrop(hand);
            handledClick = true;
        }
        else if (movable != null)
        {
            movable.HandleDrop(hand);
            handledClick = true;
        }

        if (handledClick)
        {
            if (hand == Hand.Left)
            {
                item = movingInLeft;
                movingInLeft = null;
            }
            else
            {
                item = movingInRight;
                movingInRight = null;
            }

            if (throwOn)
            {
                ThrowObject(item, hand);
            }

            ChangeIDsFromDrop(hand);
        }
    }

    private void ThrowObject(GameObject projectile, Hand hand)
    {
        // Adjusting the throwOffset for the hand that's doing the throwing
        if (hand == Hand.Left)
        {
            throwOffset.x = 1f;
        }
        else
        {
            throwOffset.x = -1f;
        }
        
        //transforms the instantiate position into world space based on the head rotation
        Vector3 origin = headDirection.TransformDirection(throwOffset);

        //adds force to the objecct
        if (projectile.GetComponent<Rigidbody>() != null)
        {
            Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
            projectileRigidbody.AddForce(origin * throwForce, ForceMode.Impulse);
        }
        else
            Debug.LogError("The gameobject you are trying to throw does not have a rigidbody");
    }
}
