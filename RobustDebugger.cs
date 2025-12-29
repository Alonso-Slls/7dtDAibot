using System;
using System.IO;
using UnityEngine;

namespace SevenDtDAibot
{
    public static class RobustDebugger
    {
        private static string logPath;
        private static string errorLogPath;
        private static bool initialized = false;
        
        public static void Initialize()
        {
            try
            {
                if (initialized) return;
                
                string logDir = Path.Combine(Application.persistentDataPath, "7dtDAibot", "logs");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                
                logPath = Path.Combine(logDir, "esp_debug.log");
                errorLogPath = Path.Combine(logDir, "esp_errors.log");
                
                initialized = true;
                
                // Clear old logs on startup
                if (File.Exists(logPath))
                    File.WriteAllText(logPath, $"=== Enhanced ESP Log Started {DateTime.Now} ===\n");
                
                if (File.Exists(errorLogPath))
                    File.WriteAllText(errorLogPath, $"=== Enhanced ESP Error Log Started {DateTime.Now} ===\n");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RobustDebugger] Failed to initialize: {ex.Message}");
            }
        }
        
        public static void Log(string message)
        {
            try
            {
                if (!initialized) Initialize();
                
                // Log to Unity console
                Debug.Log(message);
                
                // Log to file
                if (logPath != null)
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss}] {message}\n");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RobustDebugger] Logging error: {ex.Message}");
            }
        }
        
        public static void LogError(string message)
        {
            try
            {
                if (!initialized) Initialize();
                
                // Log to Unity console
                Debug.LogError(message);
                
                // Log to error file
                if (errorLogPath != null)
                {
                    File.AppendAllText(errorLogPath, $"[{DateTime.Now:HH:mm:ss}] ERROR: {message}\n");
                }
                
                // Also log to main log
                Log($"ERROR: {message}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RobustDebugger] Error logging error: {ex.Message}");
            }
        }
        
        public static void LogWarning(string message)
        {
            try
            {
                if (!initialized) Initialize();
                
                // Log to Unity console
                Debug.LogWarning(message);
                
                // Log to file
                if (logPath != null)
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now:HH:mm:ss}] WARNING: {message}\n");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RobustDebugger] Warning logging error: {ex.Message}");
            }
        }
        
        public static void LogPerformance(string message)
        {
            try
            {
                if (!initialized) Initialize();
                
                string perfLogPath = Path.Combine(Path.GetDirectoryName(logPath), "esp_performance.log");
                
                // Log to performance file
                File.AppendAllText(perfLogPath, $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
            }
            catch (Exception ex)
            {
                LogError($"Performance logging error: {ex.Message}");
            }
        }
    }
}
