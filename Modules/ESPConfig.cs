using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Game_7D2D.Modules
{
    /// <summary>
    /// Configuration management for ESP system with JSON persistence.
    /// </summary>
    public static class ESPConfig
    {
        #region Visual Settings
        
        public static Color EnemyColor { get; set; } = Color.red;
        public static Color BoneColor { get; set; } = Color.green;
        public static bool ShowEnemyNames { get; set; } = true;
        public static bool ShowHeadBoxes { get; set; } = true;
        
        #endregion
        
        #region Performance Settings
        
        public static float MaxESPDistance { get; set; } = 200f;
        public static float EntityScanInterval { get; set; } = 0.1f;
        public static float MinBoxSize { get; set; } = 20f;
        public static float BoxSizeMultiplier { get; set; } = 1000f;
        
        #endregion
        
        #region Input Settings
        
        public static KeyCode ToggleMenuKey { get; set; } = KeyCode.Insert;
        public static KeyCode UnloadKey { get; set; } = KeyCode.End;
        
        #endregion
        
        #region File Operations
        
        private static readonly string ConfigPath = Path.Combine(
            Application.persistentDataPath, "esp_config.json");
        
        public static void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    var config = JsonUtility.FromJson<ConfigData>(json);
                    ApplyConfig(config);
                }
                else
                {
                    SaveConfig(); // Create default config
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ESPConfig] Failed to load config: {ex.Message}");
            }
        }
        
        public static void SaveConfig()
        {
            try
            {
                var config = new ConfigData
                {
                    // Visual
                    EnemyColorR = EnemyColor.r,
                    EnemyColorG = EnemyColor.g,
                    EnemyColorB = EnemyColor.b,
                    EnemyColorA = EnemyColor.a,
                    BoneColorR = BoneColor.r,
                    BoneColorG = BoneColor.g,
                    BoneColorB = BoneColor.b,
                    BoneColorA = BoneColor.a,
                    ShowEnemyNames = ShowEnemyNames,
                    ShowHeadBoxes = ShowHeadBoxes,
                    
                    // Performance
                    MaxESPDistance = MaxESPDistance,
                    EntityScanInterval = EntityScanInterval,
                    MinBoxSize = MinBoxSize,
                    BoxSizeMultiplier = BoxSizeMultiplier,
                    
                    // Input
                    ToggleMenuKey = ToggleMenuKey.ToString(),
                    UnloadKey = UnloadKey.ToString()
                };
                
                string json = JsonUtility.ToJson(config, true);
                File.WriteAllText(ConfigPath, json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ESPConfig] Failed to save config: {ex.Message}");
            }
        }
        
        private static void ApplyConfig(ConfigData config)
        {
            // Visual
            EnemyColor = new Color(config.EnemyColorR, config.EnemyColorG, config.EnemyColorB, config.EnemyColorA);
            BoneColor = new Color(config.BoneColorR, config.BoneColorG, config.BoneColorB, config.BoneColorA);
            ShowEnemyNames = config.ShowEnemyNames;
            ShowHeadBoxes = config.ShowHeadBoxes;
            
            // Performance
            MaxESPDistance = config.MaxESPDistance;
            EntityScanInterval = config.EntityScanInterval;
            MinBoxSize = config.MinBoxSize;
            BoxSizeMultiplier = config.BoxSizeMultiplier;
            
            // Input
            if (System.Enum.TryParse<KeyCode>(config.ToggleMenuKey, out KeyCode menuKey))
                ToggleMenuKey = menuKey;
            if (System.Enum.TryParse<KeyCode>(config.UnloadKey, out KeyCode unloadKey))
                UnloadKey = unloadKey;
        }
        
        #endregion
        
        #region Data Classes
        
        [System.Serializable]
        private class ConfigData
        {
            // Visual
            public float EnemyColorR, EnemyColorG, EnemyColorB, EnemyColorA;
            public float BoneColorR, BoneColorG, BoneColorB, BoneColorA;
            public bool ShowEnemyNames;
            public bool ShowHeadBoxes;
            
            // Performance
            public float MaxESPDistance;
            public float EntityScanInterval;
            public float MinBoxSize;
            public float BoxSizeMultiplier;
            
            // Input
            public string ToggleMenuKey;
            public string UnloadKey;
        }
        
        #endregion
    }
}
