using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game_7D2D
{
    /// <summary>
    /// Main controller class for the ESP system.
    /// Handles entity scanning, caching, and rendering coordination.
    /// </summary>
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

        /// <summary>
        /// Initializes the ESP system and loads configuration.
        /// </summary>
        public void Start()
        {
            // Load configuration
            ESPConfig.LoadConfig();
        }

        /// <summary>
        /// Main update loop. Handles input, entity scanning, and state management.
        /// </summary>
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

        /// <summary>
        /// GUI rendering loop. Renders menu and ESP visualizations.
        /// </summary>
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
        
        /// <summary>
        /// Scans for entities and caches them with distance filtering.
        /// . Called at fixed.
        /// . </summary>
        private static void ScanEntities()
        {
            cachedEntities.Clear();
            
            if (Hacks.eLocalPlayer == null) return;
            
            Vector3 cameraPos = Hacks.eLocalPlayer.transform.position;
            
            try
            {
                // Use dedicated EntityScanner module
                cachedEntities = EntityScanner.ScanEnemies(cameraPos, ESPConfig.MaxESPDistance);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Hacks] Error in ScanEntities: {ex.Message}");
            }
        }
        
        }
        
    }

}
