using UnityEngine;
using VerdantLog.Utilities;
using VerdantLog.Player;
using VerdantLog.Core;

namespace VerdantLog.Tools
{
    /// <summary>
    /// Debug tool for diagnosing interaction and pickup issues
    /// </summary>
    public class InteractionDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool autoDebugOnStart = true;
        [SerializeField] private bool saveToFile = true;
        [SerializeField] private bool showGizmos = true;
        [SerializeField] private Color gizmoColor = Color.yellow;
        
        [Header("Target Objects")]
        [SerializeField] private GameObject[] targetObjects;
        
        private void Start()
        {
            if (autoDebugOnStart)
            {
                DebugAllInteractables();
            }
        }
        
        [ContextMenu("Debug All Interactables")]
        public void DebugAllInteractables()
        {
            if (saveToFile)
            {
                DebugFileLogger.StartNewLog("InteractionDebug");
            }
            
            LogDebug("=== INTERACTION SYSTEM DEBUG ===");
            LogDebug($"Debug started at: {System.DateTime.Now}");
            LogDebug("");
            
            // First, find and debug the player
            DebugPlayer();
            
            LogDebug("");
            LogDebug("---------- INTERACTABLE OBJECTS ----------");
            
            // Find all interactable objects in scene
            Interactable[] interactables = FindObjectsOfType<Interactable>();
            ItemPickup[] pickups = FindObjectsOfType<ItemPickup>();
            
            LogDebug($"Found {interactables.Length} Interactable components");
            LogDebug($"Found {pickups.Length} ItemPickup components");
            LogDebug("");
            
            // Debug each interactable
            foreach (Interactable interactable in interactables)
            {
                DebugInteractable(interactable);
                LogDebug("");
            }
            
            // Debug each pickup
            foreach (ItemPickup pickup in pickups)
            {
                DebugItemPickup(pickup);
                LogDebug("");
            }
            
            // Debug specific target objects if provided
            if (targetObjects != null && targetObjects.Length > 0)
            {
                LogDebug("---------- TARGET OBJECTS ----------");
                foreach (GameObject target in targetObjects)
                {
                    if (target != null)
                    {
                        DebugSpecificObject(target);
                        LogDebug("");
                    }
                }
            }
            
            LogDebug("=== DEBUG COMPLETE ===");
            
            if (saveToFile)
            {
                DebugFileLogger.SaveLog();
            }
        }
        
        private void DebugPlayer()
        {
            LogDebug("---------- PLAYER DEBUG ----------");
            
            // Find player
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                LogDebug("ERROR: No PlayerController found in scene!", LogType.Error);
                return;
            }
            
            LogDebug($"Player found: {player.gameObject.name}");
            LogDebug($"Player position: {player.transform.position}");
            LogDebug($"Player active: {player.gameObject.activeSelf}");
            
            // Check PlayerInteractor
            PlayerInteractor interactor = player.GetComponent<PlayerInteractor>();
            if (interactor == null)
            {
                LogDebug("ERROR: No PlayerInteractor component on player!", LogType.Error);
            }
            else
            {
                LogDebug("PlayerInteractor found");
                
                // Use reflection to get private fields
                var interactionRangeField = typeof(PlayerInteractor).GetField("interactionRange", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var layerMaskField = typeof(PlayerInteractor).GetField("interactableLayer", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (interactionRangeField != null)
                {
                    float range = (float)interactionRangeField.GetValue(interactor);
                    LogDebug($"Interaction range: {range}");
                }
                
                if (layerMaskField != null)
                {
                    LayerMask mask = (LayerMask)layerMaskField.GetValue(interactor);
                    
                    // Safely get layer name
                    string layerName = "Unknown";
                    try
                    {
                        // LayerMask.value contains multiple bits, we need to find the actual layer
                        for (int i = 0; i < 32; i++)
                        {
                            if ((mask.value & (1 << i)) != 0)
                            {
                                layerName = LayerMask.LayerToName(i);
                                if (!string.IsNullOrEmpty(layerName))
                                    break;
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        layerName = $"Error: {e.Message}";
                    }
                    
                    LogDebug($"Interactable layer mask: {mask.value} ({layerName})");
                }
            }
            
            // Check player colliders
            Collider2D[] playerColliders = player.GetComponents<Collider2D>();
            LogDebug($"Player has {playerColliders.Length} collider(s)");
            foreach (var col in playerColliders)
            {
                LogDebug($"  - {col.GetType().Name}: isTrigger={col.isTrigger}, enabled={col.enabled}");
            }
        }
        
        private void DebugInteractable(Interactable interactable)
        {
            LogDebug($"--- INTERACTABLE: {interactable.gameObject.name} ---");
            LogDebug($"Position: {interactable.transform.position}");
            LogDebug($"Active: {interactable.gameObject.activeSelf}");
            LogDebug($"Layer: {interactable.gameObject.layer} ({LayerMask.LayerToName(interactable.gameObject.layer)})");
            
            // Check colliders
            Collider2D[] colliders = interactable.GetComponents<Collider2D>();
            LogDebug($"Colliders: {colliders.Length}");
            
            if (colliders.Length == 0)
            {
                LogDebug("ERROR: No colliders found on interactable!", LogType.Error);
            }
            else
            {
                foreach (var col in colliders)
                {
                    LogDebug($"  - {col.GetType().Name}:");
                    LogDebug($"    * isTrigger: {col.isTrigger}");
                    LogDebug($"    * enabled: {col.enabled}");
                    LogDebug($"    * bounds: {col.bounds}");
                    
                    if (col is BoxCollider2D box)
                    {
                        LogDebug($"    * size: {box.size}");
                        LogDebug($"    * offset: {box.offset}");
                    }
                    else if (col is CircleCollider2D circle)
                    {
                        LogDebug($"    * radius: {circle.radius}");
                        LogDebug($"    * offset: {circle.offset}");
                    }
                }
            }
            
            // Check distance to player
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                float distance = Vector3.Distance(player.transform.position, interactable.transform.position);
                LogDebug($"Distance to player: {distance:F2}");
            }
            
            // Check if it has ItemPickup
            ItemPickup pickup = interactable.GetComponent<ItemPickup>();
            if (pickup != null)
            {
                LogDebug("Has ItemPickup component");
            }
            else
            {
                LogDebug("No ItemPickup component");
            }
        }
        
        private void DebugItemPickup(ItemPickup pickup)
        {
            LogDebug($"--- ITEM PICKUP: {pickup.gameObject.name} ---");
            LogDebug($"Position: {pickup.transform.position}");
            LogDebug($"Active: {pickup.gameObject.activeSelf}");
            
            // Use reflection to get private fields
            var itemDataField = typeof(ItemPickup).GetField("itemData", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var quantityField = typeof(ItemPickup).GetField("quantity", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (itemDataField != null)
            {
                var itemData = itemDataField.GetValue(pickup);
                if (itemData != null)
                {
                    LogDebug($"Item Data: {itemData}");
                }
                else
                {
                    LogDebug($"WARNING: ItemData is null for {pickup.gameObject.name}! This pickup won't work.", LogType.Warning);
                }
            }
            
            if (quantityField != null)
            {
                int quantity = (int)quantityField.GetValue(pickup);
                LogDebug($"Quantity: {quantity}");
            }
            
            // Check if it has Interactable
            Interactable interactable = pickup.GetComponent<Interactable>();
            if (interactable != null)
            {
                LogDebug("Has Interactable component");
            }
            else
            {
                LogDebug("ERROR: No Interactable component!", LogType.Error);
            }
        }
        
        private void DebugSpecificObject(GameObject obj)
        {
            LogDebug($"--- SPECIFIC OBJECT: {obj.name} ---");
            LogDebug($"Position: {obj.transform.position}");
            LogDebug($"Active: {obj.activeSelf}");
            LogDebug($"Layer: {obj.layer} ({LayerMask.LayerToName(obj.layer)})");
            LogDebug($"Tag: {obj.tag}");
            
            // List all components
            Component[] components = obj.GetComponents<Component>();
            LogDebug($"Components ({components.Length}):");
            foreach (var comp in components)
            {
                LogDebug($"  - {comp.GetType().Name}");
            }
            
            // Check colliders specifically
            Collider2D[] colliders = obj.GetComponents<Collider2D>();
            if (colliders.Length > 0)
            {
                LogDebug("Collider Details:");
                foreach (var col in colliders)
                {
                    LogDebug($"  - {col.GetType().Name}: trigger={col.isTrigger}, enabled={col.enabled}");
                }
            }
            
            // Check distance to player
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                float distance = Vector3.Distance(player.transform.position, obj.transform.position);
                LogDebug($"Distance to player: {distance:F2}");
                
                // Check if player can "see" this object with a raycast
                Vector3 direction = (obj.transform.position - player.transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(player.transform.position, direction, distance + 1f);
                if (hit.collider != null)
                {
                    LogDebug($"Raycast hit: {hit.collider.gameObject.name}");
                    if (hit.collider.gameObject == obj)
                    {
                        LogDebug("Player has clear line of sight to object");
                    }
                    else
                    {
                        LogDebug($"Object blocked by: {hit.collider.gameObject.name}");
                    }
                }
            }
        }
        
        [ContextMenu("Test Interaction Range")]
        public void TestInteractionRange()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError("No player found!");
                return;
            }
            
            PlayerInteractor interactor = player.GetComponent<PlayerInteractor>();
            if (interactor == null)
            {
                Debug.LogError("No PlayerInteractor found!");
                return;
            }
            
            // Find all interactables within a reasonable distance
            Interactable[] allInteractables = FindObjectsOfType<Interactable>();
            
            Debug.Log("=== INTERACTION RANGE TEST ===");
            foreach (var interactable in allInteractables)
            {
                float distance = Vector3.Distance(player.transform.position, interactable.transform.position);
                Debug.Log($"{interactable.gameObject.name}: distance = {distance:F2}");
                
                if (distance <= 3f) // Reasonable interaction range
                {
                    Debug.Log($"  -> Within likely interaction range");
                }
            }
        }
        
        private void LogDebug(string message, LogType logType = LogType.Log)
        {
            if (saveToFile)
            {
                DebugFileLogger.Log(message, logType);
            }
            
            // Also output to Unity console
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError($"[InteractionDebug] {message}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"[InteractionDebug] {message}");
                    break;
                default:
                    Debug.Log($"[InteractionDebug] {message}");
                    break;
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos)
                return;
                
            // Draw gizmos for player interaction range
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                PlayerInteractor interactor = player.GetComponent<PlayerInteractor>();
                if (interactor != null)
                {
                    // Try to get interaction range via reflection
                    var rangeField = typeof(PlayerInteractor).GetField("interactionRange", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    float range = 2f; // default fallback
                    if (rangeField != null)
                    {
                        range = (float)rangeField.GetValue(interactor);
                    }
                    
                    Gizmos.color = gizmoColor;
                    Gizmos.DrawWireSphere(player.transform.position, range);
                }
            }
            
            // Draw gizmos for target objects
            if (targetObjects != null)
            {
                Gizmos.color = Color.red;
                foreach (var obj in targetObjects)
                {
                    if (obj != null)
                    {
                        Gizmos.DrawWireCube(obj.transform.position, Vector3.one * 0.5f);
                        
                        // Draw line to player if exists
                        if (player != null)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(player.transform.position, obj.transform.position);
                        }
                    }
                }
            }
        }
    }
}