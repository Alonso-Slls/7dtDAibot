using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Hacks : MonoBehaviour
{
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
        Modules.ESPSettings.LoadSettings();
    }
    
    void Update()
    {
        // Check if game is loaded
        if (!isLoaded && GameManager.Instance != null)
        {
            isLoaded = true;
            Debug.Log("Game detected - ESP ready");
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
        if (!isLoaded) return;
        
        // Draw IMGUI menu
        DrawIMGUI();
        
        // Draw ESP if enabled
        if (Modules.ESPSettings.ShowEnemyESP)
        {
            foreach (var enemy in eEnemy)
            {
                if (enemy != null && enemy.IsAlive())
                {
                    Modules.ESP.esp_drawBox(enemy, Color.red);
                }
            }
        }
    }
    
    private void DrawIMGUI()
    {
        if (!showMenu) 
        {
            Debug.Log("Menu hidden, skipping draw");
            return;
        }
        
        Debug.Log("Drawing IMGUI menu");
        
        // Get centered menu position
        Vector2 pos = GetMenuPosition();
        
        // Begin an automatic layout area
        GUILayout.BeginArea(new Rect(pos.x, pos.y, 200, 180));
        
        GUILayout.Label("7D2D ESP Menu", GUI.skin.box);
        
        // Draw toggles automatically with IMGUI
        bool oldESP = Modules.ESPSettings.ShowEnemyESP;
        float oldDistance = Modules.ESPSettings.MaxESPDistance;
        
        Modules.ESPSettings.ShowEnemyESP = GUILayout.Toggle(Modules.ESPSettings.ShowEnemyESP, $"Enemy ESP {(Modules.ESPSettings.ShowEnemyESP ? "[ON]" : "[OFF]")}");
        
        // Render distance slider
        GUILayout.Label($"Render Distance: {Modules.ESPSettings.MaxESPDistance:F0}m");
        Modules.ESPSettings.MaxESPDistance = GUILayout.HorizontalSlider(Modules.ESPSettings.MaxESPDistance, 50f, 300f);
        
        // Save settings if they changed
        if (oldESP != Modules.ESPSettings.ShowEnemyESP || Mathf.Abs(oldDistance - Modules.ESPSettings.MaxESPDistance) > 0.1f)
        {
            Modules.ESPSettings.SaveSettings();
        }
        
        GUILayout.Space(10);
        GUILayout.Label($"Enemies: {eEnemy.Count}", GUI.skin.label);
        GUILayout.Label($"FPS: {(1.0f / Time.deltaTime):F0}", GUI.skin.label);
        GUILayout.Label("Insert: Toggle Menu", GUI.skin.label);
        GUILayout.Label("F1: Toggle ESP", GUI.skin.label);
        GUILayout.Label("F3: Force Update", GUI.skin.label);
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
        // Stop any running coroutines
        if (entityUpdateCoroutine != null)
        {
            StopCoroutine(entityUpdateCoroutine);
        }
        
        Debug.Log("Hacks component destroyed");
    }
}
