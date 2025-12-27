using UnityEngine;
using Game_7D2D.Modules;

namespace Game_7D2D
{
    public class Loader
    {
        public static void init()
        {
            Loader.Load = new GameObject();
            Loader.Load.AddComponent<HacksManager>();
            UnityEngine.Object.DontDestroyOnLoad(Loader.Load);
        }

        public static void unload()
        {
            _unload();
        }

        private static void _unload()
        {
            var hacksManager = Load.GetComponent<HacksManager>();
            if (hacksManager != null)
            {
                hacksManager.Cleanup();
            }
            GameObject.Destroy(Load);
        }
        
        private static GameObject Load;
    }
}
