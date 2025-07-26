using System.Collections.Generic;
using UnityEngine;
using VerdantLog.Data;

namespace VerdantLog.Systems
{
    public class ItemDatabase : MonoBehaviour
    {
        private static ItemDatabase instance;
        public static ItemDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ItemDatabase>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("ItemDatabase");
                        instance = go.AddComponent<ItemDatabase>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        [SerializeField] private List<ItemData> allItems = new List<ItemData>();
        private Dictionary<string, ItemData> itemLookup = new Dictionary<string, ItemData>();
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeDatabase();
        }
        
        private void InitializeDatabase()
        {
            itemLookup.Clear();
            
            foreach (var item in allItems)
            {
                if (item != null && !string.IsNullOrEmpty(item.ItemID))
                {
                    if (itemLookup.ContainsKey(item.ItemID))
                    {
                        Debug.LogWarning($"Duplicate ItemID found: {item.ItemID}");
                    }
                    else
                    {
                        itemLookup.Add(item.ItemID, item);
                    }
                }
            }
            
            ItemData[] resourceItems = Resources.LoadAll<ItemData>("Items");
            foreach (var item in resourceItems)
            {
                if (!itemLookup.ContainsKey(item.ItemID))
                {
                    itemLookup.Add(item.ItemID, item);
                }
            }
            
            Debug.Log($"ItemDatabase initialized with {itemLookup.Count} items");
        }
        
        public ItemData GetItem(string itemID)
        {
            if (string.IsNullOrEmpty(itemID))
                return null;
                
            if (itemLookup.TryGetValue(itemID, out ItemData item))
            {
                return item;
            }
            
            Debug.LogWarning($"Item with ID '{itemID}' not found in database");
            return null;
        }
        
        public bool ItemExists(string itemID)
        {
            return !string.IsNullOrEmpty(itemID) && itemLookup.ContainsKey(itemID);
        }
        
        public void RefreshDatabase()
        {
            InitializeDatabase();
        }
    }
}