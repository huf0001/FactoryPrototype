using UnityEngine;
using System.Collections;

public class PlayerControlThirdPerson : MonoBehaviour

{
    public Rigidbody player;
    public Transform playerCam, playerTransform, centerPoint;

    public float moveSpeed = 2f;
    private Vector3 movement;
    private Vector3 lookInputs;
    private Vector3 movePos;

    public float zoom;
    private float zoomSpeed = 3;
    private float zoomMin = -4f;
    private float zoomMax = -10f; 

    private float mouseX, mouseY, moveFrontBack, moveLeftRight;

    // Use this for initialization
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        zoom = -7f;
    }
    
    // Update is called once per frame
    void Update ()
    {
        //allows player to rotater camera up and down with right click
        if (Input.GetMouseButton (1)) 
        {
            mouseX -= Input.GetAxis ("Mouse Y");
        }
        
        //controls camera rotation constraints when using right click
        mouseX = Mathf.Clamp(mouseX, -39f, 60f);
        playerCam.LookAt(centerPoint);
        centerPoint.localRotation = Quaternion.Euler (mouseX, 0, 0);
    }

    void FixedUpdate ()
    {
    }
}