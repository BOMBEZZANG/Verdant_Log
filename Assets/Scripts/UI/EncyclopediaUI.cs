using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using VerdantLog.Core;
using VerdantLog.Systems;
using VerdantLog.Data;

namespace VerdantLog.UI
{
    public class EncyclopediaUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject encyclopediaPanel;
        [SerializeField] private Transform entriesContainer;
        [SerializeField] private GameObject entryPrefab;
        [SerializeField] private TextMeshProUGUI completionText;
        [SerializeField] private TextMeshProUGUI selectedPlantName;
        [SerializeField] private TextMeshProUGUI selectedPlantDescription;
        [SerializeField] private Image selectedPlantImage;
        
        [Header("Input")]
        [SerializeField] private InputActionReference toggleEncyclopediaAction;
        
        private void Start()
        {
            if (encyclopediaPanel != null)
                encyclopediaPanel.SetActive(false);
                
            GameEvents.OnEncyclopediaUpdated += RefreshEncyclopedia;
            RefreshEncyclopedia();
            
            if (toggleEncyclopediaAction != null)
            {
                toggleEncyclopediaAction.action.performed += OnToggleEncyclopedia;
                toggleEncyclopediaAction.action.Enable();
            }
        }
        
        private void OnDestroy()
        {
            GameEvents.OnEncyclopediaUpdated -= RefreshEncyclopedia;
            
            if (toggleEncyclopediaAction != null)
            {
                toggleEncyclopediaAction.action.performed -= OnToggleEncyclopedia;
                toggleEncyclopediaAction.action.Disable();
            }
        }
        
        private void OnToggleEncyclopedia(InputAction.CallbackContext context)
        {
            ToggleEncyclopedia();
        }
        
        public void ToggleEncyclopedia()
        {
            if (encyclopediaPanel != null)
            {
                encyclopediaPanel.SetActive(!encyclopediaPanel.activeSelf);
                if (encyclopediaPanel.activeSelf)
                {
                    RefreshEncyclopedia();
                }
            }
        }
        
        private void RefreshEncyclopedia()
        {
            var encyclopedia = EncyclopediaSystem.Instance;
            
            // Update completion text
            if (completionText != null)
            {
                completionText.text = $"Discovered: {encyclopedia.DiscoveredPlants}/{encyclopedia.TotalPlants} ({encyclopedia.GetCompletionPercentage():F1}%)";
            }
            
            // Clear existing entries
            foreach (Transform child in entriesContainer)
            {
                Destroy(child.gameObject);
            }
            
            // Create entries for all plants
            var allPlants = encyclopedia.GetAllPlants();
            foreach (var plant in allPlants)
            {
                if (entryPrefab != null && entriesContainer != null)
                {
                    GameObject entry = Instantiate(entryPrefab, entriesContainer);
                    
                    bool isDiscovered = encyclopedia.IsDiscovered(plant.PlantID);
                    
                    TextMeshProUGUI nameText = entry.GetComponentInChildren<TextMeshProUGUI>();
                    if (nameText != null)
                    {
                        nameText.text = isDiscovered ? plant.PlantName : "???";
                    }
                    
                    Button button = entry.GetComponent<Button>();
                    if (button != null && isDiscovered)
                    {
                        button.onClick.AddListener(() => SelectPlant(plant));
                    }
                    
                    // Visual feedback for undiscovered plants
                    Image image = entry.GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = isDiscovered ? Color.white : new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    }
                }
            }
        }
        
        private void SelectPlant(PlantData plant)
        {
            if (selectedPlantName != null)
                selectedPlantName.text = plant.PlantName;
                
            if (selectedPlantDescription != null)
                selectedPlantDescription.text = plant.Description;
                
            if (selectedPlantImage != null && plant.GrowthStageSprites != null && plant.GrowthStageSprites.Length > 0)
            {
                selectedPlantImage.sprite = plant.GrowthStageSprites[plant.GrowthStageSprites.Length - 1];
            }
        }
    }
}