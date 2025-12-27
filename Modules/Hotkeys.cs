using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SevenDtDAibot.Modules
{
    class Hotkeys
    {
        public static void hotkeys()
        {
            
            if (Input.GetKeyDown(KeyCode.End)) // Kill hacks on "End" key pressed
            {
                Loader.unload();
            }
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                UI.Menu = !UI.Menu;
            }
            // Run aimbot if AAIM is active and the player is holding Right-Click (legacy behavior)
            if (Input.GetKey(KeyCode.Mouse1) && Config.Settings.AimbotEnabled)
            {
                Aimbot.AimAssist();
            }
        }
    }
}
