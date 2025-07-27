using UnityEngine;
using VerdantLog.Systems;
using VerdantLog.Data;

namespace VerdantLog.Player
{
    [RequireComponent(typeof(Interactable))]
    public class PlantInteractable : MonoBehaviour
    {
        [Header("Plant Settings")]
        [SerializeField] private PlantData plantData;
        [SerializeField] private int currentGrowthStage = 0;
        [SerializeField] private bool isHarvestable = false;
        
        [Header("Visual")]
        [SerializeField] private SpriteRenderer plantSpriteRenderer;
        
        private Interactable interactable;
        
        private void Awake()
        {
            interactable = GetComponent<Interactable>();
            
            if (plantSpriteRenderer == null)
            {
                plantSpriteRenderer = GetComponent<SpriteRenderer>();
            }
            
            UpdateInteractableState();
        }
        
        private void Start()
        {
            UpdateVisuals();
        }
        
        public void HandleInteraction()
        {
            if (isHarvestable && plantData != null)
            {
                HarvestPlant();
            }
            else
            {
                Core.GameEvents.TriggerNotification("This plant is not ready for harvest yet.");
            }
        }
        
        private void HarvestPlant()
        {
            var inventory = Inventory.Instance;
            var cultivationManager = CultivationManager.Instance;
            
            if (inventory == null || cultivationManager == null)
            {
                Debug.LogError("Required systems not found!");
                return;
            }
            
            if (plantData.HarvestYield != null)
            {
                inventory.AddItem(plantData.HarvestYield.ItemID, plantData.HarvestYieldAmount);
                
                cultivationManager.TriggerCultivationSuccess(plantData.PlantID, plantData.ExpValue);
                
                Core.GameEvents.TriggerNotification($"Harvested {plantData.HarvestYieldAmount}x {plantData.HarvestYield.ItemName}!");
                
                ResetPlant();
            }
        }
        
        private void ResetPlant()
        {
            currentGrowthStage = 0;
            isHarvestable = false;
            UpdateVisuals();
            UpdateInteractableState();
        }
        
        public void SetGrowthStage(int stage)
        {
            if (plantData == null || plantData.GrowthStageSprites == null || plantData.GrowthStageSprites.Length == 0)
            {
                currentGrowthStage = 0;
                isHarvestable = false;
                UpdateInteractableState();
                return;
            }
            
            currentGrowthStage = Mathf.Clamp(stage, 0, plantData.GrowthStageSprites.Length - 1);
            
            isHarvestable = (currentGrowthStage >= plantData.GrowthStageSprites.Length - 1);
            
            UpdateVisuals();
            UpdateInteractableState();
        }
        
        public void GrowToNextStage()
        {
            if (plantData != null && plantData.GrowthStageSprites != null && 
                plantData.GrowthStageSprites.Length > 0 && 
                currentGrowthStage < plantData.GrowthStageSprites.Length - 1)
            {
                SetGrowthStage(currentGrowthStage + 1);
            }
        }
        
        private void UpdateVisuals()
        {
            if (plantSpriteRenderer != null && plantData != null && plantData.GrowthStageSprites != null && plantData.GrowthStageSprites.Length > 0)
            {
                int safeIndex = Mathf.Clamp(currentGrowthStage, 0, plantData.GrowthStageSprites.Length - 1);
                if (plantData.GrowthStageSprites[safeIndex] != null)
                {
                    plantSpriteRenderer.sprite = plantData.GrowthStageSprites[safeIndex];
                }
            }
        }
        
        private void UpdateInteractableState()
        {
            if (interactable != null)
            {
                interactable.SetInteractable(isHarvestable);
                interactable.SetInteractionName(isHarvestable ? "Harvest" : "Growing...");
            }
        }
        
        public void SetPlantData(PlantData data)
        {
            plantData = data;
            currentGrowthStage = 0;
            isHarvestable = false;
            UpdateVisuals();
            UpdateInteractableState();
        }
        
        public PlantData GetPlantData()
        {
            return plantData;
        }
        
        public int GetCurrentGrowthStage()
        {
            return currentGrowthStage;
        }
        
        public bool IsHarvestable()
        {
            return isHarvestable;
        }
    }
}