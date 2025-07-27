using UnityEngine;
using UnityEngine.Events;

namespace VerdantLog.Player
{
    public class Interactable : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private string interactionName = "Interact";
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private bool requiresItem = false;
        [SerializeField] private string requiredItemID = "";
        
        [Header("Visual Feedback")]
        [SerializeField] private GameObject highlightObject;
        [SerializeField] private Color highlightColor = Color.yellow;
        
        [Header("Events")]
        [SerializeField] private UnityEvent onInteract;
        [SerializeField] private UnityEvent onPlayerEnter;
        [SerializeField] private UnityEvent onPlayerExit;
        [SerializeField] private UnityEvent<string> onInteractWithData;
        
        private SpriteRenderer spriteRenderer;
        private Color originalColor;
        private bool playerInRange = false;
        
        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
            }
            
            if (highlightObject != null)
            {
                highlightObject.SetActive(false);
            }
        }
        
        public void TriggerInteraction()
        {
            if (!isInteractable)
                return;
                
            if (requiresItem && !string.IsNullOrEmpty(requiredItemID))
            {
                var inventory = Systems.Inventory.Instance;
                if (inventory == null || !inventory.HasItem(requiredItemID))
                {
                    Core.GameEvents.TriggerNotification($"Requires {requiredItemID} to interact");
                    return;
                }
            }
            
            onInteract?.Invoke();
            
            if (!string.IsNullOrEmpty(interactionName))
            {
                onInteractWithData?.Invoke(interactionName);
            }
            
            SendInteractionEvent();
        }
        
        private void SendInteractionEvent()
        {
            switch (tag)
            {
                case "Plant":
                    if (TryGetComponent<PlantInteractable>(out var plant))
                    {
                        plant.HandleInteraction();
                    }
                    break;
                    
                case "NPC":
                    Core.GameEvents.TriggerNotification("NPC interaction not yet implemented");
                    break;
                    
                case "Door":
                    Core.GameEvents.TriggerNotification("Door interaction not yet implemented");
                    break;
                    
                default:
                    Debug.Log($"Interacted with {gameObject.name}");
                    break;
            }
        }
        
        public void OnPlayerEnter()
        {
            if (!isInteractable)
                return;
                
            playerInRange = true;
            
            if (highlightObject != null)
            {
                highlightObject.SetActive(true);
            }
            else if (spriteRenderer != null)
            {
                spriteRenderer.color = highlightColor;
            }
            
            onPlayerEnter?.Invoke();
        }
        
        public void OnPlayerExit()
        {
            playerInRange = false;
            
            if (highlightObject != null)
            {
                highlightObject.SetActive(false);
            }
            else if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }
            
            onPlayerExit?.Invoke();
        }
        
        public void SetInteractable(bool value)
        {
            isInteractable = value;
            
            if (!isInteractable && playerInRange)
            {
                OnPlayerExit();
            }
        }
        
        public bool IsInteractable()
        {
            return isInteractable;
        }
        
        public string GetInteractionName()
        {
            return interactionName;
        }
        
        public void SetInteractionName(string name)
        {
            interactionName = name;
        }
        
        public bool RequiresItem()
        {
            return requiresItem && !string.IsNullOrEmpty(requiredItemID);
        }
        
        public string GetRequiredItemID()
        {
            return requiredItemID;
        }
    }
}