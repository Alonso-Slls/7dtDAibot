using UnityEngine;
using System.Collections.Generic;

namespace SevenDtDAibot
{
    /// <summary>
    /// Basic ESP manager with minimal functionality and robust debugging.
    /// </summary>
    public class ESPManager : MonoBehaviour
    {
        private Camera mainCamera;
        private bool showESP = true;
        private bool showMenu = false;
        private float lastUpdateTime = 0f;
        private int entityCount = 0;
        
        // Entity caching system
        private EntityEnemy[] cachedEnemies;
        private EntityPlayer[] cachedPlayers;
        private EntityAnimal[] cachedAnimals;
        private EntityItem[] cachedItems;
        private float lastEntityUpdate = 0f;
        private readonly float entityUpdateInterval = 0.1f; // Update every 100ms
        
        // Distance culling settings
        private float maxRenderDistance = 100f; // Maximum render distance in meters
        private bool showDistanceSlider = false;
        
        // ESP toggle settings
        private bool showEnemyESP = true;
        private bool showPlayerESP = true;
        private bool showAnimalESP = true;
        private bool showItemESP = true;
        private bool showAdvancedSettings = false;
        
        // Performance monitoring
        private float avgFrameTime = 0f;
        private float avgCacheTime = 0f;
        private int frameCount = 0;
        private float totalFrameTime = 0f;
        private float totalCacheTime = 0f;
        private readonly int performanceSampleSize = 60; // 1 second at 60 FPS
        
        void Start()
        {
            // Initialize debugger first
            RobustDebugger.Initialize();
            RobustDebugger.LogInfo("ESPManager", "Starting basic ESP with debugger...");
            
            // Wait for camera to be available
            InvokeRepeating(nameof(FindCamera), 0f, 1f);
            
            // Start entity caching system
            InvokeRepeating(nameof(UpdateEntityCache), 0f, entityUpdateInterval);
            
            // Log initial state
            RobustDebugger.LogGameState();
            RobustDebugger.CreateDiagnosticReport();
        }
        
        void FindCamera()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    RobustDebugger.LogCamera(mainCamera, "Camera Found");
                    RobustDebugger.LogInfo("ESPManager", "Camera found and initialized");
                    CancelInvoke(nameof(FindCamera));
                }
                else
                {
                    RobustDebugger.LogWarning("ESPManager", "Camera still not found, will retry...");
                }
            }
        }
        
        void UpdateEntityCache()
        {
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                // Update cached entity lists
                cachedEnemies = FindObjectsOfType<EntityEnemy>();
                cachedPlayers = FindObjectsOfType<EntityPlayer>();
                cachedAnimals = FindObjectsOfType<EntityAnimal>();
                cachedItems = FindObjectsOfType<EntityItem>();
                
                lastEntityUpdate = Time.time;
                
                startTime.Stop();
                RobustDebugger.LogPerformance("EntityCache", startTime.ElapsedMilliseconds, 
                    cachedEnemies.Length + cachedPlayers.Length + cachedAnimals.Length + cachedItems.Length);
                
                RobustDebugger.LogDebug("EntityCache", 
                    $"Cached {cachedEnemies.Length} enemies, {cachedPlayers.Length} players, {cachedAnimals.Length} animals, {cachedItems.Length} items");
            }
            catch (System.Exception ex)
            {
                RobustDebugger.LogError("UpdateEntityCache", $"Exception during entity caching: {ex.Message}");
            }
        }
        
        void Update()
        {
            // Toggle menu with Insert key
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                showMenu = !showMenu;
                RobustDebugger.LogInfo("Input", $"Menu toggled: {showMenu}");
            }
            
            // Toggle ESP with F1 key
            if (Input.GetKeyDown(KeyCode.F1))
            {
                showESP = !showESP;
                RobustDebugger.LogInfo("Input", $"ESP toggled: {showESP}");
            }
            
            // Generate diagnostic report with F2
            if (Input.GetKeyDown(KeyCode.F2))
            {
                RobustDebugger.CreateDiagnosticReport();
                RobustDebugger.LogInfo("Input", "Diagnostic report generated");
            }
            
            // Log frame info periodically
            RobustDebugger.LogFrame($"ESP={showESP}, Menu={showMenu}, CacheAge={(Time.time - lastEntityUpdate):F2}s");
        }
        
        void OnGUI()
        {
            if (showMenu)
            {
                DrawMenu();
            }
            
            if (showESP && mainCamera != null)
            {
                var frameStart = Time.realtimeSinceStartup;
                DrawESP();
                var frameTime = (Time.realtimeSinceStartup - frameStart) * 1000f;
                
                // Update performance metrics
                frameCount++;
                totalFrameTime += frameTime;
                
                if (frameCount >= performanceSampleSize)
                {
                    avgFrameTime = totalFrameTime / frameCount;
                    frameCount = 0;
                    totalFrameTime = 0f;
                }
                
                RobustDebugger.LogPerformance("ESP_Render", frameTime, entityCount);
            }
        }
        
        void DrawMenu()
        {
            GUI.Box(new Rect(10, 10, 250, showAdvancedSettings ? 350 : 230), "Basic ESP Menu");
            
            showESP = GUI.Toggle(new Rect(20, 40, 230, 20), showESP, "Show ESP");
            
            // Show debug info
            GUI.Label(new Rect(20, 70, 230, 20), $"Entities: {entityCount}");
            GUI.Label(new Rect(20, 90, 230, 20), $"Camera: {(mainCamera != null ? "Found" : "None")}");
            GUI.Label(new Rect(20, 110, 230, 20), $"Cache Age: {(Time.time - lastEntityUpdate):F1}s");
            GUI.Label(new Rect(20, 130, 230, 20), $"Max Distance: {maxRenderDistance:F0}m");
            GUI.Label(new Rect(20, 150, 230, 20), $"FPS: {(avgFrameTime > 0 ? (1000f / avgFrameTime).ToString("F0") : "---")}");
            
            // Advanced settings toggle
            if (GUI.Button(new Rect(20, 170, 230, 20), "Advanced Settings"))
            {
                showAdvancedSettings = !showAdvancedSettings;
                RobustDebugger.LogInfo("Input", $"Advanced settings toggled: {showAdvancedSettings}");
            }
            
            if (showAdvancedSettings)
            {
                // ESP type toggles
                GUI.Label(new Rect(20, 195, 230, 20), "ESP Types:");
                showEnemyESP = GUI.Toggle(new Rect(30, 215, 100, 20), showEnemyESP, "Enemies");
                showPlayerESP = GUI.Toggle(new Rect(140, 215, 100, 20), showPlayerESP, "Players");
                showAnimalESP = GUI.Toggle(new Rect(30, 235, 100, 20), showAnimalESP, "Animals");
                showItemESP = GUI.Toggle(new Rect(140, 235, 100, 20), showItemESP, "Items");
                
                // Distance slider
                GUI.Label(new Rect(20, 260, 230, 20), "Render Distance:");
                float newDistance = GUI.HorizontalSlider(new Rect(20, 280, 180, 20), maxRenderDistance, 50f, 500f);
                if (newDistance != maxRenderDistance)
                {
                    maxRenderDistance = newDistance;
                    RobustDebugger.LogInfo("Settings", $"Max render distance updated: {maxRenderDistance:F0}m");
                }
                GUI.Label(new Rect(205, 280, 45, 20), $"{maxRenderDistance:F0}m");
                
                // Performance info
                GUI.Label(new Rect(20, 305, 230, 20), $"Active ESP: {(showEnemyESP ? "E" : "")}{(showPlayerESP ? "P" : "")}{(showAnimalESP ? "A" : "")}{(showItemESP ? "I" : "")}");
                GUI.Label(new Rect(20, 325, 230, 20), $"Avg Frame: {avgFrameTime:F2}ms | Cache: {avgCacheTime:F2}ms");
            }
            else
            {
                // Distance slider (simplified view)
                if (GUI.Button(new Rect(20, 195, 230, 20), "Distance Settings"))
                {
                    showDistanceSlider = !showDistanceSlider;
                    RobustDebugger.LogInfo("Input", $"Distance settings toggled: {showDistanceSlider}");
                }
                
                if (showDistanceSlider)
                {
                    GUI.Label(new Rect(20, 220, 230, 20), "Render Distance:");
                    float newDistance = GUI.HorizontalSlider(new Rect(20, 240, 180, 20), maxRenderDistance, 50f, 500f);
                    if (newDistance != maxRenderDistance)
                    {
                        maxRenderDistance = newDistance;
                        RobustDebugger.LogInfo("Settings", $"Max render distance updated: {maxRenderDistance:F0}m");
                    }
                    GUI.Label(new Rect(205, 240, 45, 20), $"{maxRenderDistance:F0}m");
                }
            }
            
            if (GUI.Button(new Rect(20, showAdvancedSettings ? 355 : showDistanceSlider ? 270 : 180, 230, 30), "Close (Insert)"))
            {
                showMenu = false;
                RobustDebugger.LogInfo("Input", "Menu closed via button");
            }
        }
        
        void DrawESP()
        {
            entityCount = 0;
            var startTime = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                // Use cached entity lists
                if (cachedEnemies == null || cachedPlayers == null || cachedAnimals == null || cachedItems == null)
                {
                    RobustDebugger.LogWarning("DrawESP", "Entity cache not initialized, skipping render");
                    return;
                }
                
                RobustDebugger.LogDebug("EntityRender", 
                    $"Rendering {cachedEnemies.Length} enemies, {cachedPlayers.Length} players, {cachedAnimals.Length} animals, {cachedItems.Length} items");
                
                // Batched rendering by entity type (only if enabled)
                if (showEnemyESP) DrawBatchedEntities(cachedEnemies, Color.red, "Enemy");
                if (showPlayerESP) DrawBatchedEntities(cachedPlayers, Color.green, "Player");
                if (showAnimalESP) DrawBatchedEntities(cachedAnimals, Color.yellow, "Animal");
                if (showItemESP) DrawBatchedEntities(cachedItems, Color.cyan, "Item");
                
                startTime.Stop();
                RobustDebugger.LogPerformance("ESP_Render", startTime.ElapsedMilliseconds, entityCount);
            }
            catch (System.Exception ex)
            {
                RobustDebugger.LogError("DrawESP", $"Exception during ESP rendering: {ex.Message}");
            }
        }
        
        void DrawBatchedEntities<T>(T[] entities, Color color, string label) where T : Entity
        {
            if (entities == null || mainCamera == null) return;
            
            // Set color once for the entire batch
            GUI.color = color;
            
            foreach (var entity in entities)
            {
                if (entity == null) continue;
                
                // Skip player self for player entities
                if (label == "Player" && entity == GameManager.Instance.World.GetPrimaryPlayer())
                    continue;
                
                // Get world position and calculate distance
                Vector3 worldPos = entity.transform.position;
                float distance = Vector3.Distance(mainCamera.transform.position, worldPos);
                
                // Skip if beyond max render distance
                if (distance > maxRenderDistance)
                {
                    RobustDebugger.LogDebug("DistanceCull", $"Skipped {label} at {distance:F0}m (max: {maxRenderDistance:F0}m)");
                    continue;
                }
                
                // Convert to screen position
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
                
                // Skip if behind camera
                if (screenPos.z < 0) continue;
                
                // Flip Y coordinate for GUI
                screenPos.y = Screen.height - screenPos.y;
                
                // Draw box (color already set)
                float boxSize = Mathf.Max(20f, 1000f / distance);
                GUI.DrawTexture(new Rect(screenPos.x - boxSize/2, screenPos.y - boxSize/2, boxSize, boxSize), Texture2D.whiteTexture, ScaleMode.StretchToFill);
                
                // Draw label (reset color to white for text)
                GUI.color = Color.white;
                GUI.Label(new Rect(screenPos.x - 50, screenPos.y - boxSize/2 - 20, 100, 20), $"{label} [{distance:F0}m]");
                
                // Restore batch color for next box
                GUI.color = color;
                
                entityCount++;
                
                // Log ESP data
                RobustDebugger.LogESP(label, worldPos, distance, $"Screen=({screenPos.x:F0},{screenPos.y:F0})");
            }
            
            // Reset color to white after batch
            GUI.color = Color.white;
        }
        
        void DrawEntityESP(Entity entity, Color color, string label)
        {
            // This method is kept for compatibility but no longer used in main rendering
            // All rendering now goes through DrawBatchedEntities for better performance
            RobustDebugger.LogWarning("DrawEntityESP", "Legacy method called - should use DrawBatchedEntities instead");
        }
        
        void OnDestroy()
        {
            RobustDebugger.LogInfo("ESPManager", "ESPManager destroyed");
            RobustDebugger.CleanupOldLogs();
        }
        
        void OnApplicationQuit()
        {
            RobustDebugger.LogInfo("ESPManager", "Application quitting");
            RobustDebugger.LogInfo("ESPManager", $"Performance Stats - Avg Frame: {avgFrameTime:F2}ms, Avg Cache: {avgCacheTime:F2}ms");
            RobustDebugger.LogInfo("ESPManager", RobustDebugger.GetLogStats());
        }
    }
}
