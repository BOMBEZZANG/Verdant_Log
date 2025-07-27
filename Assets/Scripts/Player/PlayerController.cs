using UnityEngine;
using UnityEngine.InputSystem;

namespace VerdantLog.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float runSpeed = 8f;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private float deceleration = 10f;
        
        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Debug")]
        [SerializeField] private bool autoFindComponents = true;
        
        private Rigidbody2D rb;
        private Vector2 moveInput;
        private Vector2 currentVelocity;
        private bool isRunning;
        
        public enum PlayerState
        {
            Idle,
            Walking,
            Running
        }
        
        private PlayerState currentState = PlayerState.Idle;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            
            // Auto-find components if enabled
            if (autoFindComponents)
            {
                if (spriteRenderer == null)
                {
                    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        Debug.Log($"[PlayerController] Auto-found SpriteRenderer on: {spriteRenderer.gameObject.name}");
                    }
                }
                
                if (animator == null)
                {
                    animator = GetComponentInChildren<Animator>();
                }
            }
        }
        
        // Unity Input System message methods
        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }
        
        public void OnSprint(InputValue value)
        {
            isRunning = value.isPressed;
        }
        
        private void Update()
        {
            UpdatePlayerState();
            UpdateAnimation();
            UpdateSpriteDirection();
        }
        
        private void FixedUpdate()
        {
            UpdateMovement();
        }
        
        private void UpdatePlayerState()
        {
            if (moveInput.magnitude > 0.01f)
            {
                currentState = isRunning ? PlayerState.Running : PlayerState.Walking;
            }
            else
            {
                currentState = PlayerState.Idle;
            }
        }
        
        private void UpdateMovement()
        {
            float targetSpeed = 0f;
            
            switch (currentState)
            {
                case PlayerState.Walking:
                    targetSpeed = moveSpeed;
                    break;
                case PlayerState.Running:
                    targetSpeed = runSpeed;
                    break;
            }
            
            Vector2 targetVelocity = moveInput.normalized * targetSpeed;
            
            float smoothTime = targetVelocity.magnitude > currentVelocity.magnitude ? acceleration : deceleration;
            currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, smoothTime * Time.fixedDeltaTime);
            
            rb.linearVelocity = currentVelocity;
        }
        
        private void UpdateAnimation()
        {
            if (animator != null)
            {
                animator.SetBool("isWalking", currentState == PlayerState.Walking);
                animator.SetBool("isRunning", currentState == PlayerState.Running);
                animator.SetFloat("moveSpeed", currentVelocity.magnitude);
            }
        }
        
        private void UpdateSpriteDirection()
        {
            if (spriteRenderer != null && moveInput.x != 0)
            {
                spriteRenderer.flipX = moveInput.x < 0;
            }
        }
        
        public Vector2 GetMoveDirection()
        {
            return moveInput.normalized;
        }
        
        public bool IsMoving()
        {
            return currentState != PlayerState.Idle;
        }
        
        public PlayerState GetCurrentState()
        {
            return currentState;
        }
    }
}