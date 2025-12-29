using System;
using System.Collections.Generic;
using UnityEngine;

namespace SevenDtDAibot
{
    public class PerformanceMonitor
    {
        private float deltaTime = 0.0f;
        private float fps = 0.0f;
        private float frameTime = 0.0f;
        private int frameCount = 0;
        private float fpsUpdateTime = 0.0f;
        
        // Performance history for averaging
        private Queue<float> fpsHistory = new Queue<float>();
        private Queue<float> frameTimeHistory = new Queue<float>();
        private int maxHistorySize = 60; // Keep last 60 frames for averaging
        
        // Performance metrics
        private float averageFPS = 0.0f;
        private float averageFrameTime = 0.0f;
        private float minFPS = float.MaxValue;
        private float maxFPS = 0.0f;
        private float minFrameTime = float.MaxValue;
        private float maxFrameTime = 0.0f;
        
        // Memory usage tracking
        private long lastMemoryCheck = 0;
        private float memoryUsageMB = 0.0f;
        private float memoryUpdateInterval = 1.0f; // Update memory usage every second
        
        public float CurrentFPS => fps;
        public float FrameTime => frameTime;
        public float AverageFPS => averageFPS;
        public float AverageFrameTime => averageFrameTime;
        public float MinFPS => minFPS;
        public float MaxFPS => maxFPS;
        public float MinFrameTime => minFrameTime;
        public float MaxFrameTime => maxFrameTime;
        public float MemoryUsageMB => memoryUsageMB;
        
        public void Update()
        {
            try
            {
                // Calculate delta time
                deltaTime += Time.unscaledDeltaTime;
                frameCount++;
                
                // Update FPS every 0.5 seconds
                if (Time.realtimeSinceStartup - fpsUpdateTime >= 0.5f)
                {
                    fps = frameCount / deltaTime;
                    frameTime = (deltaTime / frameCount) * 1000f; // Convert to milliseconds
                    
                    // Update history
                    UpdateHistory(fps, frameTime);
                    
                    // Update min/max values
                    UpdateMinMaxValues(fps, frameTime);
                    
                    // Reset counters
                    deltaTime = 0.0f;
                    frameCount = 0;
                    fpsUpdateTime = Time.realtimeSinceStartup;
                }
                
                // Update memory usage periodically
                if (Time.realtimeSinceStartup - (lastMemoryCheck / 1000f) >= memoryUpdateInterval)
                {
                    UpdateMemoryUsage();
                    lastMemoryCheck = (long)(Time.realtimeSinceStartup * 1000f);
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[PerformanceMonitor] Update error: {ex.Message}");
            }
        }
        
        private void UpdateHistory(float currentFPS, float currentFrameTime)
        {
            try
            {
                // Add to history
                fpsHistory.Enqueue(currentFPS);
                frameTimeHistory.Enqueue(currentFrameTime);
                
                // Remove old entries if history is too large
                while (fpsHistory.Count > maxHistorySize)
                {
                    fpsHistory.Dequeue();
                }
                
                while (frameTimeHistory.Count > maxHistorySize)
                {
                    frameTimeHistory.Dequeue();
                }
                
                // Calculate averages
                CalculateAverages();
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[PerformanceMonitor] History update error: {ex.Message}");
            }
        }
        
        private void CalculateAverages()
        {
            try
            {
                if (fpsHistory.Count > 0)
                {
                    float totalFPS = 0f;
                    foreach (float fpsValue in fpsHistory)
                    {
                        totalFPS += fpsValue;
                    }
                    averageFPS = totalFPS / fpsHistory.Count;
                }
                
                if (frameTimeHistory.Count > 0)
                {
                    float totalFrameTime = 0f;
                    foreach (float frameTimeValue in frameTimeHistory)
                    {
                        totalFrameTime += frameTimeValue;
                    }
                    averageFrameTime = totalFrameTime / frameTimeHistory.Count;
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[PerformanceMonitor] Average calculation error: {ex.Message}");
            }
        }
        
        private void UpdateMinMaxValues(float currentFPS, float currentFrameTime)
        {
            try
            {
                // Update FPS min/max
                if (currentFPS < minFPS)
                    minFPS = currentFPS;
                if (currentFPS > maxFPS)
                    maxFPS = currentFPS;
                
                // Update frame time min/max
                if (currentFrameTime < minFrameTime)
                    minFrameTime = currentFrameTime;
                if (currentFrameTime > maxFrameTime)
                    maxFrameTime = currentFrameTime;
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[PerformanceMonitor] Min/Max update error: {ex.Message}");
            }
        }
        
        private void UpdateMemoryUsage()
        {
            try
            {
                // Get current memory usage in bytes
                long currentMemory = GC.GetTotalMemory(false);
                memoryUsageMB = (float)currentMemory / (1024f * 1024f); // Convert to MB
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[PerformanceMonitor] Memory usage update error: {ex.Message}");
            }
        }
        
        public void Reset()
        {
            try
            {
                // Reset all metrics
                deltaTime = 0.0f;
                fps = 0.0f;
                frameTime = 0.0f;
                frameCount = 0;
                fpsUpdateTime = 0.0f;
                
                // Clear history
                fpsHistory.Clear();
                frameTimeHistory.Clear();
                
                // Reset min/max values
                minFPS = float.MaxValue;
                maxFPS = 0.0f;
                minFrameTime = float.MaxValue;
                maxFrameTime = 0.0f;
                
                // Reset memory tracking
                lastMemoryCheck = 0;
                memoryUsageMB = 0.0f;
                
                RobustDebugger.Log("[PerformanceMonitor] Performance metrics reset");
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[PerformanceMonitor] Reset error: {ex.Message}");
            }
        }
        
        public string GetPerformanceReport()
        {
            try
            {
                return $"Performance Report:\n" +
                       $"Current FPS: {fps:F1}\n" +
                       $"Average FPS: {averageFPS:F1}\n" +
                       $"Min/Max FPS: {minFPS:F1}/{maxFPS:F1}\n" +
                       $"Current Frame Time: {frameTime:F2}ms\n" +
                       $"Average Frame Time: {averageFrameTime:F2}ms\n" +
                       $"Min/Max Frame Time: {minFrameTime:F2}ms/{maxFrameTime:F2}ms\n" +
                       $"Memory Usage: {memoryUsageMB:F1}MB\n" +
                       $"History Samples: {fpsHistory.Count}";
            }
            catch (Exception ex)
            {
                return $"Error generating performance report: {ex.Message}";
            }
        }
        
        public bool IsPerformanceGood()
        {
            // Consider performance good if FPS is above 30 and frame time is below 33ms
            return fps >= 30f && frameTime <= 33f;
        }
        
        public bool IsPerformanceExcellent()
        {
            // Consider performance excellent if FPS is above 60 and frame time is below 16ms
            return fps >= 60f && frameTime <= 16f;
        }
        
        public string GetPerformanceStatus()
        {
            if (IsPerformanceExcellent())
                return "Excellent";
            else if (IsPerformanceGood())
                return "Good";
            else if (fps >= 20f)
                return "Fair";
            else
                return "Poor";
        }
        
        public void LogPerformanceMetrics()
        {
            try
            {
                RobustDebugger.Log($"[PerformanceMonitor] {GetPerformanceReport()}");
                RobustDebugger.Log($"[PerformanceMonitor] Status: {GetPerformanceStatus()}");
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[PerformanceMonitor] Logging error: {ex.Message}");
            }
        }
    }
}
