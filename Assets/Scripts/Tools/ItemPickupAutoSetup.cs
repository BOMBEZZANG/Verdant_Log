#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using VerdantLog.Player;

namespace VerdantLog.Tools
{
    /// <summary>
    /// Automatically sets up ItemPickup connections for Interactable objects
    /// </summary>
    public class ItemPickupAutoSetup : MonoBehaviour
    {
        [ContextMenu("Auto-Setup All ItemPickups")]
        public void AutoSetupAllItemPickups()
        {
            ItemPickup[] pickups = FindObjectsOfType<ItemPickup>();
            int setupCount = 0;
            
            Debug.Log($"Found {pickups.Length} ItemPickup components");
            
            foreach (ItemPickup pickup in pickups)
            {
                if (SetupItemPickup(pickup))
                {
                    setupCount++;
                }
            }
            
            Debug.Log($"Successfully set up {setupCount} ItemPickups");
        }
        
        [ContextMenu("Setup Selected Object")]
        public void SetupSelectedObject()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected == null)
            {
                Debug.LogWarning("No object selected!");
                return;
            }
            
            ItemPickup pickup = selected.GetComponent<ItemPickup>();
            if (pickup == null)
            {
                Debug.LogWarning("Selected object has no ItemPickup component!");
                return;
            }
            
            if (SetupItemPickup(pickup))
            {
                Debug.Log($"Successfully set up {selected.name}");
            }
        }
        
        private bool SetupItemPickup(ItemPickup pickup)
        {
            Interactable interactable = pickup.GetComponent<Interactable>();
            if (interactable == null)
            {
                Debug.LogWarning($"{pickup.gameObject.name} has no Interactable component!");
                return false;
            }
            
            // Use reflection to access the private onInteract field
            var interactableType = typeof(Interactable);
            var onInteractField = interactableType.GetField("onInteract", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (onInteractField == null)
            {
                Debug.LogError("Could not find onInteract field in Interactable!");
                return false;
            }
            
            var onInteractEvent = onInteractField.GetValue(interactable) as UnityEngine.Events.UnityEvent;
            if (onInteractEvent == null)
            {
                Debug.LogError("onInteract is not a UnityEvent!");
                return false;
            }
            
            // Clear existing listeners and add our pickup method
            onInteractEvent.RemoveAllListeners();
            onInteractEvent.AddListener(pickup.OnPickup);
            
            Debug.Log($"✓ Connected {pickup.gameObject.name} pickup to interaction event");
            return true;
        }
    }
}
#endif