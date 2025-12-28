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

        //Menu Variables
        public static bool Menu = true;

        public static bool isLoaded = GameManager.Instance.gameStateManager.IsGameStarted();

        public void Start()
        {
            
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
                foreach (EntityEnemy Enemy in eEnemy)
                {
                    if (Enemy != null && Enemy.IsAlive())
                    {
                        Modules.ESP.esp_drawBox(Enemy, Color.red);
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

        
    }

}
