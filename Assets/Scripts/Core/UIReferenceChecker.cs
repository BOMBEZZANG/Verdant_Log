using UnityEngine;
using VerdantLog.UI;

namespace VerdantLog.Core
{
    public class UIReferenceChecker : MonoBehaviour
    {
        [Header("Debug - Check UI References")]
        [SerializeField] private bool checkOnStart = true;
        
        private void Start()
        {
            if (checkOnStart)
            {
                CheckUIReferences();
            }
        }
        
        [ContextMenu("Check UI References")]
        public void CheckUIReferences()
        {
            Debug.Log("=== UI Reference Check ===");
            
            // Check InventoryUI
            var inventoryUI = FindObjectOfType<InventoryUI>();
            if (inventoryUI != null)
            {
                Debug.Log($"✓ InventoryUI found on GameObject: {inventoryUI.gameObject.name}");
                
                // Use reflection to check the inventoryPanel field
                var inventoryPanelField = typeof(InventoryUI).GetField("inventoryPanel", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (inventoryPanelField != null)
                {
                    var inventoryPanel = inventoryPanelField.GetValue(inventoryUI) as GameObject;
                    if (inventoryPanel != null)
                    {
                        Debug.Log($"✓ InventoryUI.inventoryPanel assigned: {inventoryPanel.name}");
                        Debug.Log($"  - Active: {inventoryPanel.activeSelf}");
                        Debug.Log($"  - ActiveInHierarchy: {inventoryPanel.activeInHierarchy}");
                    }
                    else
                    {
                        Debug.LogError("✗ InventoryUI.inventoryPanel is NULL! Please assign it in the Inspector.");
                    }
                }
            }
            else
            {
                Debug.LogError("✗ InventoryUI not found in scene!");
            }
            
            // Check EncyclopediaUI
            var encyclopediaUI = FindObjectOfType<EncyclopediaUI>();
            if (encyclopediaUI != null)
            {
                Debug.Log($"✓ EncyclopediaUI found on GameObject: {encyclopediaUI.gameObject.name}");
                
                // Use reflection to check the encyclopediaPanel field
                var encyclopediaPanelField = typeof(EncyclopediaUI).GetField("encyclopediaPanel", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (encyclopediaPanelField != null)
                {
                    var encyclopediaPanel = encyclopediaPanelField.GetValue(encyclopediaUI) as GameObject;
                    if (encyclopediaPanel != null)
                    {
                        Debug.Log($"✓ EncyclopediaUI.encyclopediaPanel assigned: {encyclopediaPanel.name}");
                        Debug.Log($"  - Active: {encyclopediaPanel.activeSelf}");
                        Debug.Log($"  - ActiveInHierarchy: {encyclopediaPanel.activeInHierarchy}");
                    }
                    else
                    {
                        Debug.LogError("✗ EncyclopediaUI.encyclopediaPanel is NULL! Please assign it in the Inspector.");
                    }
                }
            }
            else
            {
                Debug.LogError("✗ EncyclopediaUI not found in scene!");
            }
            
            Debug.Log("=== End UI Reference Check ===");
        }
    }
}