using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]

    public class ModalRobotPlayerController : MonoBehaviour
    {
        //Variables for functional crap related to cameras
        [SerializeField] private Camera firstPersonCamera;
        [SerializeField] private Camera thirdPersonCamera;
        [SerializeField] private bool thirdPerson = true;

        private Vector3 FPCameraStartPosition;
        private Vector3 TPCameraStartPosition;
        private Vector3 cameraStartPosition;
        private Camera currentCamera;
        private bool changeCamera;

        //Variables for the cool looking shit
        [SerializeField] private Animator anim;
        [SerializeField] private Transform playerHead;
        [SerializeField] private Rigidbody leftHand, rightHand;
        [SerializeField] private Transform leftTarget, rightTarget;
        [SerializeField] private float grabSpeed = 150f;

        private bool grabR = false;
        private bool grabL = false;

        //Variables for functional crap related to movement
        [SerializeField] private bool horizontalMovement = true;
        [SerializeField] private float walkSpeed = 5;
        [SerializeField] private float runSpeed = 10;
        [SerializeField] [Range(0f, 1f)] private float runStepLengthen = 0.7f;
        [SerializeField] private float jumpSpeed = 10;
        [SerializeField] private float stickToGroundForce = 10;
        [SerializeField] private float gravityMultiplier = 2;
        [SerializeField] private MouseLook mouseLook;
        [SerializeField] private bool useFovKick = true;
        [SerializeField] private FOVKick fovKick = new FOVKick();
        [SerializeField] private bool useHeadBob = false;
        [SerializeField] private CurveControlledBob headBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob jumpBob = new LerpControlledBob();
        [SerializeField] private float stepInterval = 5;
        [SerializeField] private AudioClip[] footStepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip jumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip landSound;           // the sound played when character touches back on ground.

        private bool jump;
        private Vector2 input;
        private Vector3 moveDir = Vector3.zero;
        private CharacterController characterController;
        private CollisionFlags collisionFlags;
        private bool previouslyGrounded;
        private float stepCycle;
        private float nextStep;
        private bool jumping;
        private bool walking;
        private bool toggleHorizontal = false;
        private AudioSource audioSource;

        // Use this for initialization
        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            FPCameraStartPosition = firstPersonCamera.transform.localPosition;
            TPCameraStartPosition = thirdPersonCamera.transform.localPosition;
            
            if (!thirdPerson)
            {
                currentCamera = firstPersonCamera;
                cameraStartPosition = FPCameraStartPosition;
                thirdPersonCamera.enabled = false;
            }
            else
            {
                currentCamera = thirdPersonCamera;
                cameraStartPosition = TPCameraStartPosition;
                firstPersonCamera.enabled = false;
            }
            
            fovKick.Setup(currentCamera);
            headBob.Setup(currentCamera, stepInterval);
            stepCycle = 0f;
            nextStep = stepCycle/2f;
            jumping = false;
            audioSource = GetComponent<AudioSource>();
			mouseLook.Init(transform , currentCamera.transform);
        }

        // Update is called once per frame
        private void Update()
        {
            RotateView();

            // the jump state needs to read here to make sure it is not missed
            if (!jump)
            {
                jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!changeCamera)
            {
                changeCamera = CrossPlatformInputManager.GetButtonDown("ChangeCamera");
            }

            if (!toggleHorizontal)
            {
                toggleHorizontal = CrossPlatformInputManager.GetButtonDown("ToggleHorizontal");
            }

            if (!previouslyGrounded && characterController.isGrounded)
            {
                StartCoroutine(jumpBob.DoBobCycle());
                PlayLandingSound();
                moveDir.y = 0f;
                jumping = false;
            }

            if (!characterController.isGrounded && !jumping && previouslyGrounded)
            {
                moveDir.y = 0f;
            }

            previouslyGrounded = characterController.isGrounded;

            //Gathering input for hand animation
            if (CrossPlatformInputManager.GetButton("GrabLeft"))
            {
                grabL = true;
            }

            if (CrossPlatformInputManager.GetButton("GrabRight"))
            {
                grabR = true;
            }

            //Updating the Robot so that it looks cool
            UpdateWalkingAnimation();
            UpdateArms();
            UpdateHeadRotation();
        }

        private void PlayLandingSound()
        {
            audioSource.clip = landSound;
            audioSource.Play();
            nextStep = stepCycle + .5f;
        }

        private void UpdateWalkingAnimation()
        {
            if (CrossPlatformInputManager.GetButton("Vertical") || (horizontalMovement && CrossPlatformInputManager.GetButton("Horizontal")))
            {
                anim.SetBool("IsWalking", true);
            }
            else
            {
                anim.SetBool("IsWalking", false);
            }
        }

        //Rotates player's head vertically, but keeps its horizontal rotation the same as the body's
        //Note: head rotation and position affects where hands move to
        private void UpdateHeadRotation()
        {
            float rotate = Input.GetAxis("Mouse Y");
            float multiplier = -1;

            playerHead.Rotate(rotate * multiplier, 0, 0);
        }

        //Moves hands based on players head rotation 
        private void UpdateArms()
        {
            if (grabL)
            {
                leftHand.AddForce(leftTarget.forward * grabSpeed, ForceMode.Acceleration);
                grabL = false;
                leftHand.velocity = Vector3.zero;
            }

            if (grabR)
            {
                rightHand.AddForce(rightTarget.forward * grabSpeed, ForceMode.Acceleration);
                grabR = false;
                rightHand.velocity = Vector3.zero;
            }
        }

        private void FixedUpdate()
        {
            Vector3 desiredMove;
            float speed;
            GetInput(out speed);

            if (horizontalMovement)
            {
                // always move along the camera forward as it is the direction that it being aimed at
                desiredMove = transform.forward * input.y + transform.right * input.x;
            }
            else
            {
                // always move along the camera forward as it is the direction that it being aimed at
                desiredMove = transform.forward * input.y /*+ transform.right * input.x*/;
            }

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                               characterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            moveDir.x = desiredMove.x*speed;
            moveDir.z = desiredMove.z*speed;

            if (characterController.isGrounded)
            {
                moveDir.y = -stickToGroundForce;

                if (jump)
                {
                    moveDir.y = jumpSpeed;
                    PlayJumpSound();
                    jump = false;
                    jumping = true;
                }
            }
            else
            {
                moveDir += Physics.gravity*gravityMultiplier*Time.fixedDeltaTime;
            }

            collisionFlags = characterController.Move(moveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);

            if (changeCamera)
            {
                ChangeCamera();
                changeCamera = false;
            }

            if (toggleHorizontal)
            {
                horizontalMovement = !horizontalMovement;
                toggleHorizontal = false;
            }

            if (!thirdPerson)
            {
                UpdateCameraPosition(speed); //Dependant on 1st person or third person
            }

            mouseLook.UpdateCursorLock();
        }

        private void PlayJumpSound()
        {
            audioSource.clip = jumpSound;
            audioSource.Play();
        }

        private void ProgressStepCycle(float speed)
        {
            if (characterController.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0))
            {
                stepCycle += (characterController.velocity.magnitude + (speed*(walking ? 1f : runStepLengthen)))*Time.fixedDeltaTime;
            }

            if (!(stepCycle > nextStep))
            {
                return;
            }

            nextStep = stepCycle + stepInterval;

            PlayFootStepAudio();
        }

        private void PlayFootStepAudio()
        {
            if (!characterController.isGrounded)
            {
                return;
            }

            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, footStepSounds.Length);
            audioSource.clip = footStepSounds[n];
            audioSource.PlayOneShot(audioSource.clip);

            // move picked sound to index 0 so it's not picked next time
            footStepSounds[n] = footStepSounds[0];
            footStepSounds[0] = audioSource.clip;
        }

        private void UpdateCameraPosition(float speed)
        {
            if (!useHeadBob)
            {
                return;
            }
            else
            {
                Vector3 newCameraPosition;

                if ((characterController.velocity.magnitude > 0) && (characterController.isGrounded))
                {
                    currentCamera.transform.localPosition = headBob.DoHeadBob(characterController.velocity.magnitude + (speed * (walking ? 1f : runStepLengthen)));
                    newCameraPosition = currentCamera.transform.localPosition;
                    newCameraPosition.y = currentCamera.transform.localPosition.y - jumpBob.Offset();
                }
                else
                {
                    newCameraPosition = currentCamera.transform.localPosition;
                    newCameraPosition.y = cameraStartPosition.y - jumpBob.Offset();
                }

                currentCamera.transform.localPosition = newCameraPosition;
            }
        }

        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = walking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            walking = !Input.GetKey(KeyCode.LeftShift);
#endif

            // set the desired speed to be walking or running
            speed = walking ? walkSpeed : runSpeed;
            input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (input.sqrMagnitude > 1)
            {
                input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (walking != waswalking && useFovKick && characterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!walking ? fovKick.FOVKickUp() : fovKick.FOVKickDown());
            }
        }

        private void RotateView()
        {
            mouseLook.LookRotation (transform, firstPersonCamera.transform);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            //dont move the rigidbody if the character is on top of it
            if (collisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }

            body.AddForceAtPosition(characterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        private void ChangeCamera()
        {
            firstPersonCamera.enabled = !firstPersonCamera.enabled;
            thirdPersonCamera.enabled = !thirdPersonCamera.enabled;

            if (thirdPerson)
            {
                currentCamera = firstPersonCamera;
                cameraStartPosition = FPCameraStartPosition;
            }
            else
            {
                currentCamera = thirdPersonCamera;
                cameraStartPosition = TPCameraStartPosition;
            }

            thirdPerson = !thirdPerson;
            fovKick.Setup(currentCamera);
            headBob.Setup(currentCamera, stepInterval);
            mouseLook.Init(transform, currentCamera.transform);
        }
    }
}
