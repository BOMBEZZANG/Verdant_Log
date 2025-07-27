using UnityEngine;
using UnityEngine.InputSystem;
using VerdantLog.Systems;
using VerdantLog.Data;
using VerdantLog.Core;

namespace VerdantLog.Player
{
    public class PlayerInventoryModule : MonoBehaviour
    {
        
        [Header("Item Drop Settings")]
        [SerializeField] private GameObject itemDropPrefab;
        [SerializeField] private float dropForce = 5f;
        [SerializeField] private float dropOffset = 1f;
        
        private PlayerController playerController;
        private Inventory inventory;
        
        private void Awake()
        {
            playerController = GetComponent<PlayerController>();
            inventory = Inventory.Instance;
        }
        
        // Unity Input System message methods
        public void OnToggleInventory(InputValue value)
        {
            if (value.isPressed)
            {
                Debug.Log("PlayerInventoryModule: Toggle inventory input received");
                var inventoryUI = FindObjectOfType<UI.InventoryUI>();
                if (inventoryUI != null)
                {
                    Debug.Log("PlayerInventoryModule: Found InventoryUI, calling ToggleInventory()");
                    inventoryUI.ToggleInventory();
                }
                else
                {
                    Debug.LogWarning("PlayerInventoryModule: No InventoryUI found in scene!");
                }
            }
        }
        
        public void OnDropItem(InputValue value)
        {
            if (value.isPressed && inventory != null && itemDropPrefab != null)
            {
                var selectedItem = GetSelectedInventoryItem();
                if (selectedItem != null)
                {
                    DropItem(selectedItem, 1);
                }
            }
        }
        
        public void OnUseItem(InputValue value)
        {
            if (value.isPressed && inventory != null)
            {
                var selectedItem = GetSelectedInventoryItem();
                if (selectedItem != null)
                {
                    UseItem(selectedItem);
                }
            }
        }
        
        private InventoryItem GetSelectedInventoryItem()
        {
            // TODO: Implement UI selection system
            // For now, return the first item in inventory
            if (inventory != null)
            {
                var items = inventory.GetAllItems();
                if (items != null && items.Count > 0)
                {
                    return items[0];
                }
            }
            return null;
        }
        
        private void DropItem(InventoryItem inventoryItem, int quantity)
        {
            if (inventoryItem == null || quantity <= 0)
                return;
                
            // Get the actual ItemData from the database
            var itemData = ItemDatabase.Instance.GetItem(inventoryItem.itemID);
            if (itemData == null)
                return;
                
            if (inventory.RemoveItem(inventoryItem.itemID, quantity))
            {
                Vector2 dropDirection = playerController != null ? playerController.GetMoveDirection() : Vector2.down;
                if (dropDirection.magnitude < 0.01f)
                {
                    dropDirection = Vector2.down;
                }
                
                Vector3 dropPosition = transform.position + (Vector3)(dropDirection.normalized * dropOffset);
                
                GameObject droppedItem = Instantiate(itemDropPrefab, dropPosition, Quaternion.identity);
                
                ItemPickup pickup = droppedItem.GetComponent<ItemPickup>();
                if (pickup == null)
                {
                    pickup = droppedItem.AddComponent<ItemPickup>();
                }
                
                pickup.SetItem(itemData, quantity);
                
                Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    rb = droppedItem.AddComponent<Rigidbody2D>();
                }
                
                rb.AddForce(dropDirection.normalized * dropForce, ForceMode2D.Impulse);
                
                Core.GameEvents.TriggerNotification($"Dropped {quantity}x {itemData.ItemName}");
            }
        }
        
        private void UseItem(InventoryItem inventoryItem)
        {
            if (inventoryItem == null)
                return;
                
            var itemData = ItemDatabase.Instance.GetItem(inventoryItem.itemID);
            if (itemData == null)
                return;
                
            switch (itemData.Type)
            {
                case ItemType.Seed:
                    Core.GameEvents.TriggerNotification("Find a cultivation spot to plant seeds");
                    break;
                    
                case ItemType.Material:
                    Core.GameEvents.TriggerNotification($"Cannot use {itemData.ItemName} directly");
                    break;
                    
                case ItemType.Tool:
                    Core.GameEvents.TriggerNotification($"Tools are used automatically when needed");
                    break;
                    
                default:
                    Core.GameEvents.TriggerNotification($"Cannot use {itemData.ItemName}");
                    break;
            }
        }
        
        public bool TryGetItem(string itemID, out InventoryItem item)
        {
            item = null;
            if (inventory == null)
                return false;
                
            var items = inventory.GetAllItems();
            foreach (var invItem in items)
            {
                if (invItem.itemID == itemID)
                {
                    item = invItem;
                    return true;
                }
            }
            
            return false;
        }
        
        public bool HasItem(string itemID, int quantity = 1)
        {
            return inventory != null && inventory.HasItem(itemID, quantity);
        }
    }
}