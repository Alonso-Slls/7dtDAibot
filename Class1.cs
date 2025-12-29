using UnityEngine;

namespace Game_7D2D
{
    public class Loader
    {
        public static void init()
        {
            Debug.Log("[7dtDAibot] Loading enhanced ESP framework...");
            
            // Create main manager with enhanced ESP
            var go = new GameObject("EnhancedESPManager");
            go.AddComponent<SevenDtDAibot.EnhancedESPManager>();
            Object.DontDestroyOnLoad(go);
            
            Debug.Log("[7dtDAibot] Enhanced ESP framework loaded");
        }
    }
}
