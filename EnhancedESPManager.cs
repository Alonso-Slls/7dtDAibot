using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Modules;

namespace SevenDtDAibot
{
    public class EnhancedESPManager : MonoBehaviour
    {
        private ESPConfiguration config;
        private PerformanceMonitor performanceMonitor;
        private List<CachedEntity> cachedEntities = new List<CachedEntity>();
        private Camera mainCamera;
        private bool showMenu = false;
        private bool espEnabled = true;
        private float lastEntityScan = 0f;
        private Rect menuRect = new Rect(10, 10, 250, 300);
        private Vector2 scrollPosition = Vector2.zero;
        
        // Performance optimization settings
        private const int MAX_ENTITIES = 50; // Limit cached entities
        private const float SCAN_INTERVAL = 0.2f; // Scan every 200ms
        private const float CACHE_DURATION = 1.0f; // Cache entities for 1 second
        private float lastCacheUpdate = 0f;
        
        // Reusable objects to reduce GC pressure
        private Rect tempRect = new Rect();
        private GUIContent tempContent = new GUIContent();
        private Vector3[] screenPoints = new Vector3[8]; // Reusable array for box calculations

        void Awake()
        {
            try
            {
                config = new ESPConfiguration();
                config.LoadConfiguration();
                
                performanceMonitor = new PerformanceMonitor();
                
                RobustDebugger.Log("[EnhancedESP] Enhanced ESP Manager initialized");
                RobustDebugger.Log($"[EnhancedESP] Configuration loaded. Max distance: {config.maxESPDistance}m");
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Failed to initialize: " + ex.Message);
            }
        }

