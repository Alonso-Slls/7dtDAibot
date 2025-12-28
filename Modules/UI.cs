using UnityEngine;

namespace Modules
{
    public class UI
    {
        private static readonly int MENU_WIDTH = 200;
        private static readonly int MENU_HEIGHT = 150;
        private static readonly int MENU_X = 50;
        private static readonly int MENU_Y = 50;
        
        public static void DrawMenu()
        {
            if (!Hacks.showMenu) return;
            
            // Draw menu background
            GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            GUI.DrawTexture(new Rect(MENU_X, MENU_Y, MENU_WIDTH, MENU_HEIGHT), Texture2D.whiteTexture);
            
            // Draw menu border
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(MENU_X, MENU_Y, MENU_WIDTH, 2), Texture2D.whiteTexture); // Top
            GUI.DrawTexture(new Rect(MENU_X, MENU_Y + MENU_HEIGHT - 2, MENU_WIDTH, 2), Texture2D.whiteTexture); // Bottom
            GUI.DrawTexture(new Rect(MENU_X, MENU_Y, 2, MENU_HEIGHT), Texture2D.whiteTexture); // Left
            GUI.DrawTexture(new Rect(MENU_X + MENU_WIDTH - 2, MENU_Y, 2, MENU_HEIGHT), Texture2D.whiteTexture); // Right
            
            // Draw title
            GUI.color = Color.cyan;
            Render.DrawString(MENU_X + 10, MENU_Y + 10, "7D2D ESP Menu", Color.cyan);
            
            // Draw menu options
            GUI.color = Color.white;
            
            // Enemy ESP toggle
            string espStatus = Hacks.enemyESP ? "[ON]" : "[OFF]";
            Color espColor = Hacks.enemyESP ? Color.green : Color.red;
            Render.DrawString(MENU_X + 10, MENU_Y + 40, $"Enemy ESP {espStatus}", espColor);
            
            // Enemy Bones toggle
            string bonesStatus = Hacks.enemyBones ? "[ON]" : "[OFF]";
            Color bonesColor = Hacks.enemyBones ? Color.green : Color.red;
            Render.DrawString(MENU_X + 10, MENU_Y + 60, $"Enemy Bones {bonesStatus}", bonesColor);
            
            // Instructions
            GUI.color = Color.yellow;
            Render.DrawString(MENU_X + 10, MENU_Y + 90, "F1: Toggle Menu", Color.yellow);
            Render.DrawString(MENU_X + 10, MENU_Y + 110, "END: Unload Hack", Color.yellow);
            
            // Entity count
            GUI.color = Color.white;
            Render.DrawString(MENU_X + 10, MENU_Y + 130, $"Enemies: {Hacks.eEnemy.Count}", Color.white);
            
            // Handle mouse clicks for toggles
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector2 mousePos = Event.current.mousePosition;
                
                // Check Enemy ESP toggle click
                if (mousePos.x >= MENU_X + 10 && mousePos.x <= MENU_X + 150 &&
                    mousePos.y >= MENU_Y + 40 && mousePos.y <= MENU_Y + 55)
                {
                    Hacks.enemyESP = !Hacks.enemyESP;
                    Event.current.Use();
                }
                
                // Check Enemy Bones toggle click
                if (mousePos.x >= MENU_X + 10 && mousePos.x <= MENU_X + 150 &&
                    mousePos.y >= MENU_Y + 60 && mousePos.y <= MENU_Y + 75)
                {
                    Hacks.enemyBones = !Hacks.enemyBones;
                    Event.current.Use();
                }
            }
        }
        
        public static void DrawWatermark()
        {
            if (!Hacks.showMenu) return;
            
            // Draw watermark in bottom right corner
            string watermark = "7D2D ESP v1.0";
            Vector2 watermarkSize = GUI.skin.label.CalcSize(new GUIContent(watermark));
            
            float x = Screen.width - watermarkSize.x - 10;
            float y = Screen.height - watermarkSize.y - 10;
            
            GUI.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
            Render.DrawString(x, y, watermark, Color.gray);
        }
    }
}
