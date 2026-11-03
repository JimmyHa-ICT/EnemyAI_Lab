using UnityEngine;
using UnityEngine.InputSystem;

namespace HUST.EnemyAILab.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionAsset inputActions;

        [Header("Movement")]
        [SerializeField, Min(0f)] private float movementSpeed = 7f;
        [SerializeField, Min(0f)] private float jumpImpulse = 12f;

        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField, Min(0.01f)] private float groundCheckRadius = 0.18f;
        [SerializeField] private LayerMask groundMask;

        private Rigidbody2D body;
        private InputActionMap playerActions;
        private InputAction moveAction;
        private InputAction jumpAction;
        private float horizontalInput;
        private bool jumpQueued;

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            CacheInputActions();
        }

        private void OnEnable()
        {
            if (playerActions == null)
            {
                CacheInputActions();
            }

            playerActions?.Enable();
        }

        private void OnDisable()
        {
            playerActions?.Disable();
        }

        private void Update()
        {
            if (moveAction == null || jumpAction == null)
            {
                return;
            }

            horizontalInput = moveAction.ReadValue<Vector2>().x;
            jumpQueued |= jumpAction.WasPressedThisFrame();
        }

        private void FixedUpdate()
        {
            IsGrounded = groundCheck != null &&
                Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask) != null;

            body.linearVelocity = new Vector2(horizontalInput * movementSpeed, body.linearVelocity.y);

            if (jumpQueued && IsGrounded)
            {
                body.AddForce(Vector2.up * jumpImpulse, ForceMode2D.Impulse);
            }

            jumpQueued = false;

            if (Mathf.Abs(horizontalInput) > 0.01f)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(horizontalInput);
                transform.localScale = scale;
            }
        }

        private void CacheInputActions()
        {
            if (inputActions == null)
            {
                return;
            }

            playerActions = inputActions.FindActionMap("Player", false);
            moveAction = playerActions?.FindAction("Move", false);
            jumpAction = playerActions?.FindAction("Jump", false);

            if (playerActions == null || moveAction == null || jumpAction == null)
            {
                Debug.LogError(
                    "PlayerController requires a Player action map with Move and Jump actions.", this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null)
            {
                return;
            }

            Gizmos.color = IsGrounded ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
