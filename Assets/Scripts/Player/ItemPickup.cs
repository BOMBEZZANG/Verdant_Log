using UnityEngine;
using VerdantLog.Data;
using VerdantLog.Systems;

namespace VerdantLog.Player
{
    [RequireComponent(typeof(Interactable))]
    public class ItemPickup : MonoBehaviour
    {
        [Header("Item Settings")]
        [SerializeField] private ItemData itemData;
        [SerializeField] private int quantity = 1;
        [SerializeField] private bool destroyOnPickup = true;
        
        [Header("Visual")]
        [SerializeField] private SpriteRenderer itemSpriteRenderer;
        [SerializeField] private float bobHeight = 0.1f;
        [SerializeField] private float bobSpeed = 2f;
        
        private Interactable interactable;
        private Vector3 startPosition;
        private float bobTimer = 0f;
        
        private void Awake()
        {
            interactable = GetComponent<Interactable>();
            
            if (itemSpriteRenderer == null)
            {
                itemSpriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            startPosition = transform.position;
            
            UpdateInteractable();
        }
        
        private void Start()
        {
            if (itemData != null && itemSpriteRenderer != null && itemData.Icon != null)
            {
                itemSpriteRenderer.sprite = itemData.Icon;
            }
        }
        
        private void Update()
        {
            if (bobHeight > 0)
            {
                bobTimer += Time.deltaTime * bobSpeed;
                float newY = startPosition.y + Mathf.Sin(bobTimer) * bobHeight;
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
        
        private void UpdateInteractable()
        {
            if (interactable != null && itemData != null)
            {
                interactable.SetInteractionName($"Pick up {itemData.ItemName}");
                
                // Try to automatically connect using reflection
                TryAutoConnectInteraction();
                
                Debug.Log($"ItemPickup: Set interaction name for {itemData.ItemName}");
            }
        }
        
        private void TryAutoConnectInteraction()
        {
            if (interactable == null) return;
            
            try
            {
                // Use reflection to access the private onInteract field
                var interactableType = typeof(Interactable);
                var onInteractField = interactableType.GetField("onInteract", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (onInteractField != null)
                {
                    var onInteractEvent = onInteractField.GetValue(interactable) as UnityEngine.Events.UnityEvent;
                    if (onInteractEvent != null)
                    {
                        // Only add if not already connected
                        onInteractEvent.RemoveListener(OnPickup); // Remove any duplicates first
                        onInteractEvent.AddListener(OnPickup);
                        Debug.Log($"ItemPickup: Auto-connected interaction for {itemData.ItemName}");
                        return;
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not auto-connect interaction: {e.Message}");
            }
            
            Debug.LogWarning($"ItemPickup: Could not auto-connect. Please manually connect OnPickup() method in the Interactable's OnInteract event in the Inspector!");
        }
        
        public void OnPickup()
        {
            Debug.Log($"ItemPickup.OnPickup() called for {gameObject.name}");
            
            if (itemData == null)
            {
                Debug.LogWarning("ItemPickup has no ItemData assigned!");
                return;
            }
            
            var inventory = Inventory.Instance;
            if (inventory == null)
            {
                Debug.LogError("Inventory system not found!");
                return;
            }
            
            Debug.Log($"Attempting to add {quantity}x {itemData.ItemName} to inventory");
            
            if (inventory.AddItem(itemData.ItemID, quantity))
            {
                Debug.Log($"Successfully added item to inventory");
                Core.GameEvents.TriggerNotification($"Picked up {quantity}x {itemData.ItemName}");
                
                if (destroyOnPickup)
                {
                    Debug.Log($"Destroying pickup object");
                    Destroy(gameObject);
                }
                else
                {
                    Debug.Log($"Deactivating pickup object");
                    gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("Failed to add item - inventory full?");
                Core.GameEvents.TriggerNotification("Inventory is full!");
            }
        }
        
        public void SetItem(ItemData item, int amount = 1)
        {
            itemData = item;
            quantity = amount;
            
            if (itemSpriteRenderer != null && itemData != null && itemData.Icon != null)
            {
                itemSpriteRenderer.sprite = itemData.Icon;
            }
            
            UpdateInteractable();
        }
        
        public ItemData GetItemData()
        {
            return itemData;
        }
        
        public int GetQuantity()
        {
            return quantity;
        }
    }
}