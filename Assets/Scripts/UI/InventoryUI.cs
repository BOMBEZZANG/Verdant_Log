using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
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
        
        [Header("Input")]
        [SerializeField] private InputActionReference toggleInventoryAction;
        
        private List<GameObject> itemSlots = new List<GameObject>();
        
        private void Start()
        {
            if (inventoryPanel != null)
                inventoryPanel.SetActive(false);
                
            GameEvents.OnInventoryUpdated += RefreshInventory;
            RefreshInventory();
            
            if (toggleInventoryAction != null)
            {
                toggleInventoryAction.action.performed += OnToggleInventory;
                toggleInventoryAction.action.Enable();
            }
        }
        
        private void OnDestroy()
        {
            GameEvents.OnInventoryUpdated -= RefreshInventory;
            
            if (toggleInventoryAction != null)
            {
                toggleInventoryAction.action.performed -= OnToggleInventory;
                toggleInventoryAction.action.Disable();
            }
        }
        
        private void OnToggleInventory(InputAction.CallbackContext context)
        {
            ToggleInventory();
        }
        
        public void ToggleInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
                if (inventoryPanel.activeSelf)
                {
                    RefreshInventory();
                }
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