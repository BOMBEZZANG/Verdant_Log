using UnityEngine;

namespace VerdantLog.Core
{
    public class QuickUITest : MonoBehaviour
    {
        [Header("Quick Test Setup")]
        [SerializeField] private GameObject testInventoryPanel;
        [SerializeField] private GameObject testEncyclopediaPanel;
        
        private void Start()
        {
            Debug.Log("[QuickUITest] Started - Press I for Inventory, L for Encyclopedia");
            
            if (testInventoryPanel == null)
                Debug.LogWarning("[QuickUITest] testInventoryPanel not assigned!");
            else
                testInventoryPanel.SetActive(false);
                
            if (testEncyclopediaPanel == null)
                Debug.LogWarning("[QuickUITest] testEncyclopediaPanel not assigned!");
            else
                testEncyclopediaPanel.SetActive(false);
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I) && testInventoryPanel != null)
            {
                bool newState = !testInventoryPanel.activeSelf;
                testInventoryPanel.SetActive(newState);
                Debug.Log($"[QuickUITest] Inventory toggled to: {newState}");
            }
            
            if (Input.GetKeyDown(KeyCode.L) && testEncyclopediaPanel != null)
            {
                bool newState = !testEncyclopediaPanel.activeSelf;
                testEncyclopediaPanel.SetActive(newState);
                Debug.Log($"[QuickUITest] Encyclopedia toggled to: {newState}");
            }
        }
    }
}