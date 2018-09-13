using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PickUpScript: MonoBehaviour
{
    [SerializeField] private GameObject leftHandGuide;
    [SerializeField] private GameObject rightHandGuide;
    // [SerializeField] private GameObject leftHandObject;
    // [SerializeField] private GameObject rightHandObject;
    [SerializeField] private Camera myCamera;
    // [SerializeField] private float speed;

    private IdentifiableScript leftIDs;
    private IdentifiableScript rightIDs;

    private GameObject movingInLeft = null;
    private GameObject movingInRight = null;

    private bool leftHandPickUp = false;
    private bool rightHandPickUp = false;

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
        // Switch to reprogrammable inputs
        if (leftHandPickUp)
        {
            HandleLeftHandPickUp();
            leftHandPickUp = false;
        }
        else
        {
            leftHandPickUp = CrossPlatformInputManager.GetButtonDown("LeftHandPickUp");
        }

        // Switch to reprogrammable inputs
        if (rightHandPickUp)
        {
            HandleRightHandPickUp();
            rightHandPickUp = false;
        }
        else
        {
            rightHandPickUp = CrossPlatformInputManager.GetButtonDown("RightHandPickUp");
        }
    }

    private void HandleLeftHandPickUp()
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

    private void HandleRightHandPickUp()
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

    /*private void GuideHand(GameObject guide, GameObject hand)
    {
        hand.transform.position = Vector3.MoveTowards(hand.transform.position, guide.transform.position, speed * Time.deltaTime);
    }*/
}
