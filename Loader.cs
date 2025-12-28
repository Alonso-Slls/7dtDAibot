using UnityEngine;

namespace SevenDtDAibot
{
    /// <summary>
    /// Main entry point for the mod.
    /// </summary>
    public class Loader
    {
        public static void init()
        {
            Debug.Log("[7dtDAibot] Loading basic ESP framework...");
            
            // Create main manager
            var go = new GameObject("ESPManager");
            go.AddComponent<ESPManager>();
            Object.DontDestroyOnLoad(go);
            
            Debug.Log("[7dtDAibot] Basic ESP framework loaded");
        }
    }
}
