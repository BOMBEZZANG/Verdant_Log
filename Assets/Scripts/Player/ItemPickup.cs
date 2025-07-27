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
                
                // The onInteract event is serialized, so we'll set it up in the Unity Editor
                // or use a different approach to connect the interaction
            }
        }
        
        public void OnPickup()
        {
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
            
            if (inventory.AddItem(itemData.ItemID, quantity))
            {
                Core.GameEvents.TriggerNotification($"Picked up {quantity}x {itemData.ItemName}");
                
                if (destroyOnPickup)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
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