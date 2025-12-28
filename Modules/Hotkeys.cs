using UnityEngine;

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
                Hacks.enemyESP = !Hacks.enemyESP;
                Debug.Log($"Enemy ESP {(Hacks.enemyESP ? "enabled" : "disabled")}");
            }
            
            // Toggle Enemy Bones with F2 key
            if (Input.GetKeyDown(KeyCode.F2))
            {
                Hacks.enemyBones = !Hacks.enemyBones;
                Debug.Log($"Enemy Bones {(Hacks.enemyBones ? "enabled" : "disabled")}");
            }
            
            // Force update entities with F3 key
            if (Input.GetKeyDown(KeyCode.F3))
            {
                Hacks.updateObjects();
                Debug.Log("Entity list force updated");
            }
            
            // Additional hotkeys can be added here
            // Example: Toggle different ESP features, change colors, etc.
        }
        
        private static void UnloadHack()
        {
            try
            {
                Debug.Log("Unloading 7D2D ESP Hack...");
                
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
            state += $"F2: {Input.GetKey(KeyCode.F2)}\n";
            state += $"F3: {Input.GetKey(KeyCode.F3)}\n";
            state += $"Mouse Left: {Input.GetMouseButton(0)}\n";
            state += $"Mouse Right: {Input.GetMouseButton(1)}\n";
            
            return state;
        }
    }
}
