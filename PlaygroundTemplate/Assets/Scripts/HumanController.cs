using UnityEngine;
using System.Collections;

public class HumanController : MonoBehaviour
{


    //Human controller script
    //controls leg/body movment with character controllor
    //arms controlled by physics and rigidbodies 
    public CharacterController player;
    public float moveSpeed = 10f;
    public float rotationSpeed = 75f;
    public Transform handTargetL, handTargetR;

    private float moveDirection;
    private float rotationDirection;
    private Vector3 lookInputs;

    public Rigidbody rightHand, leftHand;
    public float grabSpeed = 150f;
    public Transform playerHead;
    private bool grabR = false;
    private bool grabL = false;

    private float jumpVelocity;
    private float gravity = 14f;
    public float jumpForce = 10f; 

    // Use this for initialization
    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //move and rotate player
        moveDirection = Input.GetAxis("Vertical") * moveSpeed;
        rotationDirection = Input.GetAxis("Horizontal") * rotationSpeed;
        moveDirection *= Time.deltaTime;
        rotationDirection *= Time.deltaTime;

        player.transform.Translate(0, 0, moveDirection);
        player.transform.Rotate(0, rotationDirection, 0);

        //make player jump
        if (player.isGrounded)
        {
            jumpVelocity = -gravity * Time.deltaTime;
            if (Input.GetButtonDown("Jump"))
            {
                jumpVelocity = jumpForce;
            }
        }
        else { jumpVelocity -= gravity * Time.deltaTime; }

        Vector3 move = new Vector3(rotationDirection, jumpVelocity, moveDirection);
        move = transform.TransformDirection(move);
        player.Move(move * Time.deltaTime);

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
    }

    void FixedUpdate()
    {
        //rotating players head all round to direct hands
        Vector3 headRot = new Vector3(lookInputs.x, lookInputs.y, 0);
        Quaternion hDeltaRotation = Quaternion.Euler(headRot);
        playerHead.rotation = hDeltaRotation;

        //executes hand movement
        //moves hands based on players head rotation 
        //there are two targets that rotate with teh mouse x and y 
        //right hand
        if (grabR)
        {
            rightHand.AddForce(handTargetR.forward * grabSpeed, ForceMode.Acceleration);
            grabR = false;
            rightHand.velocity = Vector3.zero;
        }

        //left hand
        if (grabL)
        {
            leftHand.AddForce(handTargetL.forward * grabSpeed, ForceMode.Acceleration);
            grabL = false;
            leftHand.velocity = Vector3.zero;
        }
    }
}