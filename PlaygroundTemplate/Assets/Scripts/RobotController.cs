using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {


    //robot controller script
    //controls leg/body movment and animation with character controllor
    //arms controlled by physics and rigidbodies 
    public Animator anim;
    public CharacterController player;
    public float moveSpeed = 10f;
    public float rotationSpeed = 75f;

    private float moveDirection;
    private float rotationDirection;
    private Vector3 lookInputs;

    public Rigidbody rightHand, leftHand;
    public float grabSpeed = 100f;
    public Transform playerHead;
    private bool grabR = false;
    private bool grabL = false;


    // Use this for initialization
    void Start () {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {
        //move and rotate player
        moveDirection = Input.GetAxis("Vertical") * moveSpeed;
        rotationDirection = Input.GetAxis("Horizontal") * rotationSpeed;
        moveDirection *= Time.deltaTime;
        rotationDirection *= Time.deltaTime;

        player.transform.Translate(0, 0, moveDirection);
        player.transform.Rotate(0, rotationDirection, 0);

        //controls animation
        if (moveDirection != 0)
            { anim.SetBool("IsWalking", true); }
        else { anim.SetBool("IsWalking", false); }

        //gathering hand input
        if (Input.GetMouseButton(1))
        {
            grabR = true;
        }
        if (Input.GetMouseButton(0))
        {
            grabL = true;
        }

        //gathering look input 
        lookInputs.y += Input.GetAxis("Mouse X");
        lookInputs.x -= Input.GetAxis("Mouse Y");
        lookInputs.x = Mathf.Clamp(lookInputs.x, -60f, 9f);
        lookInputs.y = Mathf.Clamp(lookInputs.y, -60f, 60f);
    }

    void FixedUpdate()
    {
        //rotating players head all round to direct hands
        Vector3 headRot = new Vector3(lookInputs.x, lookInputs.y, 0);
        Quaternion hDeltaRotation = Quaternion.Euler(headRot);
        playerHead.rotation = hDeltaRotation;

        //executes hand movement
        //moves hands based on players head rotation 
        //right hand
        if (grabR)
        {
            rightHand.AddForce(playerHead.forward * grabSpeed, ForceMode.Acceleration);
            grabR = false;
            rightHand.velocity = Vector3.zero;
        }

        //left hand
        if (grabL)
        {
            leftHand.AddForce(playerHead.forward * grabSpeed, ForceMode.Acceleration);
            grabL = false;
            leftHand.velocity = Vector3.zero;
        }
    }
}
