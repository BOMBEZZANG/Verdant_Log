using System;
using UnityEngine;

namespace VerdantLog.Core
{
    public static class GameEvents
    {
        // Inventory Events
        public static event Action OnInventoryUpdated
        {
            add => Systems.Inventory.OnInventoryUpdated += value;
            remove => Systems.Inventory.OnInventoryUpdated -= value;
        }
        
        // Cultivation Events
        public static event Action<string, int> OnCultivationSuccess
        {
            add => Systems.CultivationManager.OnCultivationSuccess += value;
            remove => Systems.CultivationManager.OnCultivationSuccess -= value;
        }
        
        public static event Action<string> OnCultivationFail
        {
            add => Systems.CultivationManager.OnCultivationFail += value;
            remove => Systems.CultivationManager.OnCultivationFail -= value;
        }
        
        // Player Progression Events
        public static event Action<int> OnPlayerLevelUp
        {
            add => Systems.PlayerStats.OnPlayerLevelUp += value;
            remove => Systems.PlayerStats.OnPlayerLevelUp -= value;
        }
        
        public static event Action<int, int> OnEXPChanged
        {
            add => Systems.PlayerStats.OnEXPChanged += value;
            remove => Systems.PlayerStats.OnEXPChanged -= value;
        }
        
        // World Events
        public static event Action<string> OnZoneUnlocked
        {
            add => Systems.WorldManager.OnZoneUnlocked += value;
            remove => Systems.WorldManager.OnZoneUnlocked -= value;
        }
        
        public static event Action<string> OnZoneEntered
        {
            add => Systems.WorldManager.OnZoneEntered += value;
            remove => Systems.WorldManager.OnZoneEntered -= value;
        }
        
        // Encyclopedia Events
        public static event Action<string> OnEntryUnlocked
        {
            add => Systems.EncyclopediaSystem.OnEntryUnlocked += value;
            remove => Systems.EncyclopediaSystem.OnEntryUnlocked -= value;
        }
        
        public static event Action OnEncyclopediaUpdated
        {
            add => Systems.EncyclopediaSystem.OnEncyclopediaUpdated += value;
            remove => Systems.EncyclopediaSystem.OnEncyclopediaUpdated -= value;
        }
        
        // Time Events
        public static event Action<string> OnTimeOfDayChanged
        {
            add => Systems.TimeManager.OnTimeOfDayChanged += value;
            remove => Systems.TimeManager.OnTimeOfDayChanged -= value;
        }
        
        // Save/Load Events
        public static event Action OnGameSaved;
        public static event Action OnGameLoaded;
        
        // UI Events
        public static event Action<string> OnNotification;
        public static event Action<string, float> OnProgressNotification;
        
        // Trigger UI Events
        public static void TriggerNotification(string message)
        {
            OnNotification?.Invoke(message);
        }
        
        public static void TriggerProgressNotification(string message, float progress)
        {
            OnProgressNotification?.Invoke(message, progress);
        }
        
        public static void TriggerGameSaved()
        {
            OnGameSaved?.Invoke();
        }
        
        public static void TriggerGameLoaded()
        {
            OnGameLoaded?.Invoke();
        }
    }
}