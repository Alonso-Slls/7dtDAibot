using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game_7D2D.Modules
{
    class UI
    {
        //******* ESP Toggle Variables ********
        public static bool t_EnemyESP = false;
        public static bool t_EnemyBones = false;
        public static bool t_ESPBoxes = true;

        private static bool toggleEnemy = false;
        private static bool toggleEnemyBones = false;
        private static bool toggleESPBoxes = false;


        public static string dbg = "debug";
        public static void DrawMenu()
        {
            if (Hacks.Menu && Hacks.isLoaded)
            {
                GUI.Box(new Rect(5f, 5f, 250f, 85f), "");
                GUI.Label(new Rect(10f, 5f, 250f, 30f), "\x37\x44\x61\x79\x73\x32\x44\x69\x65\x20\x2d\x20\x45\x53\x50\x20\x4d\x6f\x64");

                toggleEnemy = GUI.Toggle(new Rect(10f, 30f, 250f, 25f), t_EnemyESP, "Enemy ESP");
                if (toggleEnemy != t_EnemyESP)
                {
                    t_EnemyESP = !t_EnemyESP;
                }
                
                toggleEnemyBones = GUI.Toggle(new Rect(10f, 55f, 250f, 25f), t_EnemyBones, "Enemy Bones");
                if (toggleEnemyBones != t_EnemyBones)
                {
                    t_EnemyBones = !t_EnemyBones;
                }
            }
        }
    }
}
