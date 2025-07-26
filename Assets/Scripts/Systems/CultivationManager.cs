using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VerdantLog.Data;

namespace VerdantLog.Systems
{
    public class CultivationManager : MonoBehaviour
    {
        private static CultivationManager instance;
        public static CultivationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CultivationManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("CultivationManager");
                        instance = go.AddComponent<CultivationManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        public static event Action<string, int> OnCultivationSuccess;
        public static event Action<string> OnCultivationFail;
        
        [SerializeField] private List<CultivationRecipe> allRecipes = new List<CultivationRecipe>();
        private Dictionary<string, List<CultivationRecipe>> recipesBySeed = new Dictionary<string, List<CultivationRecipe>>();
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeRecipes();
        }
        
        private void InitializeRecipes()
        {
            recipesBySeed.Clear();
            
            foreach (var recipe in allRecipes)
            {
                if (recipe != null && recipe.InputSeed != null)
                {
                    string seedID = recipe.InputSeed.ItemID;
                    if (!recipesBySeed.ContainsKey(seedID))
                    {
                        recipesBySeed[seedID] = new List<CultivationRecipe>();
                    }
                    recipesBySeed[seedID].Add(recipe);
                }
            }
            
            CultivationRecipe[] resourceRecipes = Resources.LoadAll<CultivationRecipe>("Recipes");
            foreach (var recipe in resourceRecipes)
            {
                if (recipe != null && recipe.InputSeed != null)
                {
                    string seedID = recipe.InputSeed.ItemID;
                    if (!recipesBySeed.ContainsKey(seedID))
                    {
                        recipesBySeed[seedID] = new List<CultivationRecipe>();
                    }
                    if (!recipesBySeed[seedID].Contains(recipe))
                    {
                        recipesBySeed[seedID].Add(recipe);
                    }
                }
            }
            
            Debug.Log($"CultivationManager initialized with {recipesBySeed.Count} seed types");
        }
        
        public CultivationResult AttemptCultivation(CultivationContext context)
        {
            if (context == null || string.IsNullOrEmpty(context.seedID))
            {
                Debug.LogWarning("Invalid cultivation context");
                return new CultivationResult { success = false, reason = "Invalid context" };
            }
            
            if (!recipesBySeed.TryGetValue(context.seedID, out List<CultivationRecipe> possibleRecipes))
            {
                Debug.Log($"No recipes found for seed: {context.seedID}");
                OnCultivationFail?.Invoke("No recipes found for this seed");
                return new CultivationResult { success = false, reason = "No recipes for this seed" };
            }
            
            foreach (var recipe in possibleRecipes)
            {
                if (recipe.CheckConditions(context))
                {
                    PlantData outputPlant = recipe.OutputPlant;
                    if (outputPlant != null)
                    {
                        OnCultivationSuccess?.Invoke(outputPlant.PlantID, outputPlant.ExpValue);
                        return new CultivationResult 
                        { 
                            success = true, 
                            outputPlant = outputPlant,
                            recipe = recipe
                        };
                    }
                }
            }
            
            OnCultivationFail?.Invoke("Conditions not met");
            return new CultivationResult { success = false, reason = "No matching conditions" };
        }
        
        public List<CultivationRecipe> GetRecipesForSeed(string seedID)
        {
            if (string.IsNullOrEmpty(seedID) || !recipesBySeed.TryGetValue(seedID, out List<CultivationRecipe> recipes))
            {
                return new List<CultivationRecipe>();
            }
            
            return new List<CultivationRecipe>(recipes);
        }
        
        public CultivationRecipe GetRecipe(string recipeID)
        {
            return allRecipes.FirstOrDefault(r => r.RecipeID == recipeID);
        }
        
        public void RefreshRecipes()
        {
            InitializeRecipes();
        }
    }
    
    public class CultivationResult
    {
        public bool success;
        public string reason;
        public PlantData outputPlant;
        public CultivationRecipe recipe;
    }
}