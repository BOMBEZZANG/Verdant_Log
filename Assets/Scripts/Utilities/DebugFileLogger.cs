using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace VerdantLog.Utilities
{
    public static class DebugFileLogger
    {
        private static StringBuilder logBuffer = new StringBuilder();
        private static string currentLogPath = null;
        private static bool isInitialized = false;
        
        public static string LogDirectory => Path.Combine(Application.dataPath, "..", "DebugLogs");
        
        /// <summary>
        /// Initialize a new log file with timestamp
        /// </summary>
        public static void StartNewLog(string prefix = "DebugLog")
        {
            try
            {
                // Ensure directory exists
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }
                
                // Create filename with timestamp
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string filename = $"{prefix}_{timestamp}.txt";
                currentLogPath = Path.Combine(LogDirectory, filename);
                
                // Clear buffer
                logBuffer.Clear();
                
                // Write header
                logBuffer.AppendLine("==============================================");
                logBuffer.AppendLine($"Debug Log Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                logBuffer.AppendLine($"Unity Version: {Application.unityVersion}");
                logBuffer.AppendLine($"Platform: {Application.platform}");
                logBuffer.AppendLine($"Project: {Application.productName}");
                logBuffer.AppendLine("==============================================");
                logBuffer.AppendLine();
                
                isInitialized = true;
                
                Debug.Log($"Debug log started: {currentLogPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create debug log: {e.Message}");
            }
        }
        
        /// <summary>
        /// Log a message with timestamp
        /// </summary>
        public static void Log(string message, LogType logType = LogType.Log)
        {
            if (!isInitialized)
            {
                StartNewLog();
            }
            
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logTypeStr = logType.ToString().ToUpper().PadRight(7);
            string formattedMessage = $"[{timestamp}] [{logTypeStr}] {message}";
            
            logBuffer.AppendLine(formattedMessage);
        }
        
        /// <summary>
        /// Log a message with timestamp and also output to Unity console
        /// </summary>
        public static void LogWithConsole(string message, LogType logType = LogType.Log)
        {
            Log(message, logType);
            
            // Also log to Unity console
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }
        
        /// <summary>
        /// Log with specific category
        /// </summary>
        public static void LogCategory(string category, string message, LogType logType = LogType.Log)
        {
            Log($"[{category}] {message}", logType);
        }
        
        /// <summary>
        /// Log a separator line
        /// </summary>
        public static void LogSeparator(string title = null)
        {
            if (!isInitialized)
            {
                StartNewLog();
            }
            
            logBuffer.AppendLine();
            if (string.IsNullOrEmpty(title))
            {
                logBuffer.AppendLine("----------------------------------------------");
            }
            else
            {
                logBuffer.AppendLine($"---------- {title} ----------");
            }
            logBuffer.AppendLine();
        }
        
        /// <summary>
        /// Save the current log buffer to file
        /// </summary>
        /// <param name="openFolder">Whether to open the folder in file explorer after saving</param>
        public static void SaveLog(bool openFolder = false)
        {
            if (!isInitialized || string.IsNullOrEmpty(currentLogPath))
            {
                Debug.LogWarning("No log initialized to save");
                return;
            }
            
            try
            {
                // Add footer
                logBuffer.AppendLine();
                logBuffer.AppendLine("==============================================");
                logBuffer.AppendLine($"Debug Log Ended: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                logBuffer.AppendLine("==============================================");
                
                // Write to file
                File.WriteAllText(currentLogPath, logBuffer.ToString());
                
                Debug.Log($"Debug log saved to: {currentLogPath}");
                
                // Only open folder if requested
                if (openFolder)
                {
                    OpenLogFolder();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save debug log: {e.Message}");
            }
        }
        
        /// <summary>
        /// Open the log folder in file explorer
        /// </summary>
        public static void OpenLogFolder()
        {
            try
            {
                #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                System.Diagnostics.Process.Start("explorer.exe", "/select," + currentLogPath.Replace('/', '\\'));
                #elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
                System.Diagnostics.Process.Start("open", "-R " + currentLogPath);
                #else
                Debug.Log($"Opening folder not supported on this platform. Path: {LogDirectory}");
                #endif
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to open log folder: {e.Message}");
            }
        }
        
        /// <summary>
        /// Get the current log as string
        /// </summary>
        public static string GetLogContent()
        {
            return logBuffer.ToString();
        }
        
        /// <summary>
        /// Clear the current log buffer
        /// </summary>
        public static void ClearLog()
        {
            logBuffer.Clear();
            Log("Log buffer cleared");
        }
        
        /// <summary>
        /// Log object details
        /// </summary>
        public static void LogObject(string name, object obj)
        {
            if (obj == null)
            {
                Log($"{name}: null");
                return;
            }
            
            Log($"{name}: {obj.GetType().Name}");
            
            // Log Unity specific object details
            if (obj is UnityEngine.Object unityObj)
            {
                Log($"  Name: {unityObj.name}");
                
                if (obj is Component component)
                {
                    Log($"  GameObject: {component.gameObject.name}");
                    Log($"  Active: {component.gameObject.activeSelf}");
                    
                    if (obj is Transform transform)
                    {
                        Log($"  Position: {transform.position}");
                        Log($"  Rotation: {transform.rotation.eulerAngles}");
                        Log($"  Scale: {transform.localScale}");
                    }
                    else if (obj is SpriteRenderer spriteRenderer)
                    {
                        Log($"  Sprite: {(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "null")}");
                        Log($"  Color: {spriteRenderer.color}");
                        Log($"  Sorting Layer: {spriteRenderer.sortingLayerName}");
                        Log($"  Order in Layer: {spriteRenderer.sortingOrder}");
                    }
                }
            }
            else
            {
                Log($"  ToString: {obj}");
            }
        }
    }
    
    /// <summary>
    /// Auto-save logs when application quits or loses focus
    /// </summary>
    public class DebugLoggerAutoSave : MonoBehaviour
    {
        private static DebugLoggerAutoSave instance;
        [SerializeField] private float autoSaveInterval = 30f; // Save every 30 seconds
        private float lastSaveTime;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("DebugLoggerAutoSave");
                instance = go.AddComponent<DebugLoggerAutoSave>();
                DontDestroyOnLoad(go);
                
                // Initialize the logger
                DebugFileLogger.StartNewLog("GameSession");
                
                // Hook into Unity's logging system to capture all Debug messages
                Application.logMessageReceived += OnLogMessageReceived;
                
                Debug.Log("DebugFileLogger initialized and ready to capture messages");
            }
        }
        
        private void Start()
        {
            lastSaveTime = Time.time;
        }
        
        private void Update()
        {
            // Auto-save periodically
            if (Time.time - lastSaveTime >= autoSaveInterval)
            {
                DebugFileLogger.SaveLog();
                lastSaveTime = Time.time;
            }
        }
        
        private static void OnLogMessageReceived(string logString, string stackTrace, LogType type)
        {
            // Capture all Unity Debug messages automatically
            DebugFileLogger.Log(logString, type);
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                DebugFileLogger.SaveLog();
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                DebugFileLogger.SaveLog();
            }
        }
        
        private void OnApplicationQuit()
        {
            DebugFileLogger.SaveLog();
        }
        
        private void OnDestroy()
        {
            if (instance == this)
            {
                // Unsubscribe from Unity logging
                Application.logMessageReceived -= OnLogMessageReceived;
                
                DebugFileLogger.SaveLog();
                instance = null;
            }
        }
    }
}