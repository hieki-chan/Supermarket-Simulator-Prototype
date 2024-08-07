using UnityEngine;
using UnityEngine.InputSystem;
using Hieki.Utils;
using UnityEngine.Events;
using System;
using Hieki.Pubsub;

namespace Supermarket.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        public bool disableMove;
        public bool disableLook;

        [Header("Movement")]
        [SerializeField]
        private float moveSpeed;
        Vector2 move;
        public Joystick moveJoyStick;
        public LookZone lookZone;

        //[Header("Ground Check")]
        //[SerializeField, NonEditable]
        //private bool isGrounded;
        //[SerializeField]
        //private float groundCheckDistance;

        [SerializeField] private UnityEvent OnMove;

        [Header("Camera")]
        [SerializeField]
        private Camera mainCamera;
        [NonSerialized]
        public Transform cameraTrans;
        [SerializeField] private Vector3 bodyOffset;
        [SerializeField, Range(-120, 0)] private float lookDownClampedAngle;
        [SerializeField, Range(0, 120)] private float lookUpClampedAngle;
        [SerializeField] private float lookSmoothSpeed;
        [SerializeField] private float lookSensitivity;     //default sensitivity.
        [SerializeField, NonEditable] private float finalLookSensitivity;   //final sensitivity.
        [SerializeField, Tooltip("Inverse look up/down")]
        private bool inverseX;
        [SerializeField, Tooltip("Inverse look left/right")]
        private bool inverseY;
        Vector2 look;
        float rotateX;
        float rotateY;
        bool lookPerformed;

        [Header("Interaction")]
        [SerializeField]
        private float maxInteractDistance;

        public Interactable currentInteraction { get => m_currentInteraction; set { if (value == null && m_currentInteraction != null) OnInteractExit(); else m_currentInteraction = value; } }

        Interactable targetHovered;
        Interactable m_currentInteraction;

        [Header("Throw")]

        PlayerInputListener playerInput;
        [NonSerialized]
        public CharacterController m_Controller;

        //-----------------------------Sub Event----------------------------\\
        //Camera Sensivivity event.
        Topic camSenTopic = Topic.FromMessage<CamSensitivityMessage>();
        Topic controlTopic = Topic.FromMessage<ControlStateMessage>();
        Topic interactTopic = Topic.FromMessage<InteractMessage>();

        ISubscriber subscriber = new Subscriber();

        private void Awake()
        {
            playerInput = new PlayerInputListener();

            //Subcribes events
            subscriber.Subscribe<CamSensitivityMessage>(camSenTopic, (message) =>
            {
                finalLookSensitivity = lookSensitivity * message.camSensitivity;
            });

            subscriber.Subscribe<ControlStateMessage>(controlTopic, (message) =>
            {
                disableMove = disableLook = !message.state;
            });

            subscriber.Subscribe<InteractMessage>(interactTopic, (message) =>
            {
                if(currentInteraction != null && currentInteraction != message.interaction)
                {
                    currentInteraction.OnInteractExit();
                }
                currentInteraction = message.interaction;
            });
        }

        private void Start()
        {
            m_Controller = GetComponent<CharacterController>();
            cameraTrans = mainCamera.transform;
        }

        private void OnEnable()
        {
            playerInput.Player.Touch.performed += OnTouch;

            playerInput.Enable();
        }

        private void OnDestroy()
        {
            playerInput.Player.Touch.performed -= OnTouch;

            playerInput.Disable();
        }

        private void Update()
        {
            lookPerformed = lookZone.performed;
            //move = disableMove? Vector2.zero : playerInput.Player.Move.ReadValue<Vector2>();
            look = (disableLook || !lookPerformed) ? Vector2.zero : playerInput.Player.Look.ReadValue<Vector2>();

            //print(playerInput.Player.Look.ReadValue<Vector2>());

            moveJoyStick.enabled = !disableMove;
            move = moveJoyStick.Direction;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Vector2 m = playerInput.Player.Move.ReadValue<Vector2>();
            if (m != Vector2.zero && move == Vector2.zero)
                move = disableMove ? Vector2.zero : m;
#endif

            MoveRelative(move);
            //GroundCheck();
            Gravity();

            RayCastTarget();

            /*if (Input.GetMouseButtonDown(0))
            {
                OnTap(Input.mousePosition);
            }*/
        }

        private void LateUpdate()
        {
            CameraLook();
        }

        private void MoveRelative(Vector2 direction)
        {
            Vector3 relativeDir = (transform.right * direction.x + transform.forward * direction.y) * (moveSpeed * Time.deltaTime);
            if (m_Controller.enabled)
                m_Controller.Move(relativeDir);

            if (direction != Vector2.zero)
                OnMove?.Invoke();
        }

        private void Gravity()
        {
            if (m_Controller.enabled)
                m_Controller.Move(Vector3.down * (Time.deltaTime * 2.5f));
        }

        //private void GroundCheck()
        //{
        //    Ray groundRay = new Ray(transform.position, -transform.up);
        //    isGrounded = Physics.Raycast(groundRay, out RaycastHit hit, groundCheckDistance, 1 << 6);
        //}

        private void CameraLook()
        {
            //body position
            Vector3 body = Position.Offset(transform, bodyOffset);
            cameraTrans.transform.position = body;


            if (disableLook)
                return;

            //camera look
            //look left/right
            rotateY -= look.x * finalLookSensitivity * Time.deltaTime * (inverseY ? -1 : 1);
            rotateY = Mathf.Repeat(rotateY, 360);

            //look up/down
            rotateX -= look.y * finalLookSensitivity * Time.deltaTime * (inverseX ? -1 : 1);
            rotateX = Mathf.Clamp(rotateX, lookDownClampedAngle, lookUpClampedAngle);

            cameraTrans.localRotation = Quaternion.Slerp(cameraTrans.localRotation, Quaternion.Euler(rotateX, rotateY, 0), Time.deltaTime * lookSmoothSpeed);

            //player look
            Vector3 euler = transform.eulerAngles;
            euler.y = cameraTrans.transform.eulerAngles.y;
            transform.eulerAngles = euler;
        }

        private void RayCastTarget()
        {
            Vector3 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            if (RayCastTarget(screenCenter, out RaycastHit hit))
            {
                if (hit.collider.transform.root.TryGetComponent<Interactable>(out var interaction))
                {
                    if (targetHovered == null)
                    {
                        HoverEnter(interaction);
                    }
                    else if (targetHovered != null && targetHovered != interaction)
                    {
                        HoverExit(targetHovered);

                        HoverEnter(interaction);
                    }
                    //interact with target
                    targetHovered = interaction;
                    targetHovered.OnHover();

                    if (currentInteraction != null)
                    {
                        if (currentInteraction != targetHovered)
                        {
                            currentInteraction.OnHoverOther(targetHovered);
                            currentInteraction.OnHoverOther(hit.collider);
                            OnInteractWithUpdated?.Invoke(currentInteraction);
                        }
                    }
                    return;
                }
                else
                {
                    if (currentInteraction != null)
                    {
                        currentInteraction.OnHoverOther(hit.collider);
                    }
                }
            }
            if (targetHovered != null)
            {
                HoverExit(targetHovered);
                targetHovered = null;
            }

            if (currentInteraction != null)
            {
                currentInteraction.OnHoverOtherExit();
                OnInteractWithUpdated?.Invoke(currentInteraction);
            }
        }

        public void OnTouch(InputAction.CallbackContext context)
        {
            //print("touch");

            Vector2 screenPos = playerInput.Player.SceenPosition.ReadValue<Vector2>();

            OnTap(screenPos);
        }

        private void OnTap(Vector2 screenPos)
        {
            /*            if (targetHovered == null)
                            return;*/

            if (RayCastTarget(screenPos, out RaycastHit hit))
            {
                if (!hit.collider.transform.root.TryGetComponent<Interactable>(out var hitInteraction))
                {
                    return;
                }

                if (hitInteraction != targetHovered)
                {
                    currentInteraction?.OnInteractOther(hitInteraction);
                    return;
                }
                if (currentInteraction == targetHovered || targetHovered == null || currentInteraction != null)
                {
                    return;
                }

                //currentInteraction = targetHovered;
                //currentInteraction.OnInteract(this);
                targetHovered.OnInteract(this);
                //currentInteraction.OnNoInteraction += OnInteractExit;

                OnInteractWithUpdated?.Invoke(targetHovered);
            }
        }

        public void OnInteractExit()
        {
            m_currentInteraction?.OnInteractExit();
            OnInteractWithUpdated?.Invoke(null);
            //currentInteraction.OnNoInteraction -= OnInteractExit;
            m_currentInteraction = null;
        }

        private void HoverEnter(Interactable interaction)
        {
            interaction.OnHoverEnter(this);
            OnHoverEntered?.Invoke(interaction);
        }

        private void HoverExit(Interactable interaction)
        {
            interaction.OnHoverExit();
            OnHoverExited?.Invoke(interaction);
        }

        private bool RayCastTarget(Vector2 screenPos, out RaycastHit hit)
        {
            Ray ray = mainCamera.ScreenPointToRay(screenPos);
            return Physics.Raycast(ray, out hit, maxInteractDistance);
        }

        #region Player Model Events
        public UnityAction<Interactable> OnInteractWithUpdated { get; set; }
        public UnityAction<Interactable> OnHoverEntered { get; set; }
        public UnityAction<Interactable> OnHoverExited { get; set; }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 body = Position.Offset(transform, bodyOffset);
            Gizmos.DrawWireSphere(body, .3f);

            Gizmos.DrawWireSphere(transform.position, maxInteractDistance);
        }
#endif
    }
}