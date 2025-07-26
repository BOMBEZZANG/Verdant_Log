using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VerdantLog.Data;

namespace VerdantLog.Systems
{
    public class EncyclopediaSystem : MonoBehaviour
    {
        private static EncyclopediaSystem instance;
        public static EncyclopediaSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<EncyclopediaSystem>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("EncyclopediaSystem");
                        instance = go.AddComponent<EncyclopediaSystem>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        public static event Action<string> OnEntryUnlocked;
        public static event Action OnEncyclopediaUpdated;
        
        [SerializeField] private Dictionary<string, LogEntry> plantEntries = new Dictionary<string, LogEntry>();
        [SerializeField] private List<PlantData> allPlants = new List<PlantData>();
        
        private Dictionary<string, PlantData> plantLookup = new Dictionary<string, PlantData>();
        
        public int TotalPlants => plantLookup.Count;
        public int DiscoveredPlants => plantEntries.Count(e => e.Value.isDiscovered);
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeEncyclopedia();
            SubscribeToEvents();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        private void InitializeEncyclopedia()
        {
            plantLookup.Clear();
            
            foreach (var plant in allPlants)
            {
                if (plant != null && !string.IsNullOrEmpty(plant.PlantID))
                {
                    plantLookup[plant.PlantID] = plant;
                }
            }
            
            PlantData[] resourcePlants = Resources.LoadAll<PlantData>("Plants");
            foreach (var plant in resourcePlants)
            {
                if (!plantLookup.ContainsKey(plant.PlantID))
                {
                    plantLookup[plant.PlantID] = plant;
                }
            }
            
            foreach (var plant in plantLookup.Values)
            {
                if (!plantEntries.ContainsKey(plant.PlantID))
                {
                    plantEntries[plant.PlantID] = new LogEntry(plant.PlantID);
                }
            }
            
            Debug.Log($"Encyclopedia initialized with {plantLookup.Count} plants");
        }
        
        private void SubscribeToEvents()
        {
            CultivationManager.OnCultivationSuccess += OnCultivationSuccess;
        }
        
        private void UnsubscribeFromEvents()
        {
            CultivationManager.OnCultivationSuccess -= OnCultivationSuccess;
        }
        
        private void OnCultivationSuccess(string plantID, int expValue)
        {
            UnlockEntry(plantID);
        }
        
        public void UnlockEntry(string plantID)
        {
            if (string.IsNullOrEmpty(plantID))
                return;
                
            if (!plantEntries.ContainsKey(plantID))
            {
                plantEntries[plantID] = new LogEntry(plantID);
            }
            
            LogEntry entry = plantEntries[plantID];
            bool wasDiscovered = entry.isDiscovered;
            
            entry.Discover();
            entry.timesHarvested++;
            
            if (!wasDiscovered)
            {
                Debug.Log($"New plant discovered: {GetPlantData(plantID)?.PlantName ?? plantID}");
                OnEntryUnlocked?.Invoke(plantID);
            }
            
            OnEncyclopediaUpdated?.Invoke();
        }
        
        public bool IsDiscovered(string plantID)
        {
            if (string.IsNullOrEmpty(plantID) || !plantEntries.ContainsKey(plantID))
                return false;
                
            return plantEntries[plantID].isDiscovered;
        }
        
        public LogEntry GetEntry(string plantID)
        {
            if (string.IsNullOrEmpty(plantID) || !plantEntries.TryGetValue(plantID, out LogEntry entry))
                return null;
                
            return entry;
        }
        
        public PlantData GetPlantData(string plantID)
        {
            if (string.IsNullOrEmpty(plantID) || !plantLookup.TryGetValue(plantID, out PlantData plant))
                return null;
                
            return plant;
        }
        
        public List<PlantData> GetDiscoveredPlants()
        {
            return plantEntries.Where(e => e.Value.isDiscovered)
                             .Select(e => GetPlantData(e.Key))
                             .Where(p => p != null)
                             .ToList();
        }
        
        public List<PlantData> GetAllPlants()
        {
            return new List<PlantData>(plantLookup.Values);
        }
        
        public float GetCompletionPercentage()
        {
            if (TotalPlants == 0)
                return 0f;
                
            return (float)DiscoveredPlants / TotalPlants * 100f;
        }
        
        public List<string> GetHintsForUndiscoveredPlants()
        {
            List<string> hints = new List<string>();
            
            var undiscoveredPlants = plantEntries.Where(e => !e.Value.isDiscovered).Select(e => e.Key);
            
            foreach (string plantID in undiscoveredPlants)
            {
                var recipes = CultivationManager.Instance.GetRecipesForSeed(GetPlantData(plantID)?.SeedItem?.ItemID);
                foreach (var recipe in recipes)
                {
                    if (!string.IsNullOrEmpty(recipe.RecipeHint))
                    {
                        hints.Add(recipe.RecipeHint);
                    }
                }
            }
            
            return hints;
        }
    }
}