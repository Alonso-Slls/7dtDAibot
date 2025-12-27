using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SevenDtDAibot.Modules
{
    // Legacy compatibility class for backward compatibility
    public static class Hacks
    {
        public static bool Menu = false;
        public static bool isLoaded = false;
        public static EntityPlayer eLocalPlayer = null;
        public static Camera MainCamera = null;
        
        // Entity collections for backward compatibility
        public static List<EntityAnimal> eAnimal = new List<EntityAnimal>();
        public static List<EntityPlayer> ePlayers = new List<EntityPlayer>();
        public static List<EntityEnemy> eEnemy = new List<EntityEnemy>();
        
        public static Vector3 W2S(Vector3 worldPos)
        {
            if (MainCamera == null) return Vector3.zero;
            return MainCamera.WorldToScreenPoint(worldPos);
        }
        
        // Legacy compatibility methods
        public static void Initialize()
        {
            isLoaded = true;
        }
        
        public static void Cleanup()
        {
            isLoaded = false;
        }
    }
}
