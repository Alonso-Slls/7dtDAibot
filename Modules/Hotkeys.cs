using UnityEngine;
using System.Collections;

namespace Modules
{
    public class Hotkeys
    {
        public static void hotkeys()
        {
            // Toggle menu with Insert key
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                Hacks.showMenu = !Hacks.showMenu;
                Debug.Log($"Menu {(Hacks.showMenu ? "shown" : "hidden")}");
            }
            
            // Unload DLL with End key
            if (Input.GetKeyDown(KeyCode.End))
            {
                UnloadHack();
            }
            
            // Toggle Enemy ESP with F1 key
            if (Input.GetKeyDown(KeyCode.F1))
            {
                SevenDtDAibot.ESPSettings.ShowEnemyESP = !SevenDtDAibot.ESPSettings.ShowEnemyESP;
                Debug.Log($"Enemy ESP {(SevenDtDAibot.ESPSettings.ShowEnemyESP ? "enabled" : "disabled")}");
            }
            
            // Force update entities with F3 key
            if (Input.GetKeyDown(KeyCode.F3))
            {
                var hacksComponent = Object.FindObjectOfType<Hacks>();
                if (hacksComponent != null)
                {
                    hacksComponent.StartCoroutine(hacksComponent.GetType().GetMethod("UpdateEntitiesCoroutine", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        .Invoke(hacksComponent, null) as IEnumerator);
                    Debug.Log("Entity list force updated via coroutine");
                }
            }
            
            // NEW: Export debug logs with F4 key
            if (Input.GetKeyDown(KeyCode.F4))
            {
                ExportDebugLogs();
            }
            
            // NEW: Export debug logs with custom name using F5 key
            if (Input.GetKeyDown(KeyCode.F5))
            {
                ExportDebugLogs("manual_custom");
            }
            
            // Additional hotkeys can be added here
            // Example: Toggle different ESP features, change colors, etc.
        }
        
        public static void ExportDebugLogs(string suffix = "manual")
        {
            try
            {
                Debug.Log($"[Hotkeys] Starting debug log export ({suffix})...");
                
                bool success = SevenDtDAibot.RobustDebugger.ExportLogs(suffix);
                
                if (success)
                {
                    string exportDir = SevenDtDAibot.RobustDebugger.GetExportDirectory();
                    Debug.Log($"[Hotkeys] Debug logs exported successfully to: {exportDir}");
                    
                    // Show on-screen notification if menu is visible
                    if (Hacks.showMenu)
                    {
                        // This will be visible next frame in the menu
                    }
                }
                else
                {
                    Debug.LogError("[Hotkeys] Debug log export failed");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Hotkeys] Error exporting debug logs: {e.Message}");
            }
        }
        
        private static void UnloadHack()
        {
            try
            {
                Debug.Log("Unloading 7D2D ESP Hack...");
                
                // Export logs before unloading
                SevenDtDAibot.RobustDebugger.CleanupAndExport();
                
                // Find and destroy the hack GameObject
                GameObject hackObject = GameObject.Find("ESP_Hack");
                if (hackObject != null)
                {
                    Object.DestroyImmediate(hackObject);
                    Debug.Log("Hack GameObject destroyed");
                }
                
                // Alternative: Destroy this component
                Hacks hacksComponent = Object.FindObjectOfType<Hacks>();
                if (hacksComponent != null)
                {
                    Object.DestroyImmediate(hacksComponent.gameObject);
                    Debug.Log("Hacks component destroyed");
                }
                
                Debug.Log("7D2D ESP Hack unloaded successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error unloading hack: {e.Message}");
            }
        }
        
        public static bool IsKeyPressed(KeyCode key)
        {
            return Input.GetKey(key);
        }
        
        public static bool IsKeyDown(KeyCode key)
        {
            return Input.GetKeyDown(key);
        }
        
        public static bool IsKeyUp(KeyCode key)
        {
            return Input.GetKeyUp(key);
        }
        
        // Helper method to check if a key combination is pressed
        public static bool IsKeyComboPressed(KeyCode key1, KeyCode key2)
        {
            return Input.GetKey(key1) && Input.GetKeyDown(key2);
        }
        
        // Method to get current key states for debugging
        public static string GetKeyStateString()
        {
            string state = "Key States:\n";
            state += $"Insert: {Input.GetKey(KeyCode.Insert)}\n";
            state += $"End: {Input.GetKey(KeyCode.End)}\n";
            state += $"F1: {Input.GetKey(KeyCode.F1)}\n";
            state += $"F3: {Input.GetKey(KeyCode.F3)}\n";
            state += $"F4: {Input.GetKey(KeyCode.F4)} (Export Logs)\n";
            state += $"F5: {Input.GetKey(KeyCode.F5)} (Custom Export)\n";
            state += $"Mouse Left: {Input.GetMouseButton(0)}\n";
            state += $"Mouse Right: {Input.GetMouseButton(1)}\n";
            
            return state;
        }
    }
}
