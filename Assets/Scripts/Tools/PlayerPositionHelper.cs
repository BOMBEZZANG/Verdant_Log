using UnityEngine;
using VerdantLog.Player;

namespace VerdantLog.Tools
{
    /// <summary>
    /// Helper tool to position player for testing interactions
    /// </summary>
    public class PlayerPositionHelper : MonoBehaviour
    {
        [ContextMenu("Move Player To Seed")]
        public void MovePlayerToSeed()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError("No PlayerController found!");
                return;
            }
            
            // Find seed object
            ItemPickup[] pickups = FindObjectsOfType<ItemPickup>();
            ItemPickup seed = null;
            
            foreach (var pickup in pickups)
            {
                if (pickup.name.Contains("Seed") || pickup.name.Contains("Clova"))
                {
                    seed = pickup;
                    break;
                }
            }
            
            if (seed == null)
            {
                Debug.LogError("No seed pickup found!");
                return;
            }
            
            // Position player 1.5 units to the left of the seed (within interaction range of 2)
            Vector3 seedPos = seed.transform.position;
            Vector3 newPlayerPos = new Vector3(seedPos.x - 1.5f, seedPos.y, 0);
            
            player.transform.position = newPlayerPos;
            
            float distance = Vector3.Distance(newPlayerPos, seedPos);
            Debug.Log($"Moved player to {newPlayerPos}");
            Debug.Log($"Seed is at {seedPos}");
            Debug.Log($"Distance: {distance:F2} units (interaction range is 2 units)");
            
            if (distance <= 2f)
            {
                Debug.Log("✓ Player is now within interaction range!");
            }
            else
            {
                Debug.LogWarning("⚠ Player is still too far from seed!");
            }
        }
        
        [ContextMenu("Show Current Distances")]
        public void ShowCurrentDistances()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError("No PlayerController found!");
                return;
            }
            
            Interactable[] interactables = FindObjectsOfType<Interactable>();
            Debug.Log("=== CURRENT DISTANCES ===");
            Debug.Log($"Player position: {player.transform.position}");
            Debug.Log($"Interaction range: 2 units");
            Debug.Log("");
            
            foreach (var interactable in interactables)
            {
                float distance = Vector3.Distance(player.transform.position, interactable.transform.position);
                string status = distance <= 2f ? "✓ IN RANGE" : "✗ TOO FAR";
                Debug.Log($"{interactable.name}: {distance:F2} units - {status}");
            }
        }
        
        [ContextMenu("Move Player To Origin")]
        public void MovePlayerToOrigin()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError("No PlayerController found!");
                return;
            }
            
            player.transform.position = Vector3.zero;
            Debug.Log("Moved player to origin (0, 0, 0)");
        }
    }
}