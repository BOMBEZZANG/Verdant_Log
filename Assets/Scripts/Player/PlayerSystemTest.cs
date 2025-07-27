using UnityEngine;
using VerdantLog.Core;
using VerdantLog.Systems;
using VerdantLog.Data;

namespace VerdantLog.Player
{
    public class PlayerSystemTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool enableTestMode = false;
        [SerializeField] private ItemData testItemToSpawn;
        [SerializeField] private PlantData testPlantToSpawn;
        [SerializeField] private GameObject itemPickupPrefab;
        [SerializeField] private GameObject plantPrefab;
        
        [Header("Spawn Settings")]
        [SerializeField] private float spawnRadius = 3f;
        [SerializeField] private int itemsToSpawn = 5;
        
        private PlayerController playerController;
        private PlayerInteractor playerInteractor;
        
        private void Start()
        {
            if (!enableTestMode)
                return;
                
            playerController = GetComponent<PlayerController>();
            playerInteractor = GetComponent<PlayerInteractor>();
            
            SetupTestEnvironment();
        }
        
        private void SetupTestEnvironment()
        {
            Debug.Log("Setting up player system test environment...");
            
            // Subscribe to events for testing
            PlayerInteractor.OnInteractableDetected += OnInteractableDetected;
            PlayerInteractor.OnInteractableExited += OnInteractableExited;
            GameEvents.OnNotification += OnNotification;
            GameEvents.OnInventoryUpdated += OnInventoryUpdated;
            
            // Spawn test items
            if (testItemToSpawn != null && itemPickupPrefab != null)
            {
                SpawnTestItems();
            }
            
            // Spawn test plants
            if (testPlantToSpawn != null && plantPrefab != null)
            {
                SpawnTestPlants();
            }
        }
        
        private void OnDestroy()
        {
            if (enableTestMode)
            {
                PlayerInteractor.OnInteractableDetected -= OnInteractableDetected;
                PlayerInteractor.OnInteractableExited -= OnInteractableExited;
                GameEvents.OnNotification -= OnNotification;
                GameEvents.OnInventoryUpdated -= OnInventoryUpdated;
            }
        }
        
        private void SpawnTestItems()
        {
            if (testItemToSpawn == null)
            {
                Debug.LogWarning("Test item data is null, skipping item spawning");
                return;
            }
            
            for (int i = 0; i < itemsToSpawn; i++)
            {
                Vector2 randomPos = Random.insideUnitCircle * spawnRadius;
                Vector3 spawnPos = transform.position + new Vector3(randomPos.x, 0, randomPos.y);
                
                GameObject item = Instantiate(itemPickupPrefab, spawnPos, Quaternion.identity);
                ItemPickup pickup = item.GetComponent<ItemPickup>();
                
                if (pickup != null)
                {
                    pickup.SetItem(testItemToSpawn, Random.Range(1, 5));
                }
                
                item.name = $"TestItem_{testItemToSpawn.ItemName}_{i}";
            }
            
            Debug.Log($"Spawned {itemsToSpawn} test items");
        }
        
        private void SpawnTestPlants()
        {
            if (testPlantToSpawn == null)
            {
                Debug.LogWarning("Test plant data is null, skipping plant spawning");
                return;
            }
            
            Vector2[] plantPositions = new Vector2[]
            {
                new Vector2(2, 2),
                new Vector2(-2, 2),
                new Vector2(2, -2),
                new Vector2(-2, -2)
            };
            
            for (int i = 0; i < plantPositions.Length; i++)
            {
                Vector3 spawnPos = transform.position + new Vector3(plantPositions[i].x, 0, plantPositions[i].y);
                
                GameObject plant = Instantiate(plantPrefab, spawnPos, Quaternion.identity);
                PlantInteractable plantInteractable = plant.GetComponent<PlantInteractable>();
                
                if (plantInteractable == null)
                {
                    plantInteractable = plant.AddComponent<PlantInteractable>();
                    plant.AddComponent<Interactable>();
                }
                
                plantInteractable.SetPlantData(testPlantToSpawn);
                
                // Set random growth stages for variety
                if (testPlantToSpawn.GrowthStageSprites != null && testPlantToSpawn.GrowthStageSprites.Length > 0)
                {
                    int growthStage = Random.Range(0, testPlantToSpawn.GrowthStageSprites.Length);
                    plantInteractable.SetGrowthStage(growthStage);
                }
                else
                {
                    plantInteractable.SetGrowthStage(0);
                }
                
                plant.name = $"TestPlant_{testPlantToSpawn.PlantName}_{i}";
                plant.tag = "Plant";
            }
            
            Debug.Log($"Spawned {plantPositions.Length} test plants");
        }
        
        private void OnInteractableDetected(Interactable interactable)
        {
            Debug.Log($"[TEST] Detected interactable: {interactable.gameObject.name} - {interactable.GetInteractionName()}");
        }
        
        private void OnInteractableExited()
        {
            Debug.Log("[TEST] Exited interactable range");
        }
        
        private void OnNotification(string message)
        {
            Debug.Log($"[TEST] Notification: {message}");
        }
        
        private void OnInventoryUpdated()
        {
            Debug.Log("[TEST] Inventory updated!");
            
            var inventory = Inventory.Instance;
            if (inventory != null)
            {
                var items = inventory.GetAllItems();
                Debug.Log($"[TEST] Current inventory count: {items.Count}");
            }
        }
        
        private void Update()
        {
            if (!enableTestMode)
                return;
                
            // Test key bindings
            if (Input.GetKeyDown(KeyCode.F1))
            {
                Debug.Log("[TEST] Player Status:");
                Debug.Log($"- Position: {transform.position}");
                Debug.Log($"- State: {playerController?.GetCurrentState()}");
                Debug.Log($"- Moving: {playerController?.IsMoving()}");
                Debug.Log($"- Has Interactable: {playerInteractor?.HasInteractable()}");
            }
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                var inventory = Inventory.Instance;
                if (inventory != null)
                {
                    Debug.Log("[TEST] Inventory Contents:");
                    foreach (var item in inventory.GetAllItems())
                    {
                        var itemData = ItemDatabase.Instance.GetItem(item.itemID);
                        Debug.Log($"- {itemData?.ItemName ?? item.itemID} x{item.quantity}");
                    }
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            if (enableTestMode)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(transform.position, spawnRadius);
            }
        }
    }
}