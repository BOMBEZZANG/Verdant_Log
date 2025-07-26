using UnityEngine;
using VerdantLog.Systems;
using VerdantLog.Core;
using VerdantLog.Data;

namespace VerdantLog.Core
{
    public class ManualSystemTester : MonoBehaviour
    {
        [Header("Manual Tests")]
        public bool testModeEnabled = true;
        
        [ContextMenu("Test Add Basic Seed")]
        public void TestAddBasicSeed()
        {
            bool success = Inventory.Instance.AddItem("seed_basic", 5);
            Debug.Log($"Add Basic Seed Result: {success}");
        }
        
        [ContextMenu("Test Add EXP")]
        public void TestAddEXP()
        {
            PlayerStats.Instance.AddEXP(100);
            Debug.Log($"Added 100 EXP. Current Level: {PlayerStats.Instance.CurrentLevel}");
        }
        
        [ContextMenu("Test Cultivation")]
        public void TestCultivation()
        {
            if (!Inventory.Instance.HasItem("seed_basic"))
            {
                Inventory.Instance.AddItem("seed_basic", 1);
            }
            
            var context = new CultivationContext("seed_basic")
            {
                timeOfDay = TimeManager.Instance.CurrentTimeOfDay
            };
            
            var result = CultivationManager.Instance.AttemptCultivation(context);
            Debug.Log($"Cultivation Result: {result.success}");
            
            if (result.success)
            {
                Inventory.Instance.RemoveItem("seed_basic", 1);
                PlayerStats.Instance.AddEXP(result.outputPlant.ExpValue);
            }
        }
        
        [ContextMenu("Test Toggle Time")]
        public void TestToggleTime()
        {
            if (TimeManager.Instance.IsDay)
            {
                TimeManager.Instance.SetTimeToNight();
                Debug.Log("Time set to Night");
            }
            else
            {
                TimeManager.Instance.SetTimeToDay();
                Debug.Log("Time set to Day");
            }
        }
        
        [ContextMenu("Check All Systems")]
        public void CheckAllSystems()
        {
            Debug.Log("=== SYSTEM CHECK ===");
            Debug.Log($"Inventory Instance: {(Inventory.Instance != null ? "OK" : "MISSING")}");
            Debug.Log($"ItemDatabase Instance: {(ItemDatabase.Instance != null ? "OK" : "MISSING")}");
            Debug.Log($"PlayerStats Instance: {(PlayerStats.Instance != null ? "OK" : "MISSING")}");
            Debug.Log($"CultivationManager Instance: {(CultivationManager.Instance != null ? "OK" : "MISSING")}");
            Debug.Log($"WorldManager Instance: {(WorldManager.Instance != null ? "OK" : "MISSING")}");
            Debug.Log($"EncyclopediaSystem Instance: {(EncyclopediaSystem.Instance != null ? "OK" : "MISSING")}");
            Debug.Log($"TimeManager Instance: {(TimeManager.Instance != null ? "OK" : "MISSING")}");
            
            // Check if basic seed exists
            var basicSeed = ItemDatabase.Instance.GetItem("seed_basic");
            Debug.Log($"Basic Seed Data: {(basicSeed != null ? "FOUND" : "MISSING")}");
            
            Debug.Log("=== END SYSTEM CHECK ===");
        }
    }
}