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
        
        void Start()
        {
            // Initialize debugger first
            RobustDebugger.Initialize();
            RobustDebugger.LogInfo("ESPManager", "Starting basic ESP with debugger...");
            
            // Wait for camera to be available
            InvokeRepeating(nameof(FindCamera), 0f, 1f);
            
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
            RobustDebugger.LogFrame($"ESP={showESP}, Menu={showMenu}");
        }
        
        void OnGUI()
        {
            if (showMenu)
            {
                DrawMenu();
            }
            
            if (showESP && mainCamera != null)
            {
                var startTime = Time.realtimeSinceStartup;
                DrawESP();
                var duration = (Time.realtimeSinceStartup - startTime) * 1000f;
                RobustDebugger.LogPerformance("ESP_Render", duration, entityCount);
            }
        }
        
        void DrawMenu()
        {
            GUI.Box(new Rect(10, 10, 250, 150), "Basic ESP Menu");
            
            showESP = GUI.Toggle(new Rect(20, 40, 230, 20), showESP, "Show ESP");
            
            // Show debug info
            GUI.Label(new Rect(20, 70, 230, 20), $"Entities: {entityCount}");
            GUI.Label(new Rect(20, 90, 230, 20), $"Camera: {(mainCamera != null ? "Found" : "None")}");
            
            if (GUI.Button(new Rect(20, 120, 230, 30), "Close (Insert)"))
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
                // Find all entities
                var enemies = FindObjectsOfType<EntityEnemy>();
                var players = FindObjectsOfType<EntityPlayer>();
                var animals = FindObjectsOfType<EntityAnimal>();
                var items = FindObjectsOfType<EntityItem>();
                
                RobustDebugger.LogDebug("EntityScan", $"Found {enemies.Length} enemies, {players.Length} players, {animals.Length} animals, {items.Length} items");
                
                // Draw ESP for each type
                foreach (var enemy in enemies)
                {
                    DrawEntityESP(enemy, Color.red, "Enemy");
                    entityCount++;
                }
                
                foreach (var player in players)
                {
                    if (player != GameManager.Instance.World.GetPrimaryPlayer())
                    {
                        DrawEntityESP(player, Color.green, "Player");
                        entityCount++;
                    }
                }
                
                foreach (var animal in animals)
                {
                    DrawEntityESP(animal, Color.yellow, "Animal");
                    entityCount++;
                }
                
                foreach (var item in items)
                {
                    DrawEntityESP(item, Color.cyan, "Item");
                    entityCount++;
                }
                
                startTime.Stop();
                RobustDebugger.LogPerformance("EntityProcessing", startTime.ElapsedMilliseconds, entityCount);
            }
            catch (System.Exception ex)
            {
                RobustDebugger.LogError("DrawESP", $"Exception during ESP rendering: {ex.Message}");
            }
        }
        
        void DrawEntityESP(Entity entity, Color color, string label)
        {
            if (entity == null || mainCamera == null) 
            {
                RobustDebugger.LogWarning("DrawEntityESP", $"Null reference: entity={entity == null}, camera={mainCamera == null}");
                return;
            }
            
            try
            {
                // Get world position
                Vector3 worldPos = entity.transform.position;
                
                // Convert to screen position
                Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
                
                // Skip if behind camera
                if (screenPos.z < 0) return;
                
                // Flip Y coordinate for GUI
                screenPos.y = Screen.height - screenPos.y;
                
                // Calculate distance
                float distance = Vector3.Distance(mainCamera.transform.position, worldPos);
                
                // Log ESP data
                RobustDebugger.LogESP(label, worldPos, distance, $"Screen=({screenPos.x:F0},{screenPos.y:F0})");
                
                // Draw box around entity
                float boxSize = Mathf.Max(20f, 1000f / distance); // Dynamic box size
                GUI.color = color;
                GUI.DrawTexture(new Rect(screenPos.x - boxSize/2, screenPos.y - boxSize/2, boxSize, boxSize), Texture2D.whiteTexture, ScaleMode.StretchToFill);
                
                // Draw label
                GUI.color = Color.white;
                GUI.Label(new Rect(screenPos.x - 50, screenPos.y - boxSize/2 - 20, 100, 20), $"{label} [{distance:F0}m]");
                
                GUI.color = Color.white;
            }
            catch (System.Exception ex)
            {
                RobustDebugger.LogError("DrawEntityESP", $"Failed to draw ESP for {label}: {ex.Message}");
            }
        }
        
        void OnDestroy()
        {
            RobustDebugger.LogInfo("ESPManager", "ESPManager destroyed");
            RobustDebugger.CleanupOldLogs();
        }
        
        void OnApplicationQuit()
        {
            RobustDebugger.LogInfo("ESPManager", "Application quitting");
            RobustDebugger.LogInfo("ESPManager", RobustDebugger.GetLogStats());
        }
    }
}
