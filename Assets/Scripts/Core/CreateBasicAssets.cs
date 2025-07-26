using UnityEngine;
using VerdantLog.Data;
using VerdantLog.Systems;

namespace VerdantLog.Core
{
    public class CreateBasicAssets : MonoBehaviour
    {
        [ContextMenu("Create Basic Test Assets")]
        public void CreateTestAssets()
        {
            Debug.Log("Creating basic test assets...");
            
            // Create basic seed item
            var basicSeed = ScriptableObject.CreateInstance<ItemData>();
            basicSeed.name = "seed_basic";
            
            // Use reflection to set private fields
            var itemIDField = typeof(ItemData).GetField("itemID", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var itemNameField = typeof(ItemData).GetField("itemName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var descriptionField = typeof(ItemData).GetField("description", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var itemTypeField = typeof(ItemData).GetField("itemType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            itemIDField?.SetValue(basicSeed, "seed_basic");
            itemNameField?.SetValue(basicSeed, "Basic Seed");
            descriptionField?.SetValue(basicSeed, "A common seed found in the forest");
            itemTypeField?.SetValue(basicSeed, ItemType.Seed);
            
            // Add to ItemDatabase
            var itemDatabase = ItemDatabase.Instance;
            var allItemsField = typeof(ItemDatabase).GetField("allItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (allItemsField != null)
            {
                var allItems = (System.Collections.Generic.List<ItemData>)allItemsField.GetValue(itemDatabase);
                allItems.Add(basicSeed);
                itemDatabase.RefreshDatabase();
            }
            
            Debug.Log("Basic seed created and added to database");
        }
        
        [ContextMenu("Test Add Seed")]
        public void TestAddSeed()
        {
            bool result = Inventory.Instance.AddItem("seed_basic", 5);
            Debug.Log($"Add seed result: {result}");
        }
    }
}