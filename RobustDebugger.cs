using System;
using System.IO;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace SevenDtDAibot
{
    public static class RobustDebugger
    {
        private static string logPath;
        private static string errorLogPath;
        private static string exportPath;
        private static bool initialized = false;
        private static readonly object fileLock = new object();
        private static List<string> memoryBuffer = new List<string>();
        private static int maxMemoryBuffer = 1000; // Keep last 1000 entries in memory
        private static float lastExportTime = 0f;
        private const float EXPORT_COOLDOWN = 5f; // Minimum 5 seconds between exports
        
        public static void Initialize()
        {
            try
            {
                if (initialized) return;
                
                // Use project folder instead of AppData
                string logDir = Path.Combine(Application.dataPath, "..", "logs");
                if (!Directory.Exists(logDir))
                {
                    Directory.CreateDirectory(logDir);
                }
                
                logPath = Path.Combine(logDir, "esp_debug.log");
                errorLogPath = Path.Combine(logDir, "esp_errors.log");
                exportPath = Path.Combine(logDir, "exported_logs");
                
                // Ensure export directory exists
                if (!Directory.Exists(exportPath))
                {
                    Directory.CreateDirectory(exportPath);
                }
                
                initialized = true;
                
                // Clear old logs on startup
                lock (fileLock)
                {
                    try
                    {
                        if (File.Exists(logPath))
                            File.WriteAllText(logPath, $"=== Enhanced ESP Log Started {DateTime.Now} ===\n");
                        
                        if (File.Exists(errorLogPath))
                            File.WriteAllText(errorLogPath, $"=== Enhanced ESP Error Log Started {DateTime.Now} ===\n");
                    }
                    catch (IOException ex)
                    {
                        Debug.LogError($"[RobustDebugger] File access error during init: {ex.Message}");
                    }
                }
                
                Log("[RobustDebugger] Debug system initialized with export functionality");
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
                
                string timestampedMessage = $"[{DateTime.Now:HH:mm:ss}] {message}";
                
                // Add to memory buffer
                lock (memoryBuffer)
                {
                    memoryBuffer.Add(timestampedMessage);
                    if (memoryBuffer.Count > maxMemoryBuffer)
                    {
                        memoryBuffer.RemoveAt(0); // Remove oldest entry
                    }
                }
                
                // Log to Unity console
                Debug.Log(message);
                
                // Log to file with thread safety
                if (logPath != null)
                {
                    lock (fileLock)
                    {
                        try
                        {
                            File.AppendAllText(logPath, timestampedMessage + "\n");
                        }
                        catch (IOException ex)
                        {
                            Debug.LogError($"[RobustDebugger] File write error: {ex.Message}");
                        }
                    }
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
                
                string timestampedMessage = $"[{DateTime.Now:HH:mm:ss}] ERROR: {message}";
                
                // Add to memory buffer
                lock (memoryBuffer)
                {
                    memoryBuffer.Add(timestampedMessage);
                    if (memoryBuffer.Count > maxMemoryBuffer)
                    {
                        memoryBuffer.RemoveAt(0);
                    }
                }
                
                // Log to Unity console
                Debug.LogError(message);
                
                // Log to error file with thread safety
                if (errorLogPath != null)
                {
                    lock (fileLock)
                    {
                        try
                        {
                            File.AppendAllText(errorLogPath, timestampedMessage + "\n");
                        }
                        catch (IOException ex)
                        {
                            Debug.LogError($"[RobustDebugger] Error file write error: {ex.Message}");
                        }
                    }
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
                
                string timestampedMessage = $"[{DateTime.Now:HH:mm:ss}] WARNING: {message}";
                
                // Add to memory buffer
                lock (memoryBuffer)
                {
                    memoryBuffer.Add(timestampedMessage);
                    if (memoryBuffer.Count > maxMemoryBuffer)
                    {
                        memoryBuffer.RemoveAt(0);
                    }
                }
                
                // Log to Unity console
                Debug.LogWarning(message);
                
                // Log to file with thread safety
                if (logPath != null)
                {
                    lock (fileLock)
                    {
                        try
                        {
                            File.AppendAllText(logPath, timestampedMessage + "\n");
                        }
                        catch (IOException ex)
                        {
                            Debug.LogError($"[RobustDebugger] Warning file write error: {ex.Message}");
                        }
                    }
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
                
                // Log to performance file with thread safety
                lock (fileLock)
                {
                    try
                    {
                        File.AppendAllText(perfLogPath, $"[{DateTime.Now:HH:mm:ss.fff}] {message}\n");
                    }
                    catch (IOException ex)
                    {
                        LogError($"Performance logging error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Performance logging error: {ex.Message}");
            }
        }
        
        // NEW: Export logs to timestamped text file
        public static bool ExportLogs(string customSuffix = null)
        {
            try
            {
                if (!initialized) Initialize();
                
                // Check cooldown to prevent performance impact
                if (Time.time - lastExportTime < EXPORT_COOLDOWN)
                {
                    LogWarning($"Log export on cooldown ({EXPORT_COOLDOWN}s)");
                    return false;
                }
                
                float startTime = Time.realtimeSinceStartup;
                
                // Generate filename with timestamp
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
                string suffix = string.IsNullOrEmpty(customSuffix) ? "auto" : customSuffix;
                string fileName = $"esp_logs_{timestamp}_{suffix}.txt";
                string fullPath = Path.Combine(exportPath, fileName);
                
                // Build export content
                StringBuilder exportContent = new StringBuilder();
                exportContent.AppendLine($"=== 7D2D ESP Debug Log Export ===");
                exportContent.AppendLine($"Export Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                exportContent.AppendLine($"Game Version: {Application.version}");
                exportContent.AppendLine($"Unity Version: {Application.unityVersion}");
                exportContent.AppendLine($"Session Duration: {Time.realtimeSinceStartup:F2} seconds");
                exportContent.AppendLine($"Export Reason: {suffix}");
                exportContent.AppendLine("========================================");
                exportContent.AppendLine();
                
                // Add memory buffer contents
                lock (memoryBuffer)
                {
                    exportContent.AppendLine("=== Recent Log Entries (Memory Buffer) ===");
                    foreach (string entry in memoryBuffer)
                    {
                        exportContent.AppendLine(entry);
                    }
                    exportContent.AppendLine();
                }
                
                // Add current log file contents if available
                if (File.Exists(logPath))
                {
                    lock (fileLock)
                    {
                        try
                        {
                            string logContent = File.ReadAllText(logPath);
                            exportContent.AppendLine("=== Complete Debug Log ===");
                            exportContent.AppendLine(logContent);
                        }
                        catch (IOException ex)
                        {
                            exportContent.AppendLine($"Error reading log file: {ex.Message}");
                        }
                    }
                }
                
                // Add error log if available
                if (File.Exists(errorLogPath))
                {
                    lock (fileLock)
                    {
                        try
                        {
                            string errorContent = File.ReadAllText(errorLogPath);
                            exportContent.AppendLine();
                            exportContent.AppendLine("=== Error Log ===");
                            exportContent.AppendLine(errorContent);
                        }
                        catch (IOException ex)
                        {
                            exportContent.AppendLine($"Error reading error log file: {ex.Message}");
                        }
                    }
                }
                
                // Add performance summary
                exportContent.AppendLine();
                exportContent.AppendLine("=== Performance Summary ===");
                exportContent.AppendLine($"Export completed in: {(Time.realtimeSinceStartup - startTime) * 1000:F2}ms");
                exportContent.AppendLine($"Memory buffer entries: {memoryBuffer.Count}");
                exportContent.AppendLine($"Log file size: {(File.Exists(logPath) ? new FileInfo(logPath).Length : 0)} bytes");
                exportContent.AppendLine($"Error log size: {(File.Exists(errorLogPath) ? new FileInfo(errorLogPath).Length : 0)} bytes");
                
                // Write export file
                lock (fileLock)
                {
                    try
                    {
                        File.WriteAllText(fullPath, exportContent.ToString());
                        
                        float exportTime = (Time.realtimeSinceStartup - startTime) * 1000;
                        Log($"Debug logs exported to: {fileName} (took {exportTime:F2}ms)");
                        lastExportTime = Time.time;
                        return true;
                    }
                    catch (IOException ex)
                    {
                        LogError($"Failed to write export file: {ex.Message}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError($"Log export failed: {ex.Message}");
                return false;
            }
        }
        
        // NEW: Get export directory for user reference
        public static string GetExportDirectory()
        {
            if (!initialized) Initialize();
            return exportPath;
        }
        
        // NEW: Get current log directory path
        public static string GetLogDirectory()
        {
            if (!initialized) Initialize();
            return Path.GetDirectoryName(logPath);
        }
        
        // NEW: Get recent logs from memory buffer
        public static string[] GetRecentLogs(int count = 50)
        {
            lock (memoryBuffer)
            {
                int startIndex = Math.Max(0, memoryBuffer.Count - count);
                return memoryBuffer.GetRange(startIndex, memoryBuffer.Count - startIndex).ToArray();
            }
        }
        
        // NEW: Clear memory buffer (for performance management)
        public static void ClearMemoryBuffer()
        {
            lock (memoryBuffer)
            {
                memoryBuffer.Clear();
                Log("[RobustDebugger] Memory buffer cleared");
            }
        }
        
        // NEW: Automatic export on cleanup
        public static void CleanupAndExport()
        {
            try
            {
                Log("[RobustDebugger] Starting cleanup and export...");
                ExportLogs("cleanup");
                Log("[RobustDebugger] Cleanup completed");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RobustDebugger] Cleanup error: {ex.Message}");
            }
        }
    }
}
