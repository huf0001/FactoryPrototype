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
        [SerializeField] private bool thirdPerson = false;
        [SerializeField] private Camera firstPersonCamera;
        [SerializeField] private Camera thirdPersonCamera;

        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed = 5;
        [SerializeField] private float m_RunSpeed = 10;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten = 0.7f;
        [SerializeField] private float m_JumpSpeed = 10;
        [SerializeField] private float m_StickToGroundForce = 10;
        [SerializeField] private float m_GravityMultiplier = 2;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick = true;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob = false;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval = 5;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

/// <summary>
/// Start variables being integrated
/// </summary>

        // robot controller script
        // controls leg/body movment and animation with character controllor
        // arms controlled by physics and rigidbodies 
        [SerializeField] private Animator anim;
        [SerializeField] private CharacterController player;
        [SerializeField] private Transform handTargetL, handTargetR;

    //Taken care of in existing code already?
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float rotationSpeed = 75f;
    //

        [SerializeField] private Rigidbody rightHand, leftHand;
        [SerializeField] private float grabSpeed = 150f;
        [SerializeField] private Transform playerHead;

        [SerializeField] private float jumpForce = 10f;

        private float moveDirection;
        private float rotationDirection;
        private Vector3 lookInputs;

        private bool grabR = false;
        private bool grabL = false;

        private float jumpVelocity;
        private float gravity = 14f; 

/// <summary>
/// End variables being integrated
/// </summary>

        private Vector3 OriginalFPCameraPosition;
        private Vector3 OriginalTPCameraPosition;

        private Camera m_Camera;
        private bool m_Jump;
        private bool m_ChangeCamera;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            OriginalFPCameraPosition = firstPersonCamera.transform.localPosition;
            OriginalTPCameraPosition = thirdPersonCamera.transform.localPosition;
            
            if (!thirdPerson)
            {
                m_Camera = firstPersonCamera;
                m_OriginalCameraPosition = OriginalFPCameraPosition;
                thirdPersonCamera.enabled = false;
            }
            else
            {
                m_Camera = thirdPersonCamera;
                m_OriginalCameraPosition = OriginalTPCameraPosition;
                firstPersonCamera.enabled = false;
            }

            // m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
        }

        // Update is called once per frame
        private void Update()
        {
            RotateView();

            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_ChangeCamera)
            {
                m_ChangeCamera = CrossPlatformInputManager.GetButtonDown("ChangeCamera");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            //gathering hand input for the animation
            if (CrossPlatformInputManager.GetButton("GrabLeft"))
            {
                grabL = true;
            }

            if (CrossPlatformInputManager.GetButton("GrabRight"))
            {
                grabR = true;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            //UpdateHeadAnimation();

            UpdateWalkingAnimation();
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        private void UpdateWalkingAnimation()
        {
            //controls walking animation
            if (CrossPlatformInputManager.GetButton("Horizontal")||CrossPlatformInputManager.GetButton("Vertical"))
            {
                anim.SetBool("IsWalking", true);
            }
            else
            {
                anim.SetBool("IsWalking", false);
            }
        }

        private void UpdateHeadAnimation()
        {
            //move and rotate player
            moveDirection = Input.GetAxis("Vertical") * moveSpeed;
            rotationDirection = Input.GetAxis("Horizontal") * rotationSpeed;
            moveDirection *= Time.deltaTime;
            rotationDirection *= Time.deltaTime;

            player.transform.Translate(0, 0, moveDirection);
            player.transform.Rotate(0, rotationDirection, 0);

            Vector3 move = new Vector3(rotationDirection, jumpVelocity, moveDirection);
            move = transform.TransformDirection(move);
            player.Move(move * Time.deltaTime);

            //gathering look input 
            lookInputs.y += Input.GetAxis("Mouse X");
            lookInputs.x -= Input.GetAxis("Mouse Y");
            lookInputs.x = Mathf.Clamp(lookInputs.x, -60f, 9f);
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);

            if (m_ChangeCamera)
            {
                ChangeCamera();
                m_ChangeCamera = false;
            }

            if (!thirdPerson)
            {
                UpdateCameraPosition(speed); //Dependant on 1st person or third person
            }

            m_MouseLook.UpdateCursorLock();

            FixedUpdateHeadAnimation();
            UpdateArms();
        }

        private void FixedUpdateHeadAnimation()
        {
            //rotating players head all round to direct hands
            Vector3 headRot = new Vector3(lookInputs.x, lookInputs.y, 0);
            ///Vector3 headRot = new Vector3(0, lookInputs.y, 0);
            Quaternion hDeltaRotation = Quaternion.Euler(headRot);
            playerHead.rotation = hDeltaRotation;
        }

        private void UpdateArms()
        {
            //executes hand movement
            //moves hands based on players head rotation 
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

        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }

        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        private void UpdateCameraPosition(float speed)
        {
            if (!m_UseHeadBob)
            {
                return;
            }
            else
            {
                Vector3 newCameraPosition;

                if ((m_CharacterController.velocity.magnitude > 0) && (m_CharacterController.isGrounded))
                {
                    m_Camera.transform.localPosition =
                        m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                          (speed * (m_IsWalking ? 1f : m_RunstepLenghten)));
                    newCameraPosition = m_Camera.transform.localPosition;
                    newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
                }
                else
                {
                    newCameraPosition = m_Camera.transform.localPosition;
                    newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
                }

                m_Camera.transform.localPosition = newCameraPosition;
            }
        }

        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, firstPersonCamera.transform /*m_Camera.transform*/);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

        private void ChangeCamera()
        {
            firstPersonCamera.enabled = !firstPersonCamera.enabled;
            thirdPersonCamera.enabled = !thirdPersonCamera.enabled;

            if (thirdPerson)
            {
                m_Camera = firstPersonCamera;
                m_OriginalCameraPosition = OriginalFPCameraPosition;
            }
            else
            {
                m_Camera = thirdPersonCamera;
                m_OriginalCameraPosition = OriginalTPCameraPosition;
            }

            thirdPerson = !thirdPerson;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_MouseLook.Init(transform, m_Camera.transform);
        }
    }
}
