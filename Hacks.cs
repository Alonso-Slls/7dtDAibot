using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class Hacks : MonoBehaviour
{
    // Canvas ESP Manager
    private Modules.CanvasESPManager canvasESPManager;
    
    // Global entity lists
    public static List<EntityEnemy> eEnemy = new List<EntityEnemy>();
    
    // Timing variables
    private float lastUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 5f; // Update every 5 seconds
    
    // Coroutine management
    private Coroutine entityUpdateCoroutine;
    private bool isUpdatingEntities = false;
    private const int ENTITIES_PER_FRAME = 10; // Process 10 entities per frame
    
    // Game state
    public static bool isLoaded = false;
    public static bool showMenu = true; // Start with menu visible for testing
    
    void Start()
    {
        Debug.Log("Hacks component initialized");
        SevenDtDAibot.ESPSettings.LoadSettings();
        
        // Initialize Canvas ESP Manager
        InitializeCanvasESP();
        
        // Initialize RobustDebugger for better logging
        SevenDtDAibot.RobustDebugger.Initialize();
        SevenDtDAibot.RobustDebugger.Log("[Hacks] Component started successfully");
    }
    
    void InitializeCanvasESP()
    {
        // Create and initialize Canvas ESP Manager
        GameObject canvasESPManagerObj = new GameObject("CanvasESPManager");
        canvasESPManagerObj.transform.SetParent(transform);
        canvasESPManager = canvasESPManagerObj.AddComponent<Modules.CanvasESPManager>();
        
        DontDestroyOnLoad(canvasESPManagerObj);
        
        SevenDtDAibot.RobustDebugger.Log("[Hacks] Canvas ESP Manager initialized");
    }
    
    void Update()
    {
        // Check if game is loaded using multiple methods
        if (!isLoaded)
        {
            bool gameDetected = false;
            
            // Primary check: GameManager.Instance
            if (GameManager.Instance != null)
            {
                gameDetected = true;
            }
            // Fallback 1: Check if we're in a game scene
            else if (SceneManager.GetActiveScene().name != null && 
                   (SceneManager.GetActiveScene().name.Contains("Game") || 
                    SceneManager.GetActiveScene().name.Contains("Scene")))
            {
                gameDetected = true;
            }
            // Fallback 2: Check for player existence
            else if (GameObject.FindObjectOfType<EntityPlayer>() != null)
            {
                gameDetected = true;
            }
            // Fallback 3: Check for camera
            else if (Camera.main != null)
            {
                gameDetected = true;
            }
            
            if (gameDetected)
            {
                isLoaded = true;
                Debug.Log("Game detected - ESP ready");
                SevenDtDAibot.RobustDebugger.Log("[Hacks] Game state detected, ESP functionality enabled");
            }
        }
        
        // Handle hotkeys
        Modules.Hotkeys.hotkeys();
        
        // Update entity lists periodically using coroutines
        if (Time.time - lastUpdateTime > UPDATE_INTERVAL && !isUpdatingEntities)
        {
            entityUpdateCoroutine = StartCoroutine(UpdateEntitiesCoroutine());
            lastUpdateTime = Time.time;
        }
    }
    
    void OnGUI()
    {
        try
        {
            // Always try to draw menu first (for testing)
            DrawIMGUI();
            
            // Only draw ESP if game is loaded and enabled
            if (!isLoaded)
            {
                // Show loading status
                GUI.Label(new Rect(10, 10, 200, 20), "Waiting for game detection...");
                return;
            }
            
            // Draw ESP using Canvas system if enabled
            if (SevenDtDAibot.ESPSettings.ShowEnemyESP && canvasESPManager != null)
            {
                try
                {
                    // Convert list to array for Canvas ESP Manager
                    EntityEnemy[] enemyArray = eEnemy.ToArray();
                    canvasESPManager.RenderESP(enemyArray);
                    
                    // Debug info
                    if (enemyArray.Length > 0)
                    {
                        SevenDtDAibot.RobustDebugger.Log($"[Hacks] Rendered Canvas ESP for {enemyArray.Length} enemies");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[Hacks] Canvas ESP error: {ex.Message}");
                    SevenDtDAibot.RobustDebugger.LogError($"[Hacks] Canvas ESP error: {ex.Message}");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Hacks] OnGUI error: {ex.Message}");
            SevenDtDAibot.RobustDebugger.LogError($"[Hacks] OnGUI error: {ex.Message}");
            
            // Show error on screen
            GUI.Label(new Rect(10, 50, 300, 40), $"ESP Error: {ex.Message}");
        }
    }
    
    private void DrawIMGUI()
    {
        if (!showMenu) 
        {
            return; // Silently return when menu is hidden
        }
        
        // Get centered menu position
        Vector2 pos = GetMenuPosition();
        
        // Begin an automatic layout area
        GUILayout.BeginArea(new Rect(pos.x, pos.y, 280, 350));
        
        GUILayout.Label("7D2D ESP Menu", GUI.skin.box);
        
        // Game status
        GUI.color = isLoaded ? Color.white : Color.red;
        GUILayout.Label($"Game Status: {(isLoaded ? "Loaded" : "Waiting")}");
        GUI.color = Color.white;
        
        GUILayout.Space(5);
        
        // Draw toggles automatically with IMGUI
        bool oldESP = SevenDtDAibot.ESPSettings.ShowEnemyESP;
        float oldDistance = SevenDtDAibot.ESPSettings.MaxESPDistance;
        
        SevenDtDAibot.ESPSettings.ShowEnemyESP = GUILayout.Toggle(SevenDtDAibot.ESPSettings.ShowEnemyESP, 
            $"Enemy ESP {(SevenDtDAibot.ESPSettings.ShowEnemyESP ? "[ON]" : "[OFF]")}");
        
        // Render distance slider
        GUILayout.Label($"Render Distance: {SevenDtDAibot.ESPSettings.MaxESPDistance:F0}m");
        SevenDtDAibot.ESPSettings.MaxESPDistance = GUILayout.HorizontalSlider(SevenDtDAibot.ESPSettings.MaxESPDistance, 50f, 300f);
        
        // Save settings if they changed
        if (oldESP != SevenDtDAibot.ESPSettings.ShowEnemyESP || Mathf.Abs(oldDistance - SevenDtDAibot.ESPSettings.MaxESPDistance) > 0.1f)
        {
            SevenDtDAibot.ESPSettings.SaveSettings();
        }
        
        GUILayout.Space(10);
        
        // Status information
        GUILayout.Label($"Enemies Found: {eEnemy.Count}", GUI.skin.label);
        GUI.color = Camera.main != null ? Color.white : Color.red;
        GUILayout.Label($"Camera: {(Camera.main != null ? "Available" : "Not Found")}");
        GUI.color = Color.white;
        GUILayout.Label($"FPS: {(1.0f / Time.deltaTime):F0}", GUI.skin.label);
        
        GUILayout.Space(10);
        
        // Debug Log Export Section
        GUILayout.Label("Debug Log Export:", GUI.skin.box);
        
        // Show export directory
        string exportDir = SevenDtDAibot.RobustDebugger.GetExportDirectory();
        GUILayout.Label($"Export Dir: {exportDir}", GUI.skin.label);
        
        // Export buttons
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Export Logs (F4)"))
        {
            Modules.Hotkeys.ExportDebugLogs("manual_menu");
        }
        if (GUILayout.Button("Custom Export (F5)"))
        {
            Modules.Hotkeys.ExportDebugLogs("custom_menu");
        }
        GUILayout.EndHorizontal();
        
        // Show recent log entries
        GUILayout.Space(5);
        GUILayout.Label("Recent Logs:", GUI.skin.label);
        string[] recentLogs = SevenDtDAibot.RobustDebugger.GetRecentLogs(3);
        foreach (string log in recentLogs)
        {
            // Truncate long logs for display
            string displayLog = log.Length > 50 ? log.Substring(0, 47) + "..." : log;
            GUILayout.Label($"  {displayLog}", GUI.skin.label);
        }
        
        GUILayout.Space(10);
        
        // Controls
        GUILayout.Label("Controls:", GUI.skin.box);
        GUILayout.Label("Insert: Toggle Menu", GUI.skin.label);
        GUILayout.Label("F1: Toggle ESP", GUI.skin.label);
        GUILayout.Label("F3: Force Update", GUI.skin.label);
        GUILayout.Label("F4: Export Logs", GUI.skin.label);
        GUILayout.Label("F5: Custom Export", GUI.skin.label);
        GUILayout.Label("End: Unload", GUI.skin.label);
        
        GUILayout.EndArea();
    }
    
    private static Vector2 GetMenuPosition()
    {
        return new Vector2(
            (Screen.width - 200) / 2,
            (Screen.height - 180) / 2
        );
    }
    
    public static void updateObjects()
    {
        // Legacy method - now handled by coroutine
        Debug.LogWarning("updateObjects() called directly - use UpdateEntitiesCoroutine instead");
    }
    
    private IEnumerator UpdateEntitiesCoroutine()
    {
        if (isUpdatingEntities) yield break;
        isUpdatingEntities = true;
        
        // Clear existing lists
        eEnemy.Clear();
        
        // Get all enemies in one frame (still fast)
        EntityEnemy[] allEnemies;
        try
        {
            allEnemies = GameObject.FindObjectsOfType<EntityEnemy>();
            Debug.Log($"Found {allEnemies.Length} total enemies to process");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error finding enemies: {e.Message}");
            isUpdatingEntities = false;
            yield break;
        }
        
        // Process enemies in chunks to avoid frame drops
        for (int i = 0; i < allEnemies.Length; i += ENTITIES_PER_FRAME)
        {
            try
            {
                int endIndex = Mathf.Min(i + ENTITIES_PER_FRAME, allEnemies.Length);
                
                for (int j = i; j < endIndex; j++)
                {
                    var enemy = allEnemies[j];
                    if (enemy != null && enemy.IsAlive())
                    {
                        eEnemy.Add(enemy);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error processing enemy chunk: {e.Message}");
            }
            
            // Yield every chunk to prevent frame drops
            if (i + ENTITIES_PER_FRAME < allEnemies.Length)
            {
                yield return null; // Wait for next frame
            }
        }
        
        Debug.Log($"Coroutine updated entities: {eEnemy.Count} valid enemies");
        isUpdatingEntities = false;
    }
    
    void OnDestroy()
    {
        try
        {
            // Stop any running coroutines
            if (entityUpdateCoroutine != null)
            {
                StopCoroutine(entityUpdateCoroutine);
            }
            
            // Export logs automatically when component is destroyed
            SevenDtDAibot.RobustDebugger.Log("[Hacks] Component being destroyed - exporting logs...");
            SevenDtDAibot.RobustDebugger.CleanupAndExport();
            
            Debug.Log("Hacks component destroyed with log export");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[Hacks] Error during cleanup: {ex.Message}");
        }
    }
}
