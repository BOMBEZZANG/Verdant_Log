using UnityEngine;
using VerdantLog.Systems;

namespace VerdantLog.Core
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        [Header("System References")]
        [SerializeField] private bool autoInitializeSystems = true;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (autoInitializeSystems)
            {
                InitializeSystems();
            }
        }
        
        private void InitializeSystems()
        {
            // Initialize all singleton systems
            var itemDatabase = ItemDatabase.Instance;
            var inventory = Inventory.Instance;
            var cultivationManager = CultivationManager.Instance;
            var playerStats = PlayerStats.Instance;
            var worldManager = WorldManager.Instance;
            var encyclopediaSystem = EncyclopediaSystem.Instance;
            var timeManager = TimeManager.Instance;
            
            Debug.Log("All game systems initialized");
            
            // Subscribe to important events for logging
            SubscribeToEvents();
        }
        
        private void SubscribeToEvents()
        {
            GameEvents.OnPlayerLevelUp += OnPlayerLevelUp;
            GameEvents.OnZoneUnlocked += OnZoneUnlocked;
            GameEvents.OnEntryUnlocked += OnPlantDiscovered;
            GameEvents.OnCultivationSuccess += OnCultivationSuccess;
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                GameEvents.OnPlayerLevelUp -= OnPlayerLevelUp;
                GameEvents.OnZoneUnlocked -= OnZoneUnlocked;
                GameEvents.OnEntryUnlocked -= OnPlantDiscovered;
                GameEvents.OnCultivationSuccess -= OnCultivationSuccess;
            }
        }
        
        private void OnPlayerLevelUp(int newLevel)
        {
            GameEvents.TriggerNotification($"Level Up! You are now level {newLevel}");
        }
        
        private void OnZoneUnlocked(string zoneID)
        {
            var zoneData = WorldManager.Instance.GetZoneData(zoneID);
            if (zoneData != null)
            {
                GameEvents.TriggerNotification($"New area unlocked: {zoneData.ZoneName}!");
            }
        }
        
        private void OnPlantDiscovered(string plantID)
        {
            var plantData = EncyclopediaSystem.Instance.GetPlantData(plantID);
            if (plantData != null)
            {
                GameEvents.TriggerNotification($"New plant discovered: {plantData.PlantName}!");
            }
        }
        
        private void OnCultivationSuccess(string plantID, int expValue)
        {
            GameEvents.TriggerNotification($"Cultivation successful! +{expValue} EXP");
        }
        
        public void SaveGame()
        {
            // TODO: Implement save system
            Debug.Log("Game saved");
            GameEvents.TriggerGameSaved();
        }
        
        public void LoadGame()
        {
            // TODO: Implement load system
            Debug.Log("Game loaded");
            GameEvents.TriggerGameLoaded();
        }
        
        public void NewGame()
        {
            PlayerStats.Instance.ResetStats();
            Inventory.Instance.Clear();
            Debug.Log("New game started");
        }
    }
}