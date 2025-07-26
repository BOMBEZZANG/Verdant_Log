using UnityEngine;
using VerdantLog.UI;

namespace VerdantLog.Core
{
    public class UILinkFixer : MonoBehaviour
    {
        [Header("UI Link Fixer")]
        public bool debugMode = true;
        
        [ContextMenu("Fix UI Component Links")]
        public void FixUIComponentLinks()
        {
            Debug.Log("=== FIXING UI LINKS ===");
            
            // Fix InventoryUI
            InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null)
            {
                GameObject inventoryPanel = inventoryUI.gameObject;
                
                // Use reflection to set the private inventoryPanel field
                var inventoryPanelField = typeof(InventoryUI).GetField("inventoryPanel", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (inventoryPanelField != null)
                {
                    inventoryPanelField.SetValue(inventoryUI, inventoryPanel);
                    Debug.Log($"✓ InventoryUI linked to panel: {inventoryPanel.name}");
                }
                else
                {
                    Debug.LogError("Could not find inventoryPanel field in InventoryUI");
                }
            }
            else
            {
                Debug.LogError("No InventoryUI component found!");
            }
            
            // Fix EncyclopediaUI
            EncyclopediaUI encyclopediaUI = FindObjectOfType<EncyclopediaUI>();
            if (encyclopediaUI != null)
            {
                GameObject encyclopediaPanel = encyclopediaUI.gameObject;
                
                // Use reflection to set the private encyclopediaPanel field
                var encyclopediaPanelField = typeof(EncyclopediaUI).GetField("encyclopediaPanel", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (encyclopediaPanelField != null)
                {
                    encyclopediaPanelField.SetValue(encyclopediaUI, encyclopediaPanel);
                    Debug.Log($"✓ EncyclopediaUI linked to panel: {encyclopediaPanel.name}");
                }
                else
                {
                    Debug.LogError("Could not find encyclopediaPanel field in EncyclopediaUI");
                }
            }
            else
            {
                Debug.LogError("No EncyclopediaUI component found!");
            }
            
            Debug.Log("=== UI LINKS FIXED ===");
        }
        
        [ContextMenu("Test UI Toggle")]
        public void TestUIToggle()
        {
            Debug.Log("=== TESTING UI TOGGLE ===");
            
            InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null)
            {
                Debug.Log("Calling InventoryUI.ToggleInventory()");
                inventoryUI.ToggleInventory();
                
                // Check if panel is now active
                GameObject panel = inventoryUI.gameObject;
                Debug.Log($"Inventory panel active after toggle: {panel.activeInHierarchy}");
            }
            
            EncyclopediaUI encyclopediaUI = FindObjectOfType<EncyclopediaUI>();
            if (encyclopediaUI != null)
            {
                Debug.Log("Calling EncyclopediaUI.ToggleEncyclopedia()");
                encyclopediaUI.ToggleEncyclopedia();
                
                // Check if panel is now active
                GameObject panel = encyclopediaUI.gameObject;
                Debug.Log($"Encyclopedia panel active after toggle: {panel.activeInHierarchy}");
            }
        }
        
        [ContextMenu("Manual Show Panels")]
        public void ManualShowPanels()
        {
            Debug.Log("=== MANUALLY SHOWING PANELS ===");
            
            // Find and show inventory panel
            GameObject invPanel = GameObject.Find("Test Inventory Panel");
            if (invPanel != null)
            {
                invPanel.SetActive(true);
                Debug.Log("✓ Inventory panel manually activated");
            }
            else
            {
                Debug.LogError("Could not find Test Inventory Panel");
            }
            
            // Find and show encyclopedia panel
            GameObject encPanel = GameObject.Find("Test Encyclopedia Panel");
            if (encPanel != null)
            {
                encPanel.SetActive(true);
                Debug.Log("✓ Encyclopedia panel manually activated");
            }
            else
            {
                Debug.LogError("Could not find Test Encyclopedia Panel");
            }
        }
        
        [ContextMenu("Manual Hide Panels")]
        public void ManualHidePanels()
        {
            // Find and hide panels
            GameObject invPanel = GameObject.Find("Test Inventory Panel");
            if (invPanel != null)
            {
                invPanel.SetActive(false);
                Debug.Log("✓ Inventory panel manually deactivated");
            }
            
            GameObject encPanel = GameObject.Find("Test Encyclopedia Panel");
            if (encPanel != null)
            {
                encPanel.SetActive(false);
                Debug.Log("✓ Encyclopedia panel manually deactivated");
            }
        }
    }
}