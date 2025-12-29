using UnityEngine;

namespace Modules
{
    public static class ESPSettings
    {
        public static bool ShowEnemyESP { get; set; } = true;
        public static float MaxESPDistance { get; set; } = 100f;
        
        public static void LoadSettings()
        {
            ShowEnemyESP = PlayerPrefs.GetInt("EnemyESP", 1) == 1;
            MaxESPDistance = PlayerPrefs.GetFloat("MaxESPDistance", 100f);
        }
        
        public static void SaveSettings()
        {
            PlayerPrefs.SetInt("EnemyESP", ShowEnemyESP ? 1 : 0);
            PlayerPrefs.SetFloat("MaxESPDistance", MaxESPDistance);
            PlayerPrefs.Save();
        }
    }
}
