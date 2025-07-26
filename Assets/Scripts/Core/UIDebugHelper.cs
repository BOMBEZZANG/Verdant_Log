using UnityEngine;
using VerdantLog.UI;

namespace VerdantLog.Core
{
    public class UIDebugHelper : MonoBehaviour
    {
        [Header("UI Debug Helper")]
        public bool debugMode = true;
        
        [ContextMenu("Debug UI Components")]
        public void DebugUIComponents()
        {
            Debug.Log("=== UI DEBUG ===");
            
            // Check for InventoryUI components
            InventoryUI[] inventoryUIs = FindObjectsOfType<InventoryUI>();
            Debug.Log($"Found {inventoryUIs.Length} InventoryUI components");
            
            foreach (var invUI in inventoryUIs)
            {
                Debug.Log($"InventoryUI on: {invUI.gameObject.name}");
                Debug.Log($"GameObject active: {invUI.gameObject.activeInHierarchy}");
                
                // Check if the panel reference exists
                var panelField = typeof(InventoryUI).GetField("inventoryPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var panel = panelField?.GetValue(invUI) as GameObject;
                
                if (panel != null)
                {
                    Debug.Log($"Panel reference: {panel.name}, Active: {panel.activeInHierarchy}");
                }
                else
                {
                    Debug.LogWarning("InventoryUI has no panel reference!");
                }
            }
            
            // Check for EncyclopediaUI components
            EncyclopediaUI[] encyclopediaUIs = FindObjectsOfType<EncyclopediaUI>();
            Debug.Log($"Found {encyclopediaUIs.Length} EncyclopediaUI components");
            
            foreach (var encUI in encyclopediaUIs)
            {
                Debug.Log($"EncyclopediaUI on: {encUI.gameObject.name}");
                Debug.Log($"GameObject active: {encUI.gameObject.activeInHierarchy}");
                
                // Check if the panel reference exists
                var panelField = typeof(EncyclopediaUI).GetField("encyclopediaPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var panel = panelField?.GetValue(encUI) as GameObject;
                
                if (panel != null)
                {
                    Debug.Log($"Panel reference: {panel.name}, Active: {panel.activeInHierarchy}");
                }
                else
                {
                    Debug.LogWarning("EncyclopediaUI has no panel reference!");
                }
            }
            
            Debug.Log("=== END UI DEBUG ===");
        }
        
        [ContextMenu("Force Toggle Inventory")]
        public void ForceToggleInventory()
        {
            InventoryUI invUI = FindObjectOfType<InventoryUI>();
            if (invUI != null)
            {
                Debug.Log("Found InventoryUI, calling ToggleInventory()");
                invUI.ToggleInventory();
            }
            else
            {
                Debug.LogError("No InventoryUI found!");
            }
        }
        
        [ContextMenu("Force Toggle Encyclopedia")]
        public void ForceToggleEncyclopedia()
        {
            EncyclopediaUI encUI = FindObjectOfType<EncyclopediaUI>();
            if (encUI != null)
            {
                Debug.Log("Found EncyclopediaUI, calling ToggleEncyclopedia()");
                encUI.ToggleEncyclopedia();
            }
            else
            {
                Debug.LogError("No EncyclopediaUI found!");
            }
        }
        
        [ContextMenu("Create Simple Test Panels")]
        public void CreateSimpleTestPanels()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found!");
                return;
            }
            
            // Create simple inventory panel
            GameObject invPanel = new GameObject("Test Inventory Panel");
            invPanel.transform.SetParent(canvas.transform, false);
            
            var invRect = invPanel.AddComponent<RectTransform>();
            invRect.anchorMin = new Vector2(0.1f, 0.1f);
            invRect.anchorMax = new Vector2(0.9f, 0.9f);
            invRect.sizeDelta = Vector2.zero;
            invRect.anchoredPosition = Vector2.zero;
            
            var invImage = invPanel.AddComponent<UnityEngine.UI.Image>();
            invImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            // Add text
            GameObject invText = new GameObject("Inventory Text");
            invText.transform.SetParent(invPanel.transform, false);
            var invTextComponent = invText.AddComponent<TMPro.TextMeshProUGUI>();
            invTextComponent.text = "TEST INVENTORY PANEL\nPress I to close";
            invTextComponent.fontSize = 24;
            invTextComponent.color = Color.white;
            invTextComponent.alignment = TMPro.TextAlignmentOptions.Center;
            
            var textRect = invText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            // Add InventoryUI component and set it up
            var invUI = invPanel.AddComponent<InventoryUI>();
            var panelField = typeof(InventoryUI).GetField("inventoryPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            panelField?.SetValue(invUI, invPanel);
            
            invPanel.SetActive(false);
            
            // Create simple encyclopedia panel
            GameObject encPanel = new GameObject("Test Encyclopedia Panel");
            encPanel.transform.SetParent(canvas.transform, false);
            
            var encRect = encPanel.AddComponent<RectTransform>();
            encRect.anchorMin = new Vector2(0.1f, 0.1f);
            encRect.anchorMax = new Vector2(0.9f, 0.9f);
            encRect.sizeDelta = Vector2.zero;
            encRect.anchoredPosition = Vector2.zero;
            
            var encImage = encPanel.AddComponent<UnityEngine.UI.Image>();
            encImage.color = new Color(0.1f, 0.3f, 0.1f, 0.8f);
            
            // Add text
            GameObject encText = new GameObject("Encyclopedia Text");
            encText.transform.SetParent(encPanel.transform, false);
            var encTextComponent = encText.AddComponent<TMPro.TextMeshProUGUI>();
            encTextComponent.text = "TEST ENCYCLOPEDIA PANEL\nPress L to close";
            encTextComponent.fontSize = 24;
            encTextComponent.color = Color.white;
            encTextComponent.alignment = TMPro.TextAlignmentOptions.Center;
            
            var encTextRect = encText.GetComponent<RectTransform>();
            encTextRect.anchorMin = Vector2.zero;
            encTextRect.anchorMax = Vector2.one;
            encTextRect.sizeDelta = Vector2.zero;
            encTextRect.anchoredPosition = Vector2.zero;
            
            // Add EncyclopediaUI component and set it up
            var encUI = encPanel.AddComponent<EncyclopediaUI>();
            var encPanelField = typeof(EncyclopediaUI).GetField("encyclopediaPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            encPanelField?.SetValue(encUI, encPanel);
            
            encPanel.SetActive(false);
            
            Debug.Log("Created simple test panels with UI components");
        }
    }
}