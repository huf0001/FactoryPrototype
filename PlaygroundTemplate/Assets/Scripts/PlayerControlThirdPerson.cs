using UnityEngine;
using System.Collections;

public class PlayerControlThirdPerson : MonoBehaviour

{
    public Rigidbody player;
    public Transform playerCam, playerTransform;
    public Rigidbody rightHand, leftHand;
    public float grabSpeed = 5f;
    public Transform playerHead;

    public float moveSpeed = 2f;
    private Vector3 movement;
    private Vector3 lookInputs;
    private Vector3 movePos;
    private bool grabR = false;
    private bool grabL = false;

    private float mouseX, mouseY, moveFrontBack, moveLeftRight;

    // Use this for initialization
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // Update is called once per frame
    void Update ()
    {
        //gathering movement input 
        moveFrontBack = Input.GetAxis ("Vertical"); 
        moveLeftRight = Input.GetAxis ("Horizontal");

        //controls movement settings/dir
        movement = new Vector3 (moveLeftRight, 0, moveFrontBack);
        Vector3 newDir = playerCam.transform.TransformDirection (movement);
        movePos = new Vector3 (newDir.x * moveSpeed, 0, newDir.z * moveSpeed);

        //gathering look input
        lookInputs.y += Input.GetAxis("Mouse X");
        lookInputs.x -= Input.GetAxis("Mouse Y");
        lookInputs.x = Mathf.Clamp (lookInputs.x, -20f, 20f);

        //gathering hand input
        if (Input.GetMouseButton(1))
        {
            grabR = true;
        }
        if (Input.GetMouseButton(0)) {
            grabL = true;
        }
    }

    void FixedUpdate ()
    {
        //updates player movement & camera rotation
        player.velocity = movePos;
        Vector3 camRotation = new Vector3(lookInputs.x, lookInputs.y, 0);
        Quaternion cDeltaRotation = Quaternion.Euler(camRotation);
        Vector3 newRot = new Vector3 (0, lookInputs.y, 0);
        Quaternion deltaRotation = Quaternion.Euler (newRot);
        player.rotation = deltaRotation;  
        //playerCam.rotation = cDeltaRotation;
        playerHead.rotation = cDeltaRotation;



        //executes hand movement
        //moves hands based on players head rotation 
        //right hand
        if (grabR) {
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