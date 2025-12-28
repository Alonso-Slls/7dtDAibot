using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game_7D2D.Modules
{
    class Hotkeys
    {
        public static void hotkeys()
        {
            
            if (Input.GetKeyDown(ESPConfig.UnloadKey)) // Kill hacks on unload key pressed
            {
                Loader.unload();
            }
            if (Input.GetKeyDown(ESPConfig.ToggleMenuKey))
            {
                Hacks.Menu = !Hacks.Menu;
            }
        }
    }
}
