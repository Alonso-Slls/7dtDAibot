using UnityEngine;
using System.Collections.Generic;

public class Hacks : MonoBehaviour
{
    // Global entity lists
    public static List<EntityEnemy> eEnemy = new List<EntityEnemy>();
    
    // Timing variables
    private float lastUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 5f; // Update every 5 seconds
    
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
        
        // Update entity lists periodically
        if (Time.time - lastUpdateTime > UPDATE_INTERVAL)
        {
            updateObjects();
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
        GUILayout.BeginArea(new Rect(pos.x, pos.y, 200, 150));
        
        GUILayout.Label("7D2D ESP Menu", GUI.skin.box);
        
        // Draw toggles automatically with IMGUI
        bool oldESP = Modules.ESPSettings.ShowEnemyESP;
        bool oldBones = Modules.ESPSettings.ShowEnemyBones;
        
        Modules.ESPSettings.ShowEnemyESP = GUILayout.Toggle(Modules.ESPSettings.ShowEnemyESP, $"Enemy ESP {(Modules.ESPSettings.ShowEnemyESP ? "[ON]" : "[OFF]")}");
        Modules.ESPSettings.ShowEnemyBones = GUILayout.Toggle(Modules.ESPSettings.ShowEnemyBones, $"Enemy Bones {(Modules.ESPSettings.ShowEnemyBones ? "[ON]" : "[OFF]")}");
        
        // Save settings if they changed
        if (oldESP != Modules.ESPSettings.ShowEnemyESP || oldBones != Modules.ESPSettings.ShowEnemyBones)
        {
            Modules.ESPSettings.SaveSettings();
        }
        
        GUILayout.Space(10);
        GUILayout.Label($"Enemies: {eEnemy.Count}", GUI.skin.label);
        GUILayout.Label("Insert: Toggle Menu", GUI.skin.label);
        GUILayout.Label("End: Unload", GUI.skin.label);
        
        GUILayout.EndArea();
    }
    
    private static Vector2 GetMenuPosition()
    {
        return new Vector2(
            (Screen.width - 200) / 2,
            (Screen.height - 150) / 2
        );
    }
    
    public static void updateObjects()
    {
        try
        {
            // Clear existing lists
            eEnemy.Clear();
            
            // Find all enemies only (optimized for current ESP needs)
            var enemies = GameObject.FindObjectsOfType<EntityEnemy>();
            foreach (var enemy in enemies)
            {
                if (enemy != null && enemy.IsAlive())
                {
                    eEnemy.Add(enemy);
                }
            }
            
            Debug.Log($"Updated entities: {eEnemy.Count} enemies");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error updating objects: {e.Message}");
        }
    }
    
    void OnDestroy()
    {
        Debug.Log("Hacks component destroyed");
    }
}
