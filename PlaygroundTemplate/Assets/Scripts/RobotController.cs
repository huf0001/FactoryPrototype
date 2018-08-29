using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotController : MonoBehaviour {

    private Animator anim;
    private Rigidbody player;

    public Rigidbody rightHand, leftHand;
    public float grabSpeed = 5f;
    public float sprintSpeed = 0.5f;
    public Transform playerHead;
    public Transform playerCam;

    private float moveFrontBack, moveLeftRight;
    public float moveSpeed = 2f;
    private Vector3 lookInputs, moveInputs, movePos;
    private bool grabR = false;
    private bool grabL = false;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        player = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
	
	// Update is called once per frame
	void Update () {
        moveInputs.z = Input.GetAxis("Vertical");
        moveInputs.x = Input.GetAxis("Horizontal");

        lookInputs.y += Input.GetAxis("Mouse X");
        lookInputs.x -= Input.GetAxis("Mouse Y");
        lookInputs.x = Mathf.Clamp(lookInputs.x, -60f, 10f);

        //sprinting
        //if (Input.GetKey(KeyCode.LeftShift)) { moveSpeed *= sprintSpeed; }
        //controls movement settings/dir
        Vector3 newDir = playerCam.TransformDirection(moveInputs);
        movePos = new Vector3(newDir.x * moveSpeed, 0, newDir.z * moveSpeed);

        //gathering hand input
        if (Input.GetMouseButton(1))
        {
            grabR = true;
        }
        if (Input.GetMouseButton(0))
        {
            grabL = true;
        }
    }

    void FixedUpdate()
    {
        player.velocity = movePos;

        //rotating players head all round
        Vector3 headRot = new Vector3(lookInputs.x, lookInputs.y, 0);
        Quaternion hDeltaRotation = Quaternion.Euler(headRot);
        playerHead.rotation = hDeltaRotation;
        
        //rotating players body side to side
        Vector3 bodyRot = new Vector3(0, lookInputs.y, 0);
        Quaternion pDeltaRotation = Quaternion.Euler(bodyRot);
        //player.rotation = pDeltaRotation;

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
