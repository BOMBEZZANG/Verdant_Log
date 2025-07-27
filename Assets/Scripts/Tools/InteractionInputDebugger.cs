using UnityEngine;
using UnityEngine.InputSystem;
using VerdantLog.Player;
using VerdantLog.Utilities;

namespace VerdantLog.Tools
{
    /// <summary>
    /// Debug tool specifically for testing interaction input handling
    /// </summary>
    public class InteractionInputDebugger : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool saveToFile = true;
        
        private PlayerInteractor playerInteractor;
        private PlayerInput playerInput;
        
        private void Start()
        {
            if (enableDebugLogs && saveToFile)
            {
                DebugFileLogger.StartNewLog("InteractionInputDebug");
            }
            
            // Find components
            playerInteractor = FindObjectOfType<PlayerInteractor>();
            playerInput = FindObjectOfType<PlayerInput>();
            
            LogDebug("=== INTERACTION INPUT DEBUG ===");
            LogDebug($"PlayerInteractor found: {playerInteractor != null}");
            LogDebug($"PlayerInput found: {playerInput != null}");
            
            if (playerInput != null)
            {
                LogDebug($"PlayerInput GameObject: {playerInput.gameObject.name}");
                LogDebug($"PlayerInput enabled: {playerInput.enabled}");
                
                // Check if PlayerInput has the interact action
                if (playerInput.actions != null)
                {
                    var interactAction = playerInput.actions.FindAction("Interact");
                    if (interactAction != null)
                    {
                        LogDebug($"Interact action found: {interactAction.name}");
                        LogDebug($"Interact action enabled: {interactAction.enabled}");
                        LogDebug($"Interact action bindings: {interactAction.bindings.Count}");
                        
                        for (int i = 0; i < interactAction.bindings.Count; i++)
                        {
                            LogDebug($"  Binding {i}: {interactAction.bindings[i].effectivePath}");
                        }
                    }
                    else
                    {
                        LogDebug("ERROR: No 'Interact' action found!", LogType.Error);
                    }
                }
                else
                {
                    LogDebug("ERROR: PlayerInput.actions is null!", LogType.Error);
                }
            }
            
            if (saveToFile)
            {
                DebugFileLogger.SaveLog();
            }
        }
        
        private void Update()
        {
            // Monitor input manually
            if (Input.GetKeyDown(KeyCode.E))
            {
                LogDebug("E key pressed (old input system)");
                TestManualInteraction();
            }
            
            // Check if new input system detects E
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                LogDebug("E key pressed (new input system)");
            }
        }
        
        [ContextMenu("Test Manual Interaction")]
        public void TestManualInteraction()
        {
            if (playerInteractor == null)
            {
                LogDebug("No PlayerInteractor found!", LogType.Error);
                return;
            }
            
            LogDebug("=== MANUAL INTERACTION TEST ===");
            
            // Try to manually trigger interaction using reflection
            var method = typeof(PlayerInteractor).GetMethod("OnInteract", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            
            if (method != null)
            {
                LogDebug("Found OnInteract method, attempting to call...");
                try
                {
                    // Create a mock InputValue
                    method.Invoke(playerInteractor, new object[] { CreateMockInputValue() });
                    LogDebug("OnInteract method called successfully");
                }
                catch (System.Exception e)
                {
                    LogDebug($"Error calling OnInteract: {e.Message}", LogType.Error);
                }
            }
            else
            {
                LogDebug("OnInteract method not found!", LogType.Error);
                
                // List all public methods
                var methods = typeof(PlayerInteractor).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                LogDebug("Available public methods:");
                foreach (var m in methods)
                {
                    LogDebug($"  - {m.Name}");
                }
            }
        }
        
        [ContextMenu("Move Player To Seed")]
        public void MovePlayerToSeed()
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                LogDebug("No player found!", LogType.Error);
                return;
            }
            
            // Find the seed
            var interactables = FindObjectsOfType<Interactable>();
            GameObject seed = null;
            
            foreach (var interactable in interactables)
            {
                if (interactable.name.Contains("Seed") && interactable.GetComponent<ItemPickup>() != null)
                {
                    seed = interactable.gameObject;
                    break;
                }
            }
            
            if (seed != null)
            {
                Vector3 seedPos = seed.transform.position;
                Vector3 playerPos = new Vector3(seedPos.x - 1.5f, seedPos.y, 0); // Position player 1.5 units away
                player.transform.position = playerPos;
                
                LogDebug($"Moved player to {playerPos}, seed at {seedPos}");
                LogDebug($"Distance: {Vector3.Distance(playerPos, seedPos):F2}");
            }
            else
            {
                LogDebug("No seed object found!", LogType.Error);
            }
        }
        
        private InputValue CreateMockInputValue()
        {
            // This is a bit tricky, InputValue is sealed and has no public constructor
            // We'll try to simulate what happens when a button is pressed
            return null; // This might cause issues, but let's see what happens
        }
        
        private void LogDebug(string message, LogType logType = LogType.Log)
        {
            if (!enableDebugLogs)
                return;
                
            if (saveToFile)
            {
                DebugFileLogger.Log(message, logType);
            }
            
            // Also output to Unity console
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError($"[InteractionInputDebug] {message}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"[InteractionInputDebug] {message}");
                    break;
                default:
                    Debug.Log($"[InteractionInputDebug] {message}");
                    break;
            }
        }
    }
}