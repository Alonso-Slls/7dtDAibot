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
        
        // ESP toggle settings
        private bool showEnemyESP = true;
        private bool showPlayerESP = true;
        private bool showAnimalESP = true;
        private bool showItemESP = true;
        private bool showPerformancePanel = true;
        private bool showRenderPanel = true;
        private bool showDiagnosticsPanel = true;
        private Rect menuRect = new Rect(15, 15, 350, 430);
        private Vector2 menuScrollPosition = Vector2.zero;
        private GUIStyle headerLabelStyle;
        private GUIStyle infoLabelStyle;
        
        // Performance monitoring
        private float avgFrameTime = 0f;
        private float avgCacheTime = 0f;
        private int frameCount = 0;
        private float totalFrameTime = 0f;
        private float totalCacheTime = 0f;
        private readonly int performanceSampleSize = 60; // 1 second at 60 FPS
        private int cacheSampleCount = 0;
        
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

            InitializeStyles();
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
        
        void InitializeStyles()
        {
            headerLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            
            infoLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };
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
            if (headerLabelStyle == null || infoLabelStyle == null)
            {
                InitializeStyles();
            }
            
            menuRect = GUI.Window(0, menuRect, DrawMenuWindow, "7D2D ESP Dashboard");
        }
        
        void DrawMenuWindow(int windowId)
        {
            GUILayout.BeginVertical(GUILayout.Width(330));
            menuScrollPosition = GUILayout.BeginScrollView(menuScrollPosition, false, true, GUILayout.Height(370));
            
            DrawStatusSection();
            GUILayout.Space(8);
            DrawRenderSection();
            GUILayout.Space(8);
            DrawPerformanceSection();
            GUILayout.Space(8);
            DrawDiagnosticsSection();
            
            GUILayout.EndScrollView();
            
            if (GUILayout.Button("Close Menu (Insert)", GUILayout.Height(28)))
            {
                showMenu = false;
                RobustDebugger.LogInfo("Input", "Menu closed via button");
            }
            
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
        
        void DrawStatusSection()
        {
            GUILayout.Label("Runtime Status", headerLabelStyle);
            showESP = GUILayout.Toggle(showESP, "Master ESP Toggle");
            
            GUILayout.Label($"Entities Tracked: {entityCount}", infoLabelStyle);
            GUILayout.Label($"Camera State: {(mainCamera != null ? "Active" : "Missing")}", infoLabelStyle);
            GUILayout.Label($"Cache Age: {(Time.time - lastEntityUpdate):F1}s", infoLabelStyle);
        }
        
        void DrawRenderSection()
        {
            showRenderPanel = GUILayout.Toggle(showRenderPanel, "Rendering & Filters", GUI.skin.button);
            if (!showRenderPanel) return;
            
            GUILayout.BeginVertical("box");
            GUILayout.Label("Entity Types", headerLabelStyle);
            GUILayout.BeginHorizontal();
            showEnemyESP = GUILayout.Toggle(showEnemyESP, "Enemies");
            showPlayerESP = GUILayout.Toggle(showPlayerESP, "Players");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            showAnimalESP = GUILayout.Toggle(showAnimalESP, "Animals");
            showItemESP = GUILayout.Toggle(showItemESP, "Items");
            GUILayout.EndHorizontal();
            
            GUILayout.Space(4);
            GUILayout.Label($"Render Distance: {maxRenderDistance:F0}m");
            float newDistance = GUILayout.HorizontalSlider(maxRenderDistance, 50f, 500f);
            if (!Mathf.Approximately(newDistance, maxRenderDistance))
            {
                maxRenderDistance = newDistance;
                RobustDebugger.LogInfo("Settings", $"Max render distance updated: {maxRenderDistance:F0}m");
            }
            
            if (GUILayout.Button("Force Cache Refresh"))
            {
                UpdateEntityCache();
                RobustDebugger.LogInfo("Settings", "Entity cache forced via GUI");
            }
            
            GUILayout.EndVertical();
        }
        
        void DrawPerformanceSection()
        {
            showPerformancePanel = GUILayout.Toggle(showPerformancePanel, "Performance", GUI.skin.button);
            if (!showPerformancePanel) return;
            
            GUILayout.BeginVertical("box");
            string fpsLabel = avgFrameTime > 0 ? (1000f / avgFrameTime).ToString("F0") : "---";
            GUILayout.Label($"Average Frame Time: {avgFrameTime:F2}ms (~{fpsLabel} FPS)", infoLabelStyle);
            GUILayout.Label($"Average Cache Time: {avgCacheTime:F2}ms", infoLabelStyle);
            GUILayout.Label($"Samples: Frame={frameCount}, Cache={cacheSampleCount}", infoLabelStyle);
            GUILayout.EndVertical();
        }
        
        void DrawDiagnosticsSection()
        {
            showDiagnosticsPanel = GUILayout.Toggle(showDiagnosticsPanel, "Diagnostics", GUI.skin.button);
            if (!showDiagnosticsPanel) return;
            
            GUILayout.BeginVertical("box");
            GUILayout.Label("Generate reports and inspect logs.", infoLabelStyle);
            
            if (GUILayout.Button("Generate Diagnostic Report (.txt)"))
            {
                RobustDebugger.CreateDiagnosticReport();
                RobustDebugger.LogInfo("Diagnostics", "Report requested from GUI");
            }
            
            if (GUILayout.Button("Log Current Stats"))
            {
                RobustDebugger.LogInfo("Diagnostics", RobustDebugger.GetLogStats());
            }
            
            if (GUILayout.Button("Toggle Menu Visibility"))
            {
                showMenu = false;
            }
            
            GUILayout.EndVertical();
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