        void Update()
        {
            try
            {
                performanceMonitor.Update();
                
                // Handle hotkeys
                if (Input.GetKeyDown((KeyCode)config.toggleMenuKey))
                {
                    showMenu = !showMenu;
                    RobustDebugger.Log($"[EnhancedESP] Menu {(showMenu ? "shown" : "hidden")}");
                }
                
                if (Input.GetKeyDown((KeyCode)config.toggleESPKey))
                {
                    espEnabled = !espEnabled;
                    RobustDebugger.Log($"[EnhancedESP] ESP {(espEnabled ? "enabled" : "disabled")}");
                }
                
                // Find camera if not found (optimized)
                if (mainCamera == null)
                {
                    mainCamera = Camera.main;
                    if (mainCamera == null)
                    {
                        // Only search for camera if main is null (performance optimization)
                        mainCamera = FindObjectOfType<Camera>();
                    }
                }
                
                // Optimized entity scanning with adaptive interval
                float currentInterval = cachedEntities.Count > 20 ? SCAN_INTERVAL * 2 : SCAN_INTERVAL;
                if (Time.time - lastEntityScan >= currentInterval)
                {
                    ScanAndCacheEntities();
                    lastEntityScan = Time.time;
                }
                
                // Clean cache periodically to prevent memory buildup
                if (Time.time - lastCacheUpdate > CACHE_DURATION)
                {
                    CleanEntityCache();
                    lastCacheUpdate = Time.time;
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Update error: " + ex.Message);
            }
        }

        void OnGUI()
        {
            try
            {
                // Early null checks to prevent exceptions
                if (config == null)
                {
                    RobustDebugger.LogError("[EnhancedESP] Config is null in OnGUI");
                    return;
                }
                
                if (espEnabled && mainCamera != null)
                {
                    RenderAllESP();
                }
                
                if (showMenu && config != null)
                {
                    RenderMenu();
                }
                
                // Performance overlay - additional null check
                if (config.showPerformanceInfo && performanceMonitor != null)
                {
                    RenderPerformanceOverlay();
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] OnGUI error: " + ex.Message);
            }
        }

        private void ScanAndCacheEntities()
        {
            try
            {
                // Clear cache if it's been too long or too many entities
                if (Time.time - lastCacheUpdate > CACHE_DURATION * 2 || cachedEntities.Count > MAX_ENTITIES)
                {
                    cachedEntities.Clear();
                }
                
                if (mainCamera == null) return;
                
                Vector3 cameraPos = mainCamera.transform.position;
                
                // OPTIMIZED: Scan enemies only (no players, animals, or items)
                if (config.showEnemies)
                {
                    ScanEnemiesOptimized(cameraPos);
                }
                
                if (config.enableVerboseLogging && cachedEntities.Count > 0)
                {
                    RobustDebugger.Log($"[EnhancedESP] Scanned {cachedEntities.Count} enemies");
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Entity scanning error: " + ex.Message);
            }
        }

        private void ScanEnemiesOptimized(Vector3 cameraPos)
        {
            try
            {
                // Use FindObjectsOfType for better performance than manual iteration
                EntityEnemy[] allEnemies = FindObjectsOfType<EntityEnemy>();
                
                // Pre-allocate list to avoid resizing
                if (cachedEntities.Capacity < MAX_ENTITIES)
                {
                    cachedEntities.Capacity = MAX_ENTITIES;
                }
                
                int processedCount = 0;
                Color enemyColor = config.GetEnemyColor();
                
                foreach (EntityEnemy enemy in allEnemies)
                {
                    // Skip null or dead enemies early
                    if (enemy == null || !enemy.IsAlive() || enemy.transform == null)
                        continue;
                    
                    // Distance culling before any calculations
                    Vector3 enemyPos = enemy.transform.position;
                    float distance = Vector3.Distance(cameraPos, enemyPos);
                    
                    if (distance > config.maxESPDistance)
                        continue;
                    
                    // Limit entities for performance
                    if (processedCount >= MAX_ENTITIES)
                        break;
                    
                    // Add to cache
                    cachedEntities.Add(new CachedEntity
                    {
                        entity = enemy,
                        position = enemyPos,
                        distance = distance,
                        color = enemyColor,
                        typeName = "Enemy",
                        health = 100,
                        maxHealth = 100,
                        lastUpdate = Time.time
                    });
                    
                    processedCount++;
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Optimized enemy scanning error: " + ex.Message);
            }
        }
        
        private void CleanEntityCache()
        {
            try
            {
                // Remove dead or null entities
                cachedEntities.RemoveAll(entity => 
                    entity.entity == null || 
                    (entity.entity is EntityEnemy enemy && !enemy.IsAlive()) ||
                    Time.time - entity.lastUpdate > CACHE_DURATION * 2);
                
                // Sort by distance for consistent rendering
                cachedEntities.Sort((a, b) => a.distance.CompareTo(b.distance));
                
                // Limit cache size
                if (cachedEntities.Count > MAX_ENTITIES)
                {
                    cachedEntities.RemoveRange(MAX_ENTITIES, cachedEntities.Count - MAX_ENTITIES);
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Cache cleaning error: " + ex.Message);
            }
        }

        private void ScanEntityType<T>(Vector3 cameraPos, Color color, string typeName) where T : Entity
        {
            try
            {
                T[] entities = FindObjectsOfType<T>();
                
                foreach (T entity in entities)
                {
                    if (entity == null) continue;
                    
                    float distance = Vector3.Distance(cameraPos, entity.transform.position);
                    
                    // Distance culling
                    if (distance > config.maxESPDistance) continue;
                    
                    // Skip dead entities
                    if (entity is EntityEnemy enemy && !enemy.IsAlive()) continue;
                    if (entity is EntityPlayer player && !player.IsAlive()) continue;
                    
                    cachedEntities.Add(new CachedEntity
                    {
                        entity = entity,
                        position = entity.transform.position,
                        distance = distance,
                        color = color,
                        typeName = typeName,
                        health = 100, // Default health
                        maxHealth = 100 // Default max health
                    });
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[EnhancedESP] Error scanning {typeName}: " + ex.Message);
            }
        }

        private void RenderAllESP()
        {
            try
            {
                if (cachedEntities.Count == 0) return;
                
                // Pre-calculate common values
                Vector3 cameraPos = mainCamera.transform.position;
                float screenWidth = Screen.width;
                float screenHeight = Screen.height;
                float maxDistance = config.maxESPDistance;
                
                foreach (CachedEntity cached in cachedEntities)
                {
                    if (cached.entity == null) continue;
                    
                    // Quick distance check
                    if (cached.distance > maxDistance) continue;
                    
                    // Convert to screen coordinates
                    Vector3 screenPos = mainCamera.WorldToScreenPoint(cached.position);
                    
                    // Skip if behind camera
                    if (screenPos.z < 0) continue;
                    
                    // Convert to GUI coordinates
                    screenPos.y = screenHeight - screenPos.y;
                    
                    // Calculate opacity based on distance
                    float opacity = Mathf.Lerp(1.0f, 0.3f, cached.distance / maxDistance);
                    Color espColor = new Color(cached.color.r, cached.color.g, cached.color.b, cached.color.a * opacity);
                    
                    // OPTIMIZED: Only draw essential ESP features
                    DrawOptimizedESPBox(screenPos, cached, espColor);
                    
                    // Only draw distance if enabled (minimal overhead)
                    if (config.showDistance)
                    {
                        DrawOptimizedDistanceText(screenPos, cached.distance, espColor);
                    }
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] ESP rendering error: " + ex.Message);
            }
        }

        private void DrawOptimizedESPBox(Vector3 screenPos, CachedEntity cached, Color color)
        {
            try
            {
                float distance = cached.distance;
                float boxSize = Mathf.Max(config.minBoxSize, (config.boxSizeMultiplier / distance));
                
                // Calculate box bounds
                float x = screenPos.x - boxSize / 2;
                float y = screenPos.y - boxSize / 2;
                
                // OPTIMIZED: Simple box outline only (no corners for performance)
                Render.DrawBox(x, y, boxSize, boxSize, color, false);
                
                // OPTIMIZED: Simple name only (no complex calculations)
                GUI.color = color;
                GUI.Label(new Rect(x, y - 20, 100, 20), cached.typeName);
                GUI.color = Color.white;
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Optimized box drawing error: " + ex.Message);
            }
        }
        
        private void DrawOptimizedDistanceText(Vector3 screenPos, float distance, Color color)
        {
            try
            {
                string distanceText = $"{distance:F0}m";
                GUI.color = color;
                GUI.Label(new Rect(screenPos.x - 30, screenPos.y + 20, 60, 20), distanceText);
                GUI.color = Color.white;
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Optimized distance text error: " + ex.Message);
            }
        }

        private void DrawESPBox(Vector3 screenPos, CachedEntity cached, Color color)
        {
            try
            {
                float distance = cached.distance;
                float boxSize = Mathf.Max(config.minBoxSize, (config.boxSizeMultiplier / distance));
                
                // Calculate box bounds
                float x = screenPos.x - boxSize / 2;
                float y = screenPos.y - boxSize / 2;
                
                tempRect.x = x;
                tempRect.y = y;
                tempRect.width = boxSize;
                tempRect.height = boxSize;
                
                // Draw box outline using existing Render method
                Render.DrawBox(tempRect.x, tempRect.y, tempRect.width, tempRect.height, color, false);
                
                // Draw corner indicators manually
                if (config.showCornerIndicators)
                {
                    float cornerLength = 10f;
                    float cornerWidth = 2f;
                    
                    // Top-left corner
                    Render.DrawBox(x, y, cornerLength, cornerWidth, color, true);
                    Render.DrawBox(x, y, cornerWidth, cornerLength, color, true);
                    
                    // Top-right corner
                    Render.DrawBox(x + boxSize - cornerLength, y, cornerLength, cornerWidth, color, true);
                    Render.DrawBox(x + boxSize - cornerWidth, y, cornerWidth, cornerLength, color, true);
                    
                    // Bottom-left corner
                    Render.DrawBox(x, y + boxSize - cornerLength, cornerLength, cornerWidth, color, true);
                    Render.DrawBox(x, y + boxSize - cornerWidth, cornerWidth, cornerLength, color, true);
                    
                    // Bottom-right corner
                    Render.DrawBox(x + boxSize - cornerLength, y + boxSize - cornerWidth, cornerLength, cornerWidth, color, true);
                    Render.DrawBox(x + boxSize - cornerWidth, y + boxSize - cornerLength, cornerWidth, cornerLength, color, true);
                }
                
                // Draw entity name
                tempContent.text = $"{cached.typeName}";
                Vector2 nameSize = GUI.skin.label.CalcSize(tempContent);
                
                GUI.color = color;
                GUI.Label(new Rect(x, y - 20, nameSize.x, nameSize.y), tempContent.text);
                GUI.color = Color.white;
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Box drawing error: " + ex.Message);
            }
        }

        private void DrawHealthBar(Vector3 screenPos, CachedEntity cached)
        {
            try
            {
                float barWidth = 40f;
                float barHeight = 4f;
                float healthPercent = cached.maxHealth > 0 ? (float)cached.health / cached.maxHealth : 0f;
                
                float x = screenPos.x - barWidth / 2;
                float y = screenPos.y + 20f;
                
                // Background
                tempRect.x = x;
                tempRect.y = y;
                tempRect.width = barWidth;
                tempRect.height = barHeight;
                
                GUI.color = Color.black;
                GUI.DrawTexture(tempRect, Texture2D.whiteTexture);
                
                // Health fill
                tempRect.width = barWidth * healthPercent;
                
                // Color based on health percentage
                if (healthPercent > 0.6f)
                    GUI.color = Color.green;
                else if (healthPercent > 0.3f)
                    GUI.color = Color.yellow;
                else
                    GUI.color = Color.red;
                
                GUI.DrawTexture(tempRect, Texture2D.whiteTexture);
                GUI.color = Color.white;
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Health bar error: " + ex.Message);
            }
        }

        private void DrawSnapline(Vector3 screenPos, Color color)
        {
            try
            {
                Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                Render.DrawLine(center.x, center.y, screenPos.x, screenPos.y, color, 1f);
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Snapline error: " + ex.Message);
            }
        }

        private void DrawDistanceText(Vector3 screenPos, float distance, Color color)
        {
            try
            {
                string distanceText = $"{distance:F0}m";
                tempContent.text = distanceText;
                Vector2 textSize = GUI.skin.label.CalcSize(tempContent);
                
                float x = screenPos.x - textSize.x / 2;
                float y = screenPos.y + 30f;
                
                GUI.color = color;
                GUI.Label(new Rect(x, y, textSize.x, textSize.y), distanceText);
                GUI.color = Color.white;
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Distance text error: " + ex.Message);
            }
        }

        private void DrawHealthText(Vector3 screenPos, CachedEntity cached, Color color)
        {
            try
            {
                string healthText = $"{cached.health}/{cached.maxHealth}";
                tempContent.text = healthText;
                Vector2 textSize = GUI.skin.label.CalcSize(tempContent);
                
                float x = screenPos.x - textSize.x / 2;
                float y = screenPos.y + 45f;
                
                GUI.color = color;
                GUI.Label(new Rect(x, y, textSize.x, textSize.y), healthText);
                GUI.color = Color.white;
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Health text error: " + ex.Message);
            }
        }

        private void DrawFOVCircle()
        {
            try
            {
                Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                float radius = config.fovRadius * (Screen.width / 1000f);
                
                Render.DrawCircle(center.x, center.y, radius, Color.white, false);
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] FOV circle error: " + ex.Message);
            }
        }

        private void DrawCrosshair()
        {
            try
            {
                Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);
                float size = 10f;
                
                // Draw crosshair lines
                Render.DrawLine(center.x - size, center.y, center.x + size, center.y, Color.white, 1f);
                Render.DrawLine(center.x, center.y - size, center.x, center.y + size, Color.white, 1f);
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Crosshair error: " + ex.Message);
            }
        }

        private void RenderMenu()
        {
            try
            {
                GUI.Box(menuRect, "Optimized ESP Menu");
                
                GUILayout.BeginArea(new Rect(menuRect.x + 10, menuRect.y + 30, menuRect.width - 20, menuRect.height - 40));
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                
                // Master toggle
                GUILayout.BeginHorizontal();
                espEnabled = GUILayout.Toggle(espEnabled, "Master ESP Toggle");
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                
                // OPTIMIZED: Enemy-only toggle
                GUILayout.Label("ESP Features", GUI.skin.box);
                config.showEnemies = GUILayout.Toggle(config.showEnemies, "Show Enemies Only");
                GUILayout.Space(10);
                
                // OPTIMIZED: Essential visual options only
                GUILayout.Label("Visual Options", GUI.skin.box);
                config.showDistance = GUILayout.Toggle(config.showDistance, "Distance Display");
                GUILayout.Space(10);
                
                // Distance slider
                GUILayout.Label($"Max ESP Distance: {config.maxESPDistance:F0}m");
                config.maxESPDistance = GUILayout.HorizontalSlider(config.maxESPDistance, 50f, 300f);
                GUILayout.Space(10);
                
                // OPTIMIZED: Essential performance info only
                GUILayout.Label("Performance Info", GUI.skin.box);
                GUILayout.Label($"FPS: {performanceMonitor.CurrentFPS:F0}");
                GUILayout.Label($"Entities: {cachedEntities.Count}/{MAX_ENTITIES}");
                GUILayout.Label($"Camera: {(mainCamera != null ? "Active" : "None")}");
                GUILayout.Space(10);
                
                // Control buttons
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Save Config"))
                {
                    config.SaveConfiguration();
                    RobustDebugger.Log("[EnhancedESP] Configuration saved");
                }
                
                if (GUILayout.Button("Reset Config"))
                {
                    config.ResetToDefaults();
                    RobustDebugger.Log("[EnhancedESP] Configuration reset");
                }
                GUILayout.EndHorizontal();
                
                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Menu rendering error: " + ex.Message);
            }
        }

        private void RenderPerformanceOverlay()
        {
            try
            {
                string perfText = $"FPS: {performanceMonitor.CurrentFPS:F0} | Entities: {cachedEntities.Count}";
                tempContent.text = perfText;
                Vector2 textSize = GUI.skin.label.CalcSize(tempContent);
                
                float x = Screen.width - textSize.x - 10;
                float y = 10;
                
                GUI.color = Color.white;
                GUI.Box(new Rect(x - 5, y - 5, textSize.x + 10, textSize.y + 10), "");
                GUI.Label(new Rect(x, y, textSize.x, textSize.y), perfText);
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Performance overlay error: " + ex.Message);
            }
        }

        void OnDestroy()
        {
            try
            {
                config.SaveConfiguration();
                RobustDebugger.Log("[EnhancedESP] Enhanced ESP Manager destroyed");
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Cleanup error: " + ex.Message);
            }
        }
    }

    public class CachedEntity
    {
        public Entity entity;
        public Vector3 position;
        public float distance;
        public Color color;
        public string typeName;
        public int health;
        public int maxHealth;
        public float lastUpdate; // For cache management
    }
}
