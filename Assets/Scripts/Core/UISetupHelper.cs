using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VerdantLog.UI;

namespace VerdantLog.Core
{
    public class UISetupHelper : MonoBehaviour
    {
        [Header("UI Setup Helper")]
        public bool setupComplete = false;
        
        [ContextMenu("1. Create Basic UI Structure")]
        public void CreateBasicUIStructure()
        {
            // Find or create Canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }
            
            // Create HUD Panel
            GameObject hudPanel = new GameObject("HUD Panel");
            hudPanel.transform.SetParent(canvas.transform, false);
            hudPanel.AddComponent<RectTransform>();
            hudPanel.AddComponent<Image>().color = new Color(0, 0, 0, 0.3f);
            
            // Create debug text
            GameObject debugText = new GameObject("Debug Text");
            debugText.transform.SetParent(hudPanel.transform, false);
            var text = debugText.AddComponent<TextMeshProUGUI>();
            text.text = "HUD Panel - Press I for Inventory, L for Encyclopedia";
            text.color = Color.white;
            text.fontSize = 18;
            
            // Position debug text
            RectTransform debugRect = debugText.GetComponent<RectTransform>();
            debugRect.anchorMin = new Vector2(0, 1);
            debugRect.anchorMax = new Vector2(1, 1);
            debugRect.anchoredPosition = new Vector2(0, -30);
            debugRect.sizeDelta = new Vector2(0, 60);
            
            Debug.Log("Basic UI structure created!");
        }
        
        [ContextMenu("2. Create Inventory Panel")]
        public void CreateInventoryPanel()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found! Run '1. Create Basic UI Structure' first");
                return;
            }
            
            // Create Inventory Panel
            GameObject invPanel = new GameObject("Inventory Panel");
            invPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform invRect = invPanel.AddComponent<RectTransform>();
            invRect.anchorMin = Vector2.zero;
            invRect.anchorMax = Vector2.one;
            invRect.sizeDelta = Vector2.zero;
            invRect.anchoredPosition = Vector2.zero;
            
            Image invImage = invPanel.AddComponent<Image>();
            invImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            
            // Add InventoryUI component
            InventoryUI invUI = invPanel.AddComponent<InventoryUI>();
            
            // Create simple text display
            GameObject invText = new GameObject("Inventory Text");
            invText.transform.SetParent(invPanel.transform, false);
            var text = invText.AddComponent<TextMeshProUGUI>();
            text.text = "INVENTORY\nPress I to close";
            text.color = Color.white;
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = invText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            // Set panel inactive
            invPanel.SetActive(false);
            
            Debug.Log("Inventory Panel created! InventoryUI component needs manual setup.");
        }
        
        [ContextMenu("3. Create Encyclopedia Panel")]
        public void CreateEncyclopediaPanel()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found! Run '1. Create Basic UI Structure' first");
                return;
            }
            
            // Create Encyclopedia Panel
            GameObject encPanel = new GameObject("Encyclopedia Panel");
            encPanel.transform.SetParent(canvas.transform, false);
            
            RectTransform encRect = encPanel.AddComponent<RectTransform>();
            encRect.anchorMin = Vector2.zero;
            encRect.anchorMax = Vector2.one;
            encRect.sizeDelta = Vector2.zero;
            encRect.anchoredPosition = Vector2.zero;
            
            Image encImage = encPanel.AddComponent<Image>();
            encImage.color = new Color(0.1f, 0.3f, 0.1f, 0.9f);
            
            // Add EncyclopediaUI component
            EncyclopediaUI encUI = encPanel.AddComponent<EncyclopediaUI>();
            
            // Create simple text display
            GameObject encText = new GameObject("Encyclopedia Text");
            encText.transform.SetParent(encPanel.transform, false);
            var text = encText.AddComponent<TextMeshProUGUI>();
            text.text = "ENCYCLOPEDIA\nPress L to close";
            text.color = Color.white;
            text.fontSize = 24;
            text.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = encText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            // Set panel inactive
            encPanel.SetActive(false);
            
            Debug.Log("Encyclopedia Panel created! EncyclopediaUI component needs manual setup.");
        }
        
        [ContextMenu("4. Auto-Setup UI References")]
        public void AutoSetupUIReferences()
        {
            // Setup Inventory UI
            InventoryUI invUI = FindObjectOfType<InventoryUI>();
            if (invUI != null)
            {
                var invPanel = invUI.gameObject;
                
                // Use reflection to set the inventoryPanel field
                var invPanelField = typeof(InventoryUI).GetField("inventoryPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                invPanelField?.SetValue(invUI, invPanel);
                
                Debug.Log("InventoryUI references set up");
            }
            
            // Setup Encyclopedia UI
            EncyclopediaUI encUI = FindObjectOfType<EncyclopediaUI>();
            if (encUI != null)
            {
                var encPanel = encUI.gameObject;
                
                // Use reflection to set the encyclopediaPanel field
                var encPanelField = typeof(EncyclopediaUI).GetField("encyclopediaPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                encPanelField?.SetValue(encUI, encPanel);
                
                Debug.Log("EncyclopediaUI references set up");
            }
        }
    }
}