using UnityEngine;
using VerdantLog.UI;

namespace VerdantLog.Core
{
    public class UIComponentAdder : MonoBehaviour
    {
        [Header("UI Component Adder")]
        public bool debugMode = true;
        
        [ContextMenu("Add UI Components to Existing Panels")]
        public void AddUIComponentsToExistingPanels()
        {
            Debug.Log("=== ADDING UI COMPONENTS ===");
            
            // Find existing panels
            GameObject invPanel = GameObject.Find("Test Inventory Panel");
            GameObject encPanel = GameObject.Find("Test Encyclopedia Panel");
            
            if (invPanel == null)
            {
                Debug.LogError("Test Inventory Panel not found!");
                return;
            }
            
            if (encPanel == null)
            {
                Debug.LogError("Test Encyclopedia Panel not found!");
                return;
            }
            
            // Remove any existing UI components first
            InventoryUI existingInvUI = invPanel.GetComponent<InventoryUI>();
            if (existingInvUI != null)
            {
                DestroyImmediate(existingInvUI);
                Debug.Log("Removed existing InventoryUI component");
            }
            
            EncyclopediaUI existingEncUI = encPanel.GetComponent<EncyclopediaUI>();
            if (existingEncUI != null)
            {
                DestroyImmediate(existingEncUI);
                Debug.Log("Removed existing EncyclopediaUI component");
            }
            
            // Add fresh UI components
            InventoryUI newInvUI = invPanel.AddComponent<InventoryUI>();
            Debug.Log("✓ Added InventoryUI component to Test Inventory Panel");
            
            EncyclopediaUI newEncUI = encPanel.AddComponent<EncyclopediaUI>();
            Debug.Log("✓ Added EncyclopediaUI component to Test Encyclopedia Panel");
            
            // Set the panel references using reflection
            var invPanelField = typeof(InventoryUI).GetField("inventoryPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (invPanelField != null)
            {
                invPanelField.SetValue(newInvUI, invPanel);
                Debug.Log("✓ InventoryUI panel reference set");
            }
            
            var encPanelField = typeof(EncyclopediaUI).GetField("encyclopediaPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (encPanelField != null)
            {
                encPanelField.SetValue(newEncUI, encPanel);
                Debug.Log("✓ EncyclopediaUI panel reference set");
            }
            
            Debug.Log("=== UI COMPONENTS ADDED ===");
        }
        
        [ContextMenu("Create Complete Working UI")]
        public void CreateCompleteWorkingUI()
        {
            Debug.Log("=== CREATING COMPLETE WORKING UI ===");
            
            // Find or create canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                Debug.Log("Created Canvas");
            }
            
            // Destroy existing test panels if they exist
            GameObject oldInvPanel = GameObject.Find("Test Inventory Panel");
            if (oldInvPanel != null) DestroyImmediate(oldInvPanel);
            
            GameObject oldEncPanel = GameObject.Find("Test Encyclopedia Panel");
            if (oldEncPanel != null) DestroyImmediate(oldEncPanel);
            
            // Create inventory panel with proper setup
            GameObject invPanel = CreateInventoryPanel(canvas);
            Debug.Log("✓ Created working inventory panel");
            
            // Create encyclopedia panel with proper setup
            GameObject encPanel = CreateEncyclopediaPanel(canvas);
            Debug.Log("✓ Created working encyclopedia panel");
            
            Debug.Log("=== COMPLETE WORKING UI CREATED ===");
            Debug.Log("Test: Press I for inventory, Press L for encyclopedia");
        }
        
        private GameObject CreateInventoryPanel(Canvas canvas)
        {
            // Create panel
            GameObject panel = new GameObject("Working Inventory Panel");
            panel.transform.SetParent(canvas.transform, false);
            
            // Set up RectTransform
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.1f, 0.1f);
            rect.anchorMax = new Vector2(0.9f, 0.9f);
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            // Add background image
            UnityEngine.UI.Image image = panel.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            // Add text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(panel.transform, false);
            
            TMPro.TextMeshProUGUI text = textGO.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = "WORKING INVENTORY PANEL\n\nPress I to close\n\nItems will appear here when system is fully integrated.";
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            
            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            // Add InventoryUI component and configure it
            InventoryUI invUI = panel.AddComponent<InventoryUI>();
            
            // Set the panel reference using reflection
            var panelField = typeof(InventoryUI).GetField("inventoryPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            panelField?.SetValue(invUI, panel);
            
            // Start inactive
            panel.SetActive(false);
            
            return panel;
        }
        
        private GameObject CreateEncyclopediaPanel(Canvas canvas)
        {
            // Create panel
            GameObject panel = new GameObject("Working Encyclopedia Panel");
            panel.transform.SetParent(canvas.transform, false);
            
            // Set up RectTransform
            RectTransform rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.1f, 0.1f);
            rect.anchorMax = new Vector2(0.9f, 0.9f);
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            
            // Add background image
            UnityEngine.UI.Image image = panel.AddComponent<UnityEngine.UI.Image>();
            image.color = new Color(0.1f, 0.3f, 0.1f, 0.9f);
            
            // Add text
            GameObject textGO = new GameObject("Text");
            textGO.transform.SetParent(panel.transform, false);
            
            TMPro.TextMeshProUGUI text = textGO.AddComponent<TMPro.TextMeshProUGUI>();
            text.text = "WORKING ENCYCLOPEDIA PANEL\n\nPress L to close\n\nDiscovered plants will appear here when you cultivate them.";
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = TMPro.TextAlignmentOptions.Center;
            
            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            // Add EncyclopediaUI component and configure it
            EncyclopediaUI encUI = panel.AddComponent<EncyclopediaUI>();
            
            // Set the panel reference using reflection
            var panelField = typeof(EncyclopediaUI).GetField("encyclopediaPanel", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            panelField?.SetValue(encUI, panel);
            
            // Start inactive
            panel.SetActive(false);
            
            return panel;
        }
        
        [ContextMenu("Test Working UI")]
        public void TestWorkingUI()
        {
            Debug.Log("=== TESTING WORKING UI ===");
            
            InventoryUI invUI = FindObjectOfType<InventoryUI>();
            if (invUI != null)
            {
                Debug.Log("Found InventoryUI - testing toggle");
                invUI.ToggleInventory();
            }
            else
            {
                Debug.LogError("No InventoryUI found!");
            }
            
            EncyclopediaUI encUI = FindObjectOfType<EncyclopediaUI>();
            if (encUI != null)
            {
                Debug.Log("Found EncyclopediaUI - testing toggle");
                encUI.ToggleEncyclopedia();
            }
            else
            {
                Debug.LogError("No EncyclopediaUI found!");
            }
        }
    }
}