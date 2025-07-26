using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VerdantLog.Data;

namespace VerdantLog.Systems
{
    public class WorldManager : MonoBehaviour
    {
        private static WorldManager instance;
        public static WorldManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<WorldManager>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("WorldManager");
                        instance = go.AddComponent<WorldManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }
        
        public static event Action<string> OnZoneUnlocked;
        public static event Action<string> OnZoneEntered;
        
        [SerializeField] private List<ZoneData> allZones = new List<ZoneData>();
        [SerializeField] private Dictionary<string, bool> unlockedZones = new Dictionary<string, bool>();
        [SerializeField] private string currentZoneID = "zone_hub";
        
        private Dictionary<string, ZoneData> zoneLookup = new Dictionary<string, ZoneData>();
        
        public string CurrentZoneID => currentZoneID;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            InitializeZones();
            SubscribeToEvents();
        }
        
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        
        private void InitializeZones()
        {
            zoneLookup.Clear();
            
            foreach (var zone in allZones)
            {
                if (zone != null && !string.IsNullOrEmpty(zone.ZoneID))
                {
                    zoneLookup[zone.ZoneID] = zone;
                }
            }
            
            ZoneData[] resourceZones = Resources.LoadAll<ZoneData>("Zones");
            foreach (var zone in resourceZones)
            {
                if (!zoneLookup.ContainsKey(zone.ZoneID))
                {
                    zoneLookup[zone.ZoneID] = zone;
                }
            }
            
            foreach (var zone in zoneLookup.Values)
            {
                if (zone.UnlockRequirement.unlockType == UnlockType.Always)
                {
                    unlockedZones[zone.ZoneID] = true;
                }
            }
            
            Debug.Log($"WorldManager initialized with {zoneLookup.Count} zones");
            CheckUnlockConditions();
        }
        
        private void SubscribeToEvents()
        {
            PlayerStats.OnPlayerLevelUp += OnPlayerLevelUp;
            Inventory.OnInventoryUpdated += OnInventoryUpdated;
        }
        
        private void UnsubscribeFromEvents()
        {
            PlayerStats.OnPlayerLevelUp -= OnPlayerLevelUp;
            Inventory.OnInventoryUpdated -= OnInventoryUpdated;
        }
        
        private void OnPlayerLevelUp(int newLevel)
        {
            CheckUnlockConditions();
        }
        
        private void OnInventoryUpdated()
        {
            CheckUnlockConditions();
        }
        
        private void CheckUnlockConditions()
        {
            foreach (var kvp in zoneLookup)
            {
                string zoneID = kvp.Key;
                ZoneData zone = kvp.Value;
                
                if (!IsZoneUnlocked(zoneID) && CheckZoneRequirement(zone.UnlockRequirement))
                {
                    UnlockZone(zoneID);
                }
            }
        }
        
        private bool CheckZoneRequirement(UnlockRequirement requirement)
        {
            if (requirement == null)
                return true;
                
            switch (requirement.unlockType)
            {
                case UnlockType.Level:
                    return PlayerStats.Instance.CurrentLevel >= requirement.requiredLevel;
                    
                case UnlockType.Item:
                    return Inventory.Instance.HasItem(requirement.requiredItemID, requirement.requiredItemCount);
                    
                case UnlockType.Always:
                    return true;
                    
                default:
                    return false;
            }
        }
        
        public void UnlockZone(string zoneID)
        {
            if (string.IsNullOrEmpty(zoneID) || !zoneLookup.ContainsKey(zoneID))
            {
                Debug.LogWarning($"Cannot unlock zone: {zoneID} not found");
                return;
            }
            
            if (IsZoneUnlocked(zoneID))
            {
                return;
            }
            
            unlockedZones[zoneID] = true;
            Debug.Log($"Zone unlocked: {zoneLookup[zoneID].ZoneName}");
            OnZoneUnlocked?.Invoke(zoneID);
        }
        
        public bool IsZoneUnlocked(string zoneID)
        {
            if (string.IsNullOrEmpty(zoneID))
                return false;
                
            return unlockedZones.TryGetValue(zoneID, out bool unlocked) && unlocked;
        }
        
        public List<ZoneData> GetUnlockedZones()
        {
            return zoneLookup.Where(kvp => IsZoneUnlocked(kvp.Key))
                           .Select(kvp => kvp.Value)
                           .ToList();
        }
        
        public List<ZoneData> GetAllZones()
        {
            return new List<ZoneData>(zoneLookup.Values);
        }
        
        public ZoneData GetZoneData(string zoneID)
        {
            if (string.IsNullOrEmpty(zoneID) || !zoneLookup.TryGetValue(zoneID, out ZoneData zone))
            {
                return null;
            }
            
            return zone;
        }
        
        public void EnterZone(string zoneID)
        {
            if (!IsZoneUnlocked(zoneID))
            {
                Debug.LogWarning($"Cannot enter locked zone: {zoneID}");
                return;
            }
            
            ZoneData zone = GetZoneData(zoneID);
            if (zone == null || string.IsNullOrEmpty(zone.SceneName))
            {
                Debug.LogWarning($"Zone {zoneID} has no valid scene");
                return;
            }
            
            currentZoneID = zoneID;
            OnZoneEntered?.Invoke(zoneID);
            SceneManager.LoadScene(zone.SceneName);
        }
    }
}