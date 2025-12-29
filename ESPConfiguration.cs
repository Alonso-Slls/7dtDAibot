using System;
using System.IO;
using UnityEngine;

namespace SevenDtDAibot
{
    [Serializable]
    public class ESPConfiguration
    {
        // Colors
        public SerializableColor enemyColor = new SerializableColor(1.0f, 0.0f, 0.0f, 1.0f);
        public SerializableColor playerColor = new SerializableColor(0.0f, 1.0f, 0.0f, 1.0f);
        public SerializableColor animalColor = new SerializableColor(1.0f, 1.0f, 0.0f, 1.0f);
        public SerializableColor itemColor = new SerializableColor(0.0f, 1.0f, 1.0f, 1.0f);
        
        // Performance settings
        public float maxESPDistance = 200.0f;
        public float entityScanInterval = 0.1f;
        public bool enableVerboseLogging = false;
        
        // Entity toggles
        public bool showEnemies = true;
        public bool showPlayers = true;
        public bool showAnimals = true;
        public bool showItems = true;
        
        // Visual features
        public bool showHealthBars = true;
        public bool showSnaplines = false;
        public bool showDistance = true;
        public bool showHealthText = false;
        public bool showFOVCircle = false;
        public bool showCrosshair = false;
        public bool showCornerIndicators = true;
        public bool showPerformanceInfo = true;
        
        // Hotkeys
        public int toggleMenuKey = 45;  // Insert key
        public int toggleESPKey = 112;  // F1 key
        
        // Visual settings
        public float fovRadius = 100.0f;
        public float minBoxSize = 20.0f;
        public float boxSizeMultiplier = 1000.0f;
        
        private string configPath;
        
        public ESPConfiguration()
        {
            configPath = Path.Combine(Application.persistentDataPath, "7dtDAibot", "esp_config.json");
            
            // Ensure directory exists
            string directory = Path.GetDirectoryName(configPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        
        public void LoadConfiguration()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    // For now, use default configuration
                    // TODO: Implement JSON parsing manually or use alternative
                    RobustDebugger.Log($"[ESPConfig] Configuration file found at: {configPath}, using defaults");
                }
                else
                {
                    RobustDebugger.Log("[ESPConfig] No configuration file found, using defaults");
                    SaveConfiguration();
                }
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[ESPConfig] Failed to load configuration: {ex.Message}");
                ResetToDefaults();
            }
        }
        
        public void SaveConfiguration()
        {
            try
            {
                // For now, save a simple config file
                string configText = $"# Enhanced ESP Configuration\n" +
                                  $"MaxDistance={maxESPDistance}\n" +
                                  $"ShowEnemies={showEnemies}\n" +
                                  $"ShowPlayers={showPlayers}\n" +
                                  $"ShowAnimals={showAnimals}\n" +
                                  $"ShowItems={showItems}\n";
                
                File.WriteAllText(configPath, configText);
                RobustDebugger.Log($"[ESPConfig] Configuration saved to: {configPath}");
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[ESPConfig] Failed to save configuration: {ex.Message}");
            }
        }
        
        public void ResetToDefaults()
        {
            try
            {
                // Reset to default values
                enemyColor = new SerializableColor(1.0f, 0.0f, 0.0f, 1.0f);
                playerColor = new SerializableColor(0.0f, 1.0f, 0.0f, 1.0f);
                animalColor = new SerializableColor(1.0f, 1.0f, 0.0f, 1.0f);
                itemColor = new SerializableColor(0.0f, 1.0f, 1.0f, 1.0f);
                
                maxESPDistance = 200.0f;
                entityScanInterval = 0.1f;
                enableVerboseLogging = false;
                
                showEnemies = true;
                showPlayers = true;
                showAnimals = true;
                showItems = true;
                
                showHealthBars = true;
                showSnaplines = false;
                showDistance = true;
                showHealthText = false;
                showFOVCircle = false;
                showCrosshair = false;
                showCornerIndicators = true;
                showPerformanceInfo = true;
                
                toggleMenuKey = 45;
                toggleESPKey = 112;
                
                fovRadius = 100.0f;
                minBoxSize = 20.0f;
                boxSizeMultiplier = 1000.0f;
                
                SaveConfiguration();
                RobustDebugger.Log("[ESPConfig] Configuration reset to defaults");
            }
            catch (Exception ex)
            {
                RobustDebugger.LogError($"[ESPConfig] Failed to reset configuration: {ex.Message}");
            }
        }
        
        public Color GetEnemyColor()
        {
            return enemyColor.ToColor();
        }
        
        public Color GetPlayerColor()
        {
            return playerColor.ToColor();
        }
        
        public Color GetAnimalColor()
        {
            return animalColor.ToColor();
        }
        
        public Color GetItemColor()
        {
            return itemColor.ToColor();
        }
        
        public void SetEnemyColor(Color color)
        {
            enemyColor = new SerializableColor(color);
        }
        
        public void SetPlayerColor(Color color)
        {
            playerColor = new SerializableColor(color);
        }
        
        public void SetAnimalColor(Color color)
        {
            animalColor = new SerializableColor(color);
        }
        
        public void SetItemColor(Color color)
        {
            itemColor = new SerializableColor(color);
        }
    }
    
    [Serializable]
    public class SerializableColor
    {
        public float r;
        public float g;
        public float b;
        public float a;
        
        public SerializableColor() { }
        
        public SerializableColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        
        public SerializableColor(Color color)
        {
            this.r = color.r;
            this.g = color.g;
            this.b = color.b;
            this.a = color.a;
        }
        
        public Color ToColor()
        {
            return new Color(r, g, b, a);
        }
    }
}
