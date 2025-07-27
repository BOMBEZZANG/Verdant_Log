using UnityEngine;
using VerdantLog.Utilities;

namespace VerdantLog.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerVisualDebug : MonoBehaviour
    {
        [Header("Debug Settings")]
        [SerializeField] private bool autoFixVisuals = true;
        [SerializeField] private bool saveDebugLog = true;
        [SerializeField] private Sprite debugSprite;
        [SerializeField] private Color debugColor = Color.white;
        
        [Header("References")]
        [SerializeField] private SpriteRenderer playerSprite;
        
        private void Start()
        {
            DiagnoseAndFix();
        }
        
        [ContextMenu("Debug Hierarchy")]
        public void DebugHierarchy()
        {
            Debug.Log($"=== Player Hierarchy Debug ===");
            Debug.Log($"Player GameObject: '{gameObject.name}'");
            Debug.Log($"Child Count: {transform.childCount}");
            
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                Debug.Log($"  Child {i}: '{child.name}' (active: {child.gameObject.activeSelf})");
                
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Debug.Log($"    - Has SpriteRenderer");
                    Debug.Log($"    - Sprite: {(sr.sprite != null ? sr.sprite.name : "null")}");
                    Debug.Log($"    - Color: {sr.color}");
                }
            }
            
            // Check if GetComponentInChildren works
            SpriteRenderer anySR = GetComponentInChildren<SpriteRenderer>();
            if (anySR != null)
            {
                Debug.Log($"GetComponentInChildren found SpriteRenderer on: '{anySR.gameObject.name}'");
            }
            else
            {
                Debug.Log("GetComponentInChildren found NO SpriteRenderer!");
            }
        }
        
        [ContextMenu("Diagnose Visual Issues")]
        public void DiagnoseAndFix()
        {
            if (saveDebugLog)
            {
                DebugFileLogger.StartNewLog("PlayerVisualDebug");
            }
            
            LogDebug("=== Player Visual Diagnosis ===");
            
            // Find or create visual object
            Transform visualTransform = null;
            
            // First try direct Find
            visualTransform = transform.Find("Visual");
            
            // If not found, try case-insensitive search through all children
            if (visualTransform == null)
            {
                foreach (Transform child in transform)
                {
                    if (child.name.Equals("Visual", System.StringComparison.OrdinalIgnoreCase))
                    {
                        visualTransform = child;
                        LogDebug($"Found Visual child with name: '{child.name}'");
                        break;
                    }
                }
            }
            
            // If still not found, search for any child with SpriteRenderer
            if (visualTransform == null)
            {
                SpriteRenderer childSprite = GetComponentInChildren<SpriteRenderer>();
                if (childSprite != null && childSprite.transform != transform)
                {
                    visualTransform = childSprite.transform;
                    LogDebug($"Found child with SpriteRenderer: '{visualTransform.name}'", LogType.Warning);
                }
            }
            
            if (visualTransform == null)
            {
                LogDebug("Visual object not found! Creating one...", LogType.Warning);
                if (autoFixVisuals)
                {
                    GameObject visual = new GameObject("Visual");
                    visual.transform.SetParent(transform, false);
                    visualTransform = visual.transform;
                }
            }
            else
            {
                LogDebug($"Visual object found: '{visualTransform.name}' (active: {visualTransform.gameObject.activeSelf})");
            }
            
            // Check SpriteRenderer
            if (visualTransform != null)
            {
                playerSprite = visualTransform.GetComponent<SpriteRenderer>();
                if (playerSprite == null)
                {
                    LogDebug("SpriteRenderer not found on Visual object!");
                    if (autoFixVisuals)
                    {
                        playerSprite = visualTransform.gameObject.AddComponent<SpriteRenderer>();
                        LogDebug("Added SpriteRenderer component");
                    }
                }
            }
            
            if (playerSprite != null)
            {
                // Check sprite assignment
                if (playerSprite.sprite == null)
                {
                    LogDebug("No sprite assigned to SpriteRenderer!");
                    if (autoFixVisuals && debugSprite != null)
                    {
                        playerSprite.sprite = debugSprite;
                        LogDebug("Assigned debug sprite");
                    }
                    else if (autoFixVisuals)
                    {
                        // Create a simple colored square as placeholder
                        CreatePlaceholderSprite();
                    }
                }
                else
                {
                    LogDebug($"Sprite assigned: {playerSprite.sprite.name}");
                }
                
                // Check sorting layer
                LogDebug($"Sorting Layer: {playerSprite.sortingLayerName} (ID: {playerSprite.sortingLayerID})");
                LogDebug($"Order in Layer: {playerSprite.sortingOrder}");
                
                if (autoFixVisuals)
                {
                    // Ensure proper sorting
                    playerSprite.sortingLayerName = "Default"; // or "Player" if it exists
                    playerSprite.sortingOrder = 10;
                    LogDebug("Fixed sorting layer settings");
                }
                
                // Check color and transparency
                LogDebug($"Sprite Color: {playerSprite.color}");
                if (playerSprite.color.a < 0.1f)
                {
                    LogDebug("Sprite is nearly transparent!");
                    if (autoFixVisuals)
                    {
                        playerSprite.color = debugColor;
                        LogDebug("Fixed sprite transparency");
                    }
                }
                
                // Check scale
                Vector3 scale = visualTransform.localScale;
                LogDebug($"Visual Scale: {scale}");
                if (scale.x == 0 || scale.y == 0)
                {
                    LogDebug("Visual scale is zero!");
                    if (autoFixVisuals)
                    {
                        visualTransform.localScale = Vector3.one;
                        LogDebug("Fixed visual scale");
                    }
                }
            }
            
            // Check player position relative to camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                Vector3 playerPos = transform.position;
                Vector3 camPos = mainCam.transform.position;
                
                LogDebug($"Player Position: {playerPos}");
                LogDebug($"Camera Position: {camPos}");
                
                if (mainCam.orthographic)
                {
                    LogDebug($"Camera Size: {mainCam.orthographicSize}");
                    
                    // Check if player is in camera view
                    Vector3 viewPos = mainCam.WorldToViewportPoint(playerPos);
                    LogDebug($"Player in viewport: {viewPos}");
                    
                    if (viewPos.x < -0.1f || viewPos.x > 1.1f || viewPos.y < -0.1f || viewPos.y > 1.1f)
                    {
                        LogDebug("Player is outside camera view!");
                        if (autoFixVisuals)
                        {
                            transform.position = new Vector3(camPos.x, camPos.y, 0);
                            LogDebug("Moved player to camera center");
                        }
                    }
                }
                else
                {
                    LogDebug("Camera is in Perspective mode. For 2D games, use Orthographic!");
                }
                
                // Check Z positions
                if (playerPos.z >= camPos.z)
                {
                    LogDebug($"Player Z ({playerPos.z}) is behind or at camera Z ({camPos.z})!");
                    if (autoFixVisuals)
                    {
                        transform.position = new Vector3(playerPos.x, playerPos.y, 0);
                        LogDebug("Fixed player Z position");
                    }
                }
            }
            else
            {
                LogDebug("No Main Camera found in scene!");
            }
            
            // Check if player components are set up correctly
            PlayerController controller = GetComponent<PlayerController>();
            if (controller != null)
            {
                LogDebug("PlayerController found and active");
            }
            
            LogDebug("=== Diagnosis Complete ===");
            
            // Save the debug log
            if (saveDebugLog)
            {
                DebugFileLogger.SaveLog();
            }
        }
        
        private void LogDebug(string message, LogType logType = LogType.Log)
        {
            if (saveDebugLog)
            {
                DebugFileLogger.Log(message, logType);
            }
            else
            {
                switch (logType)
                {
                    case LogType.Error:
                        LogDebug(message);
                        break;
                    case LogType.Warning:
                        LogDebug(message);
                        break;
                    default:
                        LogDebug(message);
                        break;
                }
            }
        }
        
        private void CreatePlaceholderSprite()
        {
            // Create a simple white square texture
            Texture2D tex = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.white;
            }
            tex.SetPixels(pixels);
            tex.Apply();
            
            // Create sprite from texture
            Sprite placeholder = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            playerSprite.sprite = placeholder;
            playerSprite.color = debugColor;
            
            LogDebug("Created placeholder sprite");
        }
        
        private void OnDrawGizmos()
        {
            // Draw a visible gizmo at player position
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, Vector3.one);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2);
        }
    }
}