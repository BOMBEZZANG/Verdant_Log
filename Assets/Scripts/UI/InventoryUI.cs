using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VerdantLog.Core;
using VerdantLog.Systems;

namespace VerdantLog.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private Transform itemsContainer;
        [SerializeField] private GameObject itemSlotPrefab;
        [SerializeField] private TextMeshProUGUI slotsText;
        
        
        private List<GameObject> itemSlots = new List<GameObject>();
        
        private void Start()
        {
            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);
                
            GameEvents.OnInventoryUpdated += RefreshInventory;
            RefreshInventory();
        }
        
        private void OnDestroy()
        {
            GameEvents.OnInventoryUpdated -= RefreshInventory;
        }
        
        
        public void ToggleInventory()
        {
            Debug.Log("InventoryUI: ToggleInventory() called");
            if (inventoryPanel != null)
            {
                bool newState = !inventoryPanel.activeSelf;
                inventoryPanel.SetActive(newState);
                Debug.Log($"InventoryUI: Panel set to {(newState ? "active" : "inactive")}");
                if (inventoryPanel.activeSelf)
                {
                    RefreshInventory();
                }
            }
            else
            {
                Debug.LogWarning("InventoryUI: inventoryPanel is null!");
            }
        }
        
        private void RefreshInventory()
        {
            // Clear existing slots
            foreach (var slot in itemSlots)
            {
                Destroy(slot);
            }
            itemSlots.Clear();
            
            // Get inventory items
            var items = Inventory.Instance.GetAllItems();
            
            // Create slots for each item
            foreach (var item in items)
            {
                if (itemSlotPrefab != null && itemsContainer != null)
                {
                    GameObject slot = Instantiate(itemSlotPrefab, itemsContainer);
                    itemSlots.Add(slot);
                    
                    // Update slot display
                    var itemData = ItemDatabase.Instance.GetItem(item.itemID);
                    if (itemData != null)
                    {
                        TextMeshProUGUI nameText = slot.GetComponentInChildren<TextMeshProUGUI>();
                        if (nameText != null)
                        {
                            nameText.text = $"{itemData.ItemName} x{item.quantity}";
                        }
                        
                        Image icon = slot.transform.Find("Icon")?.GetComponent<Image>();
                        if (icon != null && itemData.Icon != null)
                        {
                            icon.sprite = itemData.Icon;
                        }
                    }
                }
            }
            
            // Update slots text
            if (slotsText != null)
            {
                slotsText.text = $"Slots: {Inventory.Instance.GetUsedSlots()}/{Inventory.Instance.GetMaxSlots()}";
            }
        }
    }
}