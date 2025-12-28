using UnityEngine;

namespace Game_7D2D
{
    public class Loader
    {
        public static void init()
        {
            // Create the main GameObject for our hack
            GameObject hackObject = new GameObject("ESP_Hack");
            
            // Add the Hacks component which contains all the logic
            hackObject.AddComponent<Hacks>();
            
            // Make sure the object persists between scene loads
            UnityEngine.Object.DontDestroyOnLoad(hackObject);
            
            Debug.Log("7D2D ESP Hack Loaded Successfully");
        }
    }
}
