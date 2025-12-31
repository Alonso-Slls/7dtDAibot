using UnityEngine;

namespace Game_7D2D
{
    public class Loader
    {
        public static void init()
        {
            Debug.Log("[7dtDAibot] Loading ESP framework...");
            
            // Create main GameObject with Hacks component (the working system)
            var go = new GameObject("ESP_Hack");
            go.AddComponent<Hacks>();
            Object.DontDestroyOnLoad(go);
            
            Debug.Log("[7dtDAibot] ESP framework loaded successfully");
        }
    }
}
