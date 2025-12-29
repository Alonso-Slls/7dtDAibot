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
        private Rect menuRect = new Rect(10, 10, 300, 400);
        private Vector2 scrollPosition = Vector2.zero;
        
        // Reusable objects to reduce GC pressure
        private Rect tempRect = new Rect();
        private GUIContent tempContent = new GUIContent();

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
                
                // Find camera if not found
                if (mainCamera == null)
                {
                    mainCamera = Camera.main;
                    if (mainCamera == null)
                    {
                        mainCamera = FindObjectOfType<Camera>();
                    }
                }
                
                // Scan entities at configured interval
                if (Time.time - lastEntityScan >= config.entityScanInterval)
                {
                    ScanAndCacheEntities();
                    lastEntityScan = Time.time;
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
                if (espEnabled && mainCamera != null)
                {
                    RenderAllESP();
                }
                
                if (showMenu)
                {
                    RenderMenu();
                }
                
                // Performance overlay
                if (config.showPerformanceInfo)
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
                cachedEntities.Clear();
                Vector3 cameraPos = mainCamera.transform.position;
                
                // Scan enemies
                if (config.showEnemies)
                {
                    ScanEntityType<EntityEnemy>(cameraPos, config.GetEnemyColor(), "Enemy");
                }
                
                // Scan players
                if (config.showPlayers)
                {
                    ScanEntityType<EntityPlayer>(cameraPos, config.GetPlayerColor(), "Player");
                }
                
                // Scan animals
                if (config.showAnimals)
                {
                    ScanEntityType<EntityAnimal>(cameraPos, config.GetAnimalColor(), "Animal");
                }
                
                // Scan items
                if (config.showItems)
                {
                    ScanEntityType<EntityItem>(cameraPos, config.GetItemColor(), "Item");
                }
                
                if (config.enableVerboseLogging)
                {
                    RobustDebugger.Log($"[EnhancedESP] Scanned {cachedEntities.Count} entities");
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] Entity scanning error: " + ex.Message);
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
                foreach (CachedEntity cached in cachedEntities)
                {
                    if (cached.entity == null) continue;
                    
                    Vector3 screenPos = mainCamera.WorldToScreenPoint(cached.position);
                    
                    // Skip if behind camera
                    if (screenPos.z < 0) continue;
                    
                    screenPos.y = Screen.height - screenPos.y;
                    
                    // Calculate opacity based on distance
                    float opacity = Mathf.Lerp(1.0f, 0.3f, cached.distance / config.maxESPDistance);
                    Color espColor = new Color(cached.color.r, cached.color.g, cached.color.b, cached.color.a * opacity);
                    
                    // Draw ESP box
                    DrawESPBox(screenPos, cached, espColor);
                    
                    // Draw additional features
                    if (config.showHealthBars)
                    {
                        DrawHealthBar(screenPos, cached);
                    }
                    
                    if (config.showSnaplines)
                    {
                        DrawSnapline(screenPos, espColor);
                    }
                    
                    if (config.showDistance)
                    {
                        DrawDistanceText(screenPos, cached.distance, espColor);
                    }
                    
                    if (config.showHealthText)
                    {
                        DrawHealthText(screenPos, cached, espColor);
                    }
                }
                
                // Draw global overlays
                if (config.showFOVCircle)
                {
                    DrawFOVCircle();
                }
                
                if (config.showCrosshair)
                {
                    DrawCrosshair();
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError("[EnhancedESP] ESP rendering error: " + ex.Message);
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
                GUI.Box(menuRect, "Enhanced ESP Menu v2.0");
                
                GUILayout.BeginArea(new Rect(menuRect.x + 10, menuRect.y + 30, menuRect.width - 20, menuRect.height - 40));
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                
                // Master toggle
                GUILayout.BeginHorizontal();
                espEnabled = GUILayout.Toggle(espEnabled, "Master ESP Toggle");
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                
                // ESP Features
                GUILayout.Label("ESP Features", GUI.skin.box);
                config.showEnemies = GUILayout.Toggle(config.showEnemies, "Show Enemies");
                config.showPlayers = GUILayout.Toggle(config.showPlayers, "Show Players");
                config.showAnimals = GUILayout.Toggle(config.showAnimals, "Show Animals");
                config.showItems = GUILayout.Toggle(config.showItems, "Show Items");
                GUILayout.Space(10);
                
                // Visual Options
                GUILayout.Label("Visual Options", GUI.skin.box);
                config.showHealthBars = GUILayout.Toggle(config.showHealthBars, "Health Bars");
                config.showSnaplines = GUILayout.Toggle(config.showSnaplines, "Snaplines");
                config.showDistance = GUILayout.Toggle(config.showDistance, "Distance Display");
                config.showHealthText = GUILayout.Toggle(config.showHealthText, "Health Text");
                config.showFOVCircle = GUILayout.Toggle(config.showFOVCircle, "FOV Circle");
                config.showCrosshair = GUILayout.Toggle(config.showCrosshair, "Crosshair");
                config.showCornerIndicators = GUILayout.Toggle(config.showCornerIndicators, "Corner Indicators");
                GUILayout.Space(10);
                
                // Distance slider
                GUILayout.Label($"Max ESP Distance: {config.maxESPDistance:F0}m");
                config.maxESPDistance = GUILayout.HorizontalSlider(config.maxESPDistance, 50f, 500f);
                GUILayout.Space(10);
                
                // Performance info
                GUILayout.Label("Performance Info", GUI.skin.box);
                GUILayout.Label($"FPS: {performanceMonitor.CurrentFPS:F0}");
                GUILayout.Label($"Frame Time: {performanceMonitor.FrameTime:F2}ms");
                GUILayout.Label($"Entities: {cachedEntities.Count}");
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
    }
}
