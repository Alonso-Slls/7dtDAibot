using System;
using System.IO;
using System.Threading;
using UnityEngine;

namespace SevenDtDAibot
{
    /// <summary>
    /// Robust file-based debugger for ESP framework.
    /// Provides comprehensive logging with rotation and performance tracking.
    /// </summary>
    public static class RobustDebugger
    {
        private static readonly string LogDirectory = "logs";
        private static readonly string LogFileName = "esp_debug.log";
        private static readonly string LogFilePath = Path.Combine(LogDirectory, LogFileName);
        private static readonly string ErrorLogFileName = "esp_errors.log";
        private static readonly string ErrorLogFilePath = Path.Combine(LogDirectory, ErrorLogFileName);
        private static readonly string PerformanceLogFileName = "esp_performance.log";
        private static readonly string PerformanceLogFilePath = Path.Combine(LogDirectory, PerformanceLogFileName);
        
        private static readonly object _lockObject = new object();
        private static readonly int MaxLogFileSize = 10 * 1024 * 1024; // 10MB
        private static readonly int MaxLogFiles = 5;
        
        private static bool _initialized = false;
        private static int _frameCount = 0;
        private static float _lastLogTime = 0f;
        private static System.Diagnostics.Stopwatch _performanceStopwatch;
        
        /// <summary>
        /// Initialize the debugger system.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                lock (_lockObject)
                {
                    if (_initialized) return;
                    
                    // Ensure log directory exists
                    if (!Directory.Exists(LogDirectory))
                    {
                        Directory.CreateDirectory(LogDirectory);
                        LogInfo("RobustDebugger", $"Created log directory: {LogDirectory}");
                    }
                    
                    // Rotate old logs if they're too large
                    RotateLogIfNeeded(LogFilePath);
                    RotateLogIfNeeded(ErrorLogFilePath);
                    RotateLogIfNeeded(PerformanceLogFilePath);
                    
                    // Initialize performance tracking
                    _performanceStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    
                    _initialized = true;
                    
                    LogInfo("RobustDebugger", "Debugger initialized successfully");
                    LogInfo("RobustDebugger", $"Log file: {Path.GetFullPath(LogFilePath)}");
                    LogInfo("RobustDebugger", $"Error log: {Path.GetFullPath(ErrorLogFilePath)}");
                    LogInfo("RobustDebugger", $"Performance log: {Path.GetFullPath(PerformanceLogFilePath)}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RobustDebugger] Failed to initialize: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Log an informational message.
        /// </summary>
        public static void LogInfo(string source, string message)
        {
            LogMessage("INFO", source, message);
        }
        
        /// <summary>
        /// Log a warning message.
        /// </summary>
        public static void LogWarning(string source, string message)
        {
            LogMessage("WARN", source, message);
        }
        
        /// <summary>
        /// Log an error message.
        /// </summary>
        public static void LogError(string source, string message)
        {
            LogMessage("ERROR", source, message);
            LogToFile(ErrorLogFilePath, FormatMessage("ERROR", source, message));
        }
        
        /// <summary>
        /// Log a debug message.
        /// </summary>
        public static void LogDebug(string source, string message)
        {
            LogMessage("DEBUG", source, message);
        }
        
        /// <summary>
        /// Log ESP-related information.
        /// </summary>
        public static void LogESP(string entity, Vector3 position, float distance, string additionalInfo = "")
        {
            if (!_initialized) Initialize();
            
            string message = $"ESP: {entity} at {position:F1} ({distance:F1}m) {additionalInfo}";
            LogMessage("ESP", "ESPRenderer", message);
        }
        
        /// <summary>
        /// Log performance metrics.
        /// </summary>
        public static void LogPerformance(string operation, float durationMs, int entityCount = 0)
        {
            if (!_initialized) Initialize();
            
            string message = $"PERF: {operation} took {durationMs:F2}ms";
            if (entityCount > 0) message += $" for {entityCount} entities";
            
            LogToFile(PerformanceLogFilePath, FormatMessage("PERF", "Performance", message));
        }
        
        /// <summary>
        /// Log frame-by-frame information (use sparingly).
        /// </summary>
        public static void LogFrame(string info)
        {
            _frameCount++;
            float currentTime = Time.time;
            
            // Only log every 60 frames to avoid spam
            if (_frameCount % 60 == 0)
            {
                float fps = 60f / (currentTime - _lastLogTime);
                string message = $"Frame {_frameCount}, FPS: {fps:F1}, {info}";
                LogDebug("FrameLogger", message);
                _lastLogTime = currentTime;
            }
        }
        
        /// <summary>
        /// Log camera information.
        /// </summary>
        public static void LogCamera(Camera camera, string context)
        {
            if (camera == null)
            {
                LogWarning("CameraLogger", $"{context}: Camera is null");
                return;
            }
            
            string message = $"{context}: Camera active={camera.isActiveAndEnabled}, " +
                           $"pos={camera.transform.position:F1}, " +
                           $"fov={camera.fieldOfView:F1}";
            LogDebug("CameraLogger", message);
        }
        
        /// <summary>
        /// Log game state information.
        /// </summary>
        public static void LogGameState()
        {
            try
            {
                string message = "Game State: ";
                
                if (GameManager.Instance == null)
                {
                    message += "GameManager is null";
                }
                else
                {
                    message += $"GameManager exists, ";
                    
                    if (GameManager.Instance.gameStateManager == null)
                    {
                        message += "GameStateManager is null";
                    }
                    else
                    {
                        bool gameStarted = GameManager.Instance.gameStateManager.IsGameStarted();
                        message += $"GameStarted={gameStarted}";
                    }
                }
                
                LogInfo("GameState", message);
            }
            catch (Exception ex)
            {
                LogError("GameState", $"Failed to get game state: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Create a diagnostic report.
        /// </summary>
        public static void CreateDiagnosticReport()
        {
            if (!_initialized) Initialize();
            
            try
            {
                string reportPath = Path.Combine(LogDirectory, $"diagnostic_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
                
                using (StreamWriter writer = new StreamWriter(reportPath))
                {
                    writer.WriteLine("=== 7D2D ESP Framework Diagnostic Report ===");
                    writer.WriteLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    writer.WriteLine();
                    
                    // System Info
                    writer.WriteLine("=== System Information ===");
                    writer.WriteLine($"Unity Version: {Application.unityVersion}");
                    writer.WriteLine($"Platform: {Application.platform}");
                    writer.WriteLine($"System Language: {Application.systemLanguage}");
                    writer.WriteLine();
                    
                    // Game State
                    writer.WriteLine("=== Game State ===");
                    writer.WriteLine($"GameManager.Exists: {GameManager.Instance != null}");
                    if (GameManager.Instance != null)
                    {
                        writer.WriteLine($"GameStateManager.Exists: {GameManager.Instance.gameStateManager != null}");
                        if (GameManager.Instance.gameStateManager != null)
                        {
                            writer.WriteLine($"GameStarted: {GameManager.Instance.gameStateManager.IsGameStarted()}");
                        }
                    }
                    writer.WriteLine();
                    
                    // Camera State
                    writer.WriteLine("=== Camera State ===");
                    Camera mainCamera = Camera.main;
                    writer.WriteLine($"Camera.Main.Exists: {mainCamera != null}");
                    if (mainCamera != null)
                    {
                        writer.WriteLine($"Camera.Active: {mainCamera.isActiveAndEnabled}");
                        writer.WriteLine($"Camera.Position: {mainCamera.transform.position}");
                        writer.WriteLine($"Camera.FOV: {mainCamera.fieldOfView}");
                    }
                    writer.WriteLine();
                    
                    // Component State
                    writer.WriteLine("=== Component State ===");
                    var espManager = GameObject.FindObjectOfType<ESPManager>();
                    writer.WriteLine($"ESPManager.Exists: {espManager != null}");
                    if (espManager != null)
                    {
                        writer.WriteLine($"ESPManager.Active: {espManager.enabled}");
                    }
                    writer.WriteLine();
                    
                    // Performance
                    writer.WriteLine("=== Performance ===");
                    writer.WriteLine($"Target Frame Rate: {Application.targetFrameRate}");
                    writer.WriteLine($"Current Frame: {Time.frameCount}");
                    writer.WriteLine($"Time: {Time.time:F2}s");
                    if (_performanceStopwatch != null)
                    {
                        writer.WriteLine($"Uptime: {_performanceStopwatch.ElapsedMilliseconds}ms");
                    }
                    writer.WriteLine();
                    
                    // Log Files
                    writer.WriteLine("=== Log Files ===");
                    if (File.Exists(LogFilePath))
                    {
                        var logInfo = new FileInfo(LogFilePath);
                        writer.WriteLine($"Main Log: {logInfo.Length} bytes, {logInfo.LastWriteTime}");
                    }
                    if (File.Exists(ErrorLogFilePath))
                    {
                        var errorInfo = new FileInfo(ErrorLogFilePath);
                        writer.WriteLine($"Error Log: {errorInfo.Length} bytes, {errorInfo.LastWriteTime}");
                    }
                }
                
                LogInfo("Diagnostics", $"Diagnostic report created: {reportPath}");
            }
            catch (Exception ex)
            {
                LogError("Diagnostics", $"Failed to create diagnostic report: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Core logging method.
        /// </summary>
        private static void LogMessage(string level, string source, string message)
        {
            if (!_initialized) Initialize();
            
            try
            {
                string formattedMessage = FormatMessage(level, source, message);
                LogToFile(LogFilePath, formattedMessage);
                
                // Also output to Unity console for debugging
                switch (level)
                {
                    case "ERROR":
                        Debug.LogError($"[{level}] {source}: {message}");
                        break;
                    case "WARN":
                        Debug.LogWarning($"[{level}] {source}: {message}");
                        break;
                    default:
                        Debug.Log($"[{level}] {source}: {message}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RobustDebugger] Failed to log message: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Format a log message with timestamp.
        /// </summary>
        private static string FormatMessage(string level, string source, string message)
        {
            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{level}] [{source}] {message}";
        }
        
        /// <summary>
        /// Write message to log file with rotation.
        /// </summary>
        private static void LogToFile(string filePath, string message)
        {
            try
            {
                lock (_lockObject)
                {
                    RotateLogIfNeeded(filePath);
                    
                    using (StreamWriter writer = File.AppendText(filePath))
                    {
                        writer.WriteLine(message);
                        writer.Flush(); // Ensure immediate write
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RobustDebugger] Failed to write to {filePath}: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Rotate log files if they exceed size limit.
        /// </summary>
        private static void RotateLogIfNeeded(string filePath)
        {
            try
            {
                if (!File.Exists(filePath)) return;
                
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > MaxLogFileSize)
                {
                    string directory = Path.GetDirectoryName(filePath);
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    string extension = Path.GetExtension(filePath);
                    
                    // Move existing files
                    for (int i = MaxLogFiles - 1; i >= 1; i--)
                    {
                        string oldFile = Path.Combine(directory, $"{fileName}.{i}{extension}");
                        string newFile = Path.Combine(directory, $"{fileName}.{i + 1}{extension}");
                        
                        if (File.Exists(oldFile))
                        {
                            if (File.Exists(newFile)) File.Delete(newFile);
                            File.Move(oldFile, newFile);
                        }
                    }
                    
                    // Move current file to .1
                    string backupFile = Path.Combine(directory, $"{fileName}.1{extension}");
                    if (File.Exists(backupFile)) File.Delete(backupFile);
                    File.Move(filePath, backupFile);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[RobustDebugger] Failed to rotate log {filePath}: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Clean up old log files.
        /// </summary>
        public static void CleanupOldLogs()
        {
            try
            {
                if (!Directory.Exists(LogDirectory)) return;
                
                string[] logFiles = Directory.GetFiles(LogDirectory, "*.log*");
                DateTime cutoff = DateTime.Now.AddDays(-7); // Keep logs for 7 days
                
                foreach (string file in logFiles)
                {
                    FileInfo fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < cutoff)
                    {
                        File.Delete(file);
                        LogInfo("Cleanup", $"Deleted old log: {file}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("Cleanup", $"Failed to cleanup old logs: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Get log statistics.
        /// </summary>
        public static string GetLogStats()
        {
            if (!_initialized) Initialize();
            
            try
            {
                string stats = "Log Statistics:\n";
                
                if (File.Exists(LogFilePath))
                {
                    var info = new FileInfo(LogFilePath);
                    stats += $"Main Log: {info.Length:N0} bytes, Modified: {info.LastWriteTime:yyyy-MM-dd HH:mm:ss}\n";
                }
                
                if (File.Exists(ErrorLogFilePath))
                {
                    var info = new FileInfo(ErrorLogFilePath);
                    stats += $"Error Log: {info.Length:N0} bytes, Modified: {info.LastWriteTime:yyyy-MM-dd HH:mm:ss}\n";
                }
                
                if (File.Exists(PerformanceLogFilePath))
                {
                    var info = new FileInfo(PerformanceLogFilePath);
                    stats += $"Performance Log: {info.Length:N0} bytes, Modified: {info.LastWriteTime:yyyy-MM-dd HH:mm:ss}\n";
                }
                
                return stats;
            }
            catch (Exception ex)
            {
                return $"Failed to get log stats: {ex.Message}";
            }
        }
    }
}
