using System.Collections.Generic;
using UnityEngine;

namespace VerdantLog.Data
{
    [CreateAssetMenu(fileName = "New Cultivation Recipe", menuName = "Verdant Log/Data/Cultivation Recipe")]
    public class CultivationRecipe : ScriptableObject
    {
        [Header("Recipe Information")]
        [SerializeField] private string recipeID;
        [SerializeField] private string recipeName;
        [TextArea(2, 4)]
        [SerializeField] private string recipeHint;
        
        [Header("Input and Output")]
        [SerializeField] private ItemData inputSeed;
        [SerializeField] private PlantData outputPlant;
        
        [Header("Required Conditions")]
        [SerializeField] private List<CultivationCondition> requiredConditions = new List<CultivationCondition>();
        
        [Header("Discovery")]
        [SerializeField] private bool isDiscoveredByDefault = false;
        
        public string RecipeID => recipeID;
        public string RecipeName => recipeName;
        public string RecipeHint => recipeHint;
        public ItemData InputSeed => inputSeed;
        public PlantData OutputPlant => outputPlant;
        public List<CultivationCondition> RequiredConditions => requiredConditions;
        public bool IsDiscoveredByDefault => isDiscoveredByDefault;
        
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(recipeID))
            {
                recipeID = name;
            }
        }
        
        public bool CheckConditions(CultivationContext context)
        {
            foreach (var condition in requiredConditions)
            {
                if (!CheckSingleCondition(condition, context))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        private bool CheckSingleCondition(CultivationCondition condition, CultivationContext context)
        {
            switch (condition.Type)
            {
                case ConditionType.TimeOfDay:
                    return context.timeOfDay == condition.Value;
                    
                case ConditionType.ItemUsed:
                    return context.itemUsedID == condition.Value;
                    
                case ConditionType.Adjacency:
                    return context.adjacentPlantIDs.Contains(condition.Value);
                    
                default:
                    Debug.LogWarning($"Unhandled condition type: {condition.Type}");
                    return true;
            }
        }
    }
    
    [System.Serializable]
    public class CultivationContext
    {
        public string seedID;
        public string timeOfDay;
        public string itemUsedID;
        public List<string> adjacentPlantIDs = new List<string>();
        
        public CultivationContext(string seedID)
        {
            this.seedID = seedID;
        }
    }
}