using UnityEngine;
using System.Collections.Generic;

public class Hacks : MonoBehaviour
{
    // Global entity lists
    public static List<EntityEnemy> eEnemy = new List<EntityEnemy>();
    public static List<EntityPlayer> ePlayers = new List<EntityPlayer>();
    public static List<EntityItem> eItems = new List<EntityItem>();
    
    // Timing variables
    private float lastUpdateTime = 0f;
    private const float UPDATE_INTERVAL = 5f; // Update every 5 seconds
    
    // Game state
    public static bool isLoaded = false;
    public static bool showMenu = false;
    
    // ESP settings
    public static bool enemyESP = true;
    public static bool enemyBones = true;
    
    void Start()
    {
        Debug.Log("Hacks component initialized");
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
        
        // Draw menu
        Modules.UI.DrawMenu();
        
        // Draw ESP if enabled
        if (enemyESP)
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
    
    public static void updateObjects()
    {
        try
        {
            // Clear existing lists
            eEnemy.Clear();
            ePlayers.Clear();
            eItems.Clear();
            
            // Find all enemies
            var enemies = GameObject.FindObjectsOfType<EntityEnemy>();
            foreach (var enemy in enemies)
            {
                if (enemy != null && enemy.IsAlive())
                {
                    eEnemy.Add(enemy);
                }
            }
            
            // Find all players
            var players = GameObject.FindObjectsOfType<EntityPlayer>();
            foreach (var player in players)
            {
                if (player != null && player.IsAlive())
                {
                    ePlayers.Add(player);
                }
            }
            
            // Find all items
            var items = GameObject.FindObjectsOfType<EntityItem>();
            foreach (var item in items)
            {
                if (item != null)
                {
                    eItems.Add(item);
                }
            }
            
            Debug.Log($"Updated entities: {eEnemy.Count} enemies, {ePlayers.Count} players, {eItems.Count} items");
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
