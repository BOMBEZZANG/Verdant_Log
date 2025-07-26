using UnityEngine;
using VerdantLog.Systems;
using VerdantLog.Data;

namespace VerdantLog.Core
{
    public class TestSystemIntegration : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool enableTestKeys = true;
        
        private void Start()
        {
            if (enableTestKeys)
            {
                Debug.Log("Test Keys Enabled:");
                Debug.Log("1 - Add Basic Seed");
                Debug.Log("2 - Add Mystic Seed");
                Debug.Log("3 - Add Crystal Seed + Crystal Dust");
                Debug.Log("4 - Test Basic Cultivation");
                Debug.Log("5 - Test Night Cultivation");
                Debug.Log("6 - Test Crystal Cultivation");
                Debug.Log("7 - Add 100 EXP");
                Debug.Log("8 - Check Zone Unlocks");
                Debug.Log("T - Toggle Day/Night");
            }
        }
        
        private void Update()
        {
            if (!enableTestKeys)
                return;
                
            // Add items
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Inventory.Instance.AddItem("seed_basic", 5);
                GameEvents.TriggerNotification("Added 5 Basic Seeds");
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Inventory.Instance.AddItem("seed_mystic", 3);
                GameEvents.TriggerNotification("Added 3 Mystic Seeds");
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Inventory.Instance.AddItem("seed_crystal", 2);
                Inventory.Instance.AddItem("mat_crystal_dust", 5);
                GameEvents.TriggerNotification("Added 2 Crystal Seeds and 5 Crystal Dust");
            }
            
            // Test cultivation
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TestCultivation("seed_basic", null);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                TestCultivation("seed_mystic", null);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                TestCultivation("seed_crystal", "mat_crystal_dust");
            }
            
            // Add EXP
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                PlayerStats.Instance.AddEXP(100);
            }
            
            // Check zones
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                var zones = WorldManager.Instance.GetUnlockedZones();
                string message = "Unlocked Zones: ";
                foreach (var zone in zones)
                {
                    message += zone.ZoneName + ", ";
                }
                GameEvents.TriggerNotification(message);
            }
            
            // Toggle time
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (TimeManager.Instance.IsDay)
                {
                    TimeManager.Instance.SetTimeToNight();
                    GameEvents.TriggerNotification("Time set to Night");
                }
                else
                {
                    TimeManager.Instance.SetTimeToDay();
                    GameEvents.TriggerNotification("Time set to Day");
                }
            }
        }
        
        private void TestCultivation(string seedID, string itemUsedID)
        {
            if (!Inventory.Instance.HasItem(seedID))
            {
                GameEvents.TriggerNotification($"You don't have any {seedID}!");
                return;
            }
            
            var context = new CultivationContext(seedID)
            {
                timeOfDay = TimeManager.Instance.CurrentTimeOfDay,
                itemUsedID = itemUsedID
            };
            
            if (!string.IsNullOrEmpty(itemUsedID))
            {
                if (!Inventory.Instance.HasItem(itemUsedID))
                {
                    GameEvents.TriggerNotification($"You don't have any {itemUsedID}!");
                    return;
                }
                
                Inventory.Instance.RemoveItem(itemUsedID, 1);
            }
            
            var result = CultivationManager.Instance.AttemptCultivation(context);
            
            if (result.success)
            {
                Inventory.Instance.RemoveItem(seedID, 1);
                
                if (result.outputPlant != null)
                {
                    Inventory.Instance.AddItem(result.outputPlant.HarvestYield.ItemID, result.outputPlant.HarvestYieldAmount);
                    PlayerStats.Instance.AddEXP(result.outputPlant.ExpValue);
                }
            }
        }
    }
}