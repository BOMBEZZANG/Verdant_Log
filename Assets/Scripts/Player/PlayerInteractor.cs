using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VerdantLog.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private LayerMask interactableLayer = -1;
        [SerializeField] private Transform interactionPoint;
        
        [Header("Detection")]
        [SerializeField] private bool useRaycast = false;
        [SerializeField] private float detectionRadius = 0.5f;
        
        
        [Header("Debug")]
        [SerializeField] private bool showDebugGizmos = true;
        
        public static event Action<Interactable> OnInteractableDetected;
        public static event Action OnInteractableExited;
        
        private Interactable currentInteractable;
        private PlayerController playerController;
        
        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            
            if (interactionPoint == null)
            {
                // Create interaction point as a child, but ensure it doesn't cause DontDestroyOnLoad warnings
                GameObject interactionObj = new GameObject("InteractionPoint");
                interactionObj.transform.SetParent(transform, false);
                interactionObj.transform.localPosition = Vector3.up * 0.5f;
                interactionPoint = interactionObj.transform;
            }
        }
        
        private void Update()
        {
            UpdateInteractionPoint();
            DetectInteractable();
        }
        
        private void UpdateInteractionPoint()
        {
            if (playerController != null && interactionPoint != null)
            {
                Vector2 moveDirection = playerController.GetMoveDirection();
                if (moveDirection.magnitude > 0.01f)
                {
                    interactionPoint.localPosition = new Vector3(moveDirection.x, moveDirection.y, 0) * interactionRange;
                }
            }
        }
        
        private void DetectInteractable()
        {
            Interactable detectedInteractable = null;
            
            if (useRaycast)
            {
                Vector2 direction = (interactionPoint.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, interactionRange, interactableLayer);
                
                if (hit.collider != null)
                {
                    detectedInteractable = hit.collider.GetComponent<Interactable>();
                }
            }
            else
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(interactionPoint.position, detectionRadius, interactableLayer);
                
                float closestDistance = float.MaxValue;
                foreach (var collider in colliders)
                {
                    Interactable interactable = collider.GetComponent<Interactable>();
                    if (interactable != null && interactable.IsInteractable())
                    {
                        float distance = Vector2.Distance(transform.position, collider.transform.position);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            detectedInteractable = interactable;
                        }
                    }
                }
            }
            
            if (detectedInteractable != currentInteractable)
            {
                if (currentInteractable != null)
                {
                    currentInteractable.OnPlayerExit();
                    OnInteractableExited?.Invoke();
                }
                
                currentInteractable = detectedInteractable;
                
                if (currentInteractable != null)
                {
                    currentInteractable.OnPlayerEnter();
                    OnInteractableDetected?.Invoke(currentInteractable);
                }
            }
        }
        
        // Unity Input System message method
        public void OnInteract(InputValue value)
        {
            if (value.isPressed && currentInteractable != null && currentInteractable.IsInteractable())
            {
                currentInteractable.TriggerInteraction();
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!showDebugGizmos) return;
            
            if (interactionPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(interactionPoint.position, detectionRadius);
            }
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
            
            if (currentInteractable != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentInteractable.transform.position);
            }
        }
        
        public Interactable GetCurrentInteractable()
        {
            return currentInteractable;
        }
        
        public bool HasInteractable()
        {
            return currentInteractable != null && currentInteractable.IsInteractable();
        }
    }
}