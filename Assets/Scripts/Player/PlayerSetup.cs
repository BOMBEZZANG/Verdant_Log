using UnityEngine;
using UnityEngine.InputSystem;

namespace VerdantLog.Player
{
    [RequireComponent(typeof(PlayerController))]
    [RequireComponent(typeof(PlayerInteractor))]
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Player Components")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerInteractor playerInteractor;
        [SerializeField] private PlayerInventoryModule inventoryModule;
        
        [Header("Physics")]
        [SerializeField] private Rigidbody2D rb2d;
        [SerializeField] private Collider2D playerCollider;
        
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        
        [Header("Input")]
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private InputActionAsset inputActions;
        
        [Header("Layer Settings")]
        [SerializeField] private LayerMask interactableLayer = -1;
        
        private void Reset()
        {
            SetupComponents();
        }
        
        [ContextMenu("Setup Player Components")]
        public void SetupComponents()
        {
            // Get or add core components
            playerController = GetComponent<PlayerController>() ?? gameObject.AddComponent<PlayerController>();
            playerInteractor = GetComponent<PlayerInteractor>() ?? gameObject.AddComponent<PlayerInteractor>();
            inventoryModule = GetComponent<PlayerInventoryModule>() ?? gameObject.AddComponent<PlayerInventoryModule>();
            
            // Setup physics
            rb2d = GetComponent<Rigidbody2D>();
            if (rb2d == null)
            {
                rb2d = gameObject.AddComponent<Rigidbody2D>();
                rb2d.gravityScale = 0f;
                rb2d.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            
            // Setup collider
            playerCollider = GetComponent<Collider2D>();
            if (playerCollider == null)
            {
                var capsuleCollider = gameObject.AddComponent<CapsuleCollider2D>();
                capsuleCollider.size = new Vector2(0.8f, 1.6f);
                capsuleCollider.offset = new Vector2(0f, 0.8f);
                playerCollider = capsuleCollider;
            }
            
            // Setup visual components
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                GameObject visualGO = new GameObject("Visual");
                visualGO.transform.SetParent(transform);
                visualGO.transform.localPosition = Vector3.zero;
                spriteRenderer = visualGO.AddComponent<SpriteRenderer>();
                spriteRenderer.sortingLayerName = "Player";
            }
            
            animator = GetComponentInChildren<Animator>();
            if (animator == null && spriteRenderer != null)
            {
                animator = spriteRenderer.gameObject.AddComponent<Animator>();
            }
            
            // Setup input
            playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                playerInput = gameObject.AddComponent<PlayerInput>();
            }
            
            // Configure PlayerInput
            if (inputActions != null)
            {
                playerInput.actions = inputActions;
            }
            playerInput.defaultActionMap = "Player";
            playerInput.defaultControlScheme = "Keyboard&Mouse";
            playerInput.notificationBehavior = PlayerNotifications.SendMessages;
            
            // Configure layers
            gameObject.layer = LayerMask.NameToLayer("Player");
            
            // Set tag
            gameObject.tag = "Player";
            
            Debug.Log("Player setup complete!");
        }
        
        private void Start()
        {
            ValidateSetup();
        }
        
        private void ValidateSetup()
        {
            if (playerController == null)
                Debug.LogError("PlayerController is missing!");
                
            if (playerInteractor == null)
                Debug.LogError("PlayerInteractor is missing!");
                
            if (rb2d == null)
                Debug.LogError("Rigidbody2D is missing!");
                
            if (playerCollider == null)
                Debug.LogError("Collider2D is missing!");
                
            if (spriteRenderer == null)
                Debug.LogWarning("SpriteRenderer is missing - player will be invisible!");
                
            if (playerInput == null || playerInput.actions == null)
                Debug.LogWarning("PlayerInput is not properly configured!");
        }
        
        public void SetPlayerSprite(Sprite sprite)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
            }
        }
        
        public void SetAnimatorController(RuntimeAnimatorController controller)
        {
            if (animator != null)
            {
                animator.runtimeAnimatorController = controller;
            }
        }
    }
}