using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VerdantLog.Core;
using VerdantLog.Systems;

namespace VerdantLog.UI
{
    public class SimpleInventoryUI : MonoBehaviour
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
            {
                inventoryPanel.SetActive(false);
                Debug.Log($"[SimpleInventoryUI] Panel assigned: {inventoryPanel.name}");
            }
            else
            {
                Debug.LogError("[SimpleInventoryUI] inventoryPanel is NULL! Please assign it in Inspector!");
            }
                
            GameEvents.OnInventoryUpdated += RefreshInventory;
            RefreshInventory();
        }
        
        private void OnDestroy()
        {
            GameEvents.OnInventoryUpdated -= RefreshInventory;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("[SimpleInventoryUI] I key pressed!");
                ToggleInventory();
            }
        }
        
        public void ToggleInventory()
        {
            if (inventoryPanel != null)
            {
                bool newState = !inventoryPanel.activeSelf;
                inventoryPanel.SetActive(newState);
                Debug.Log($"[SimpleInventoryUI] Panel toggled to: {newState}");
                
                if (newState)
                {
                    RefreshInventory();
                }
            }
            else
            {
                Debug.LogError("[SimpleInventoryUI] Cannot toggle - inventoryPanel is NULL!");
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