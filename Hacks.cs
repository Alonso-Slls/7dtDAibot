using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game_7D2D
{
    class Hacks : MonoBehaviour
    {
        public static Camera MainCamera = null;
        public static float Timer = 0f;

        //Entities
        public static List<EntityEnemy> eEnemy = new List<EntityEnemy>();
        public static LocalPlayer localP;
        public static EntityPlayerLocal eLocalPlayer;
        
        // Entity caching for performance
        private static List<EntityData> cachedEntities = new List<EntityData>();
        private static float lastScanTime = 0f;
        
        // Reusable objects to reduce GC
        public static Rect boxRect = new Rect();
        public static Rect labelRect = new Rect();

        //Menu Variables
        public static bool Menu = true;

        public static bool isLoaded = GameManager.Instance.gameStateManager.IsGameStarted();

        public void Start()
        {
            // Load configuration
            ESPConfig.LoadConfig();
        }

        public void Update()
        {
            if (isLoaded)
            {
                Modules.Hotkeys.hotkeys();
                Timer += Time.deltaTime;

                if (Timer >= 5f)
                {
                    Timer = 0f;
                    updateObjects();
                }
                
                // Scan entities at fixed interval for performance
                if (Modules.UI.t_EnemyESP && Time.time - lastScanTime > ESPConfig.EntityScanInterval)
                {
                    ScanEntities();
                    lastScanTime = Time.time;
                }
            }
            
            checkState();
        }

        public void OnGUI()
        {
            if (!isLoaded)
            {
                GUI.Box(new Rect(5f, 5f, 250f, 35f), "");
                GUI.Label(new Rect(10f, 5f, 250f, 30f), "Menu will load when in a game");
                return;
            }

            Modules.UI.DrawMenu();

            if (Modules.UI.t_EnemyESP)
            {
                // Render cached entities instead of scanning every frame
                foreach (var entityData in cachedEntities)
                {
                    if (entityData.Entity != null && entityData.Entity.IsAlive())
                    {
                        Modules.ESP.esp_drawBox(entityData.Entity, entityData.Color);
                    }
                }
            }

            

        }
        private void checkState()
        {
            if (isLoaded != GameManager.Instance.gameStateManager.IsGameStarted())
            {
                isLoaded = !isLoaded;
            }
        }

        public static void updateObjects()
        {
            if (Modules.UI.t_EnemyESP)
            {
                Hacks.eEnemy = UnityEngine.GameObject.FindObjectsOfType<EntityEnemy>().Where<EntityEnemy>(s => s.IsAlive()).ToList();
            }
            Hacks.localP = UnityEngine.GameObject.FindObjectOfType<LocalPlayer>();
            Hacks.eLocalPlayer = UnityEngine.GameObject.FindObjectOfType<EntityPlayerLocal>();
        }
        
        private static void ScanEntities()
        {
            cachedEntities.Clear();
            
            if (Hacks.eLocalPlayer == null) return;
            
            Vector3 cameraPos = Hacks.eLocalPlayer.transform.position;
            
            try
            {
                var enemies = UnityEngine.GameObject.FindObjectsOfType<EntityEnemy>();
                if (enemies != null)
                {
                    foreach (var enemy in enemies)
                    {
                        if (enemy == null || !enemy.IsAlive()) continue;
                        
                        float distance = Vector3.Distance(cameraPos, enemy.transform.position);
                        
                        // Distance culling
                        if (distance > ESPConfig.MaxESPDistance) continue;
                        
                        cachedEntities.Add(new EntityData
                        {
                            Entity = enemy,
                            Color = ESPConfig.EnemyColor,
                            Label = "Enemy",
                            Position = enemy.transform.position,
                            Distance = distance
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Log error if needed
            }
        }
        
        // Helper class to cache entity data
        private class EntityData
        {
            public EntityEnemy Entity;
            public Color Color;
            public string Label;
            public Vector3 Position;
            public float Distance;
        }

        
    }

}
