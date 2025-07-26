using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VerdantLog.Core;
using VerdantLog.Data;

namespace VerdantLog.Systems
{
    public class Inventory : MonoBehaviour
    {
        private static Inventory instance;
        public static Inventory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Inventory>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("Inventory");
                        instance = go.AddComponent<Inventory>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        public static event Action OnInventoryUpdated;
        
        [SerializeField] private int maxSlots = 30;
        [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        public bool AddItem(string itemID, int quantity)
        {
            if (string.IsNullOrEmpty(itemID) || quantity <= 0)
                return false;
                
            ItemData itemData = ItemDatabase.Instance.GetItem(itemID);
            if (itemData == null)
            {
                Debug.LogWarning($"Cannot add item: {itemID} not found in database");
                return false;
            }
            
            int remainingQuantity = quantity;
            
            if (itemData.IsStackable)
            {
                var existingStacks = items.Where(i => i.itemID == itemID && i.quantity < itemData.MaxStackSize).ToList();
                
                foreach (var stack in existingStacks)
                {
                    int spaceInStack = itemData.MaxStackSize - stack.quantity;
                    int amountToAdd = Mathf.Min(spaceInStack, remainingQuantity);
                    stack.quantity += amountToAdd;
                    remainingQuantity -= amountToAdd;
                    
                    if (remainingQuantity <= 0)
                        break;
                }
            }
            
            while (remainingQuantity > 0 && items.Count < maxSlots)
            {
                int stackSize = itemData.IsStackable ? Mathf.Min(remainingQuantity, itemData.MaxStackSize) : 1;
                items.Add(new InventoryItem(itemID, stackSize));
                remainingQuantity -= stackSize;
            }
            
            if (remainingQuantity > 0)
            {
                Debug.LogWarning($"Inventory full! Could not add {remainingQuantity} {itemData.ItemName}");
                OnInventoryUpdated?.Invoke();
                return false;
            }
            
            OnInventoryUpdated?.Invoke();
            return true;
        }
        
        public bool RemoveItem(string itemID, int quantity)
        {
            if (string.IsNullOrEmpty(itemID) || quantity <= 0)
                return false;
                
            if (!HasItem(itemID, quantity))
                return false;
                
            int remainingToRemove = quantity;
            var itemsToRemove = new List<InventoryItem>();
            
            foreach (var item in items.Where(i => i.itemID == itemID))
            {
                if (remainingToRemove <= 0)
                    break;
                    
                if (item.quantity <= remainingToRemove)
                {
                    remainingToRemove -= item.quantity;
                    itemsToRemove.Add(item);
                }
                else
                {
                    item.quantity -= remainingToRemove;
                    remainingToRemove = 0;
                }
            }
            
            foreach (var item in itemsToRemove)
            {
                items.Remove(item);
            }
            
            OnInventoryUpdated?.Invoke();
            return true;
        }
        
        public bool HasItem(string itemID, int quantity = 1)
        {
            if (string.IsNullOrEmpty(itemID) || quantity <= 0)
                return false;
                
            int totalQuantity = items.Where(i => i.itemID == itemID).Sum(i => i.quantity);
            return totalQuantity >= quantity;
        }
        
        public int GetItemCount(string itemID)
        {
            if (string.IsNullOrEmpty(itemID))
                return 0;
                
            return items.Where(i => i.itemID == itemID).Sum(i => i.quantity);
        }
        
        public List<InventoryItem> GetAllItems()
        {
            return new List<InventoryItem>(items);
        }
        
        public void Clear()
        {
            items.Clear();
            OnInventoryUpdated?.Invoke();
        }
        
        public bool IsFull()
        {
            return items.Count >= maxSlots;
        }
        
        public int GetUsedSlots()
        {
            return items.Count;
        }
        
        public int GetMaxSlots()
        {
            return maxSlots;
        }
    }
}