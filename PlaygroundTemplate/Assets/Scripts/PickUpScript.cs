using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript: MonoBehaviour
{
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private Camera myCamera;

    private IdentifiableScript leftIDs;
    private IdentifiableScript rightIDs;

    private GameObject movingInLeft;
    private GameObject movingInRight;

    // Use this for initialization
	void Start ()
    {
        leftIDs = leftHand.GetComponent<IdentifiableScript>();
        rightIDs = rightHand.GetComponent<IdentifiableScript>();

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
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }
    }

    private void HandleLeftClick()
    {
        if (IsEmpty(Hand.Left))
        {
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject item = hit.transform.gameObject;
                bool handledClick = false;

                if (item.GetComponent<AttachableScript>() != null)
                {
                    item.GetComponent<AttachableScript>().HandlePickUp(Hand.Left);
                    handledClick = true;
                }
                else if (item.GetComponent<MovableScript>() != null)
                {
                    item.GetComponent<MovableScript>().HandlePickUp(Hand.Left);
                    handledClick = true;
                }

                if (handledClick)
                {
                    movingInLeft = item;
                    ChangeIDsFromPickUp(Hand.Left);
                }
            }
        }
        else if (IsHolding(Hand.Left))
        {
            bool handledClick = false;

            if (movingInLeft.GetComponent<AttachableScript>() != null)
            {
                movingInLeft.GetComponent<AttachableScript>().HandleDrop(Hand.Left);
                handledClick = true;
            }
            else if (movingInLeft.GetComponent<MovableScript>() != null)
            {
                movingInLeft.GetComponent<MovableScript>().HandleDrop(Hand.Left);
                handledClick = true;
            }

            if (handledClick)
            {
                ChangeIDsFromDrop(Hand.Left);
                movingInLeft = null;
            }
        }
    }

    private void HandleRightClick()
    {
        if (IsEmpty(Hand.Right))
        {
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject item = hit.transform.gameObject;
                bool handledClick = false;

                if (item.GetComponent<AttachableScript>() != null)
                {
                    item.GetComponent<AttachableScript>().HandlePickUp(Hand.Right);
                    handledClick = true;
                }
                else if (item.GetComponent<MovableScript>() != null)
                {
                    item.GetComponent<MovableScript>().HandlePickUp(Hand.Right);
                    handledClick = true;
                }

                if (handledClick)
                {
                    movingInRight = item;
                    ChangeIDsFromPickUp(Hand.Right);
                }
            }
        }
        else if (IsHolding(Hand.Right))
        {
            bool handledClick = false;

            if (movingInRight.GetComponent<AttachableScript>() != null)
            {
                movingInRight.GetComponent<AttachableScript>().HandleDrop(Hand.Right);
                handledClick = true;
            }
            else if (movingInRight.GetComponent<MovableScript>() != null)
            {
                movingInRight.GetComponent<MovableScript>().HandleDrop(Hand.Right);
                handledClick = true;
            }

            if (handledClick)
            {
                ChangeIDsFromDrop(Hand.Right);
                movingInRight = null;
            }
        }
    }
}
