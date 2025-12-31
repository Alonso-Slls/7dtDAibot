using UnityEngine;

namespace Modules
{
    public class UI
    {
        private static readonly int MENU_WIDTH = 280;
        private static readonly int MENU_HEIGHT = 220;
        private static readonly int MENU_X = 50;
        private static readonly int MENU_Y = 50;
        
        // UI Colors
        private static readonly Color BG_COLOR = new Color(0.05f, 0.05f, 0.1f, 0.95f);
        private static readonly Color BORDER_COLOR = new Color(0.2f, 0.6f, 1.0f, 1.0f);
        private static readonly Color TITLE_COLOR = new Color(0.0f, 0.8f, 1.0f, 1.0f);
        private static readonly Color SLIDER_BG = new Color(0.3f, 0.3f, 0.4f, 0.8f);
        private static readonly Color SLIDER_FILL = new Color(0.0f, 0.8f, 1.0f, 0.9f);
        
        // Slider properties
        private static readonly int SLIDER_WIDTH = 180;
        private static readonly int SLIDER_HEIGHT = 8;
        private static readonly int SLIDER_X_OFFSET = 10;
        private static readonly int SLIDER_Y_OFFSET = 80;
        private static bool isDraggingSlider = false;
        
        public static void DrawMenu()
        {
            if (!Hacks.showMenu) return;
            
            // Draw enhanced menu background with gradient effect
            GUI.color = BG_COLOR;
            GUI.DrawTexture(new Rect(MENU_X, MENU_Y, MENU_WIDTH, MENU_HEIGHT), Texture2D.whiteTexture);
            
            // Draw enhanced border with glow effect
            GUI.color = BORDER_COLOR;
            GUI.DrawTexture(new Rect(MENU_X, MENU_Y, MENU_WIDTH, 3), Texture2D.whiteTexture); // Top
            GUI.DrawTexture(new Rect(MENU_X, MENU_Y + MENU_HEIGHT - 3, MENU_WIDTH, 3), Texture2D.whiteTexture); // Bottom
            GUI.DrawTexture(new Rect(MENU_X, MENU_Y, 3, MENU_HEIGHT), Texture2D.whiteTexture); // Left
            GUI.DrawTexture(new Rect(MENU_X + MENU_WIDTH - 3, MENU_Y, 3, MENU_HEIGHT), Texture2D.whiteTexture); // Right
            
            // Draw inner border for depth
            GUI.color = new Color(BORDER_COLOR.r * 0.5f, BORDER_COLOR.g * 0.5f, BORDER_COLOR.b * 0.5f, 0.5f);
            GUI.DrawTexture(new Rect(MENU_X + 3, MENU_Y + 3, MENU_WIDTH - 6, 1), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(MENU_X + 3, MENU_Y + MENU_HEIGHT - 4, MENU_WIDTH - 6, 1), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(MENU_X + 3, MENU_Y + 3, 1, MENU_HEIGHT - 6), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(MENU_X + MENU_WIDTH - 4, MENU_Y + 3, 1, MENU_HEIGHT - 6), Texture2D.whiteTexture);
            
            // Draw enhanced title with shadow
            GUI.color = Color.black;
            Render.DrawString(MENU_X + 11, MENU_Y + 11, "7D2D ESP Menu", Color.black);
            GUI.color = TITLE_COLOR;
            Render.DrawString(MENU_X + 10, MENU_Y + 10, "7D2D ESP Menu", TITLE_COLOR);
            
            // Draw enhanced menu options with better spacing
            GUI.color = Color.white;
            
            // Enemy ESP toggle with enhanced styling
            string espStatus = SevenDtDAibot.ESPSettings.ShowEnemyESP ? "[ON]" : "[OFF]";
            Color espColor = SevenDtDAibot.ESPSettings.ShowEnemyESP ? Color.green : Color.red;
            Render.DrawString(MENU_X + 10, MENU_Y + 40, $"● Enemy ESP {espStatus}", espColor);
            
            // Render Distance slider
            GUI.color = Color.white;
            Render.DrawString(MENU_X + 10, MENU_Y + 65, $"Render Distance: {SevenDtDAibot.ESPSettings.MaxESPDistance:F0}m", Color.white);
            
            // Draw slider background
            GUI.color = SLIDER_BG;
            GUI.DrawTexture(new Rect(MENU_X + SLIDER_X_OFFSET, MENU_Y + SLIDER_Y_OFFSET, SLIDER_WIDTH, SLIDER_HEIGHT), Texture2D.whiteTexture);
            
            // Draw slider fill based on current distance
            float distancePercent = (SevenDtDAibot.ESPSettings.MaxESPDistance - 50f) / 250f; // 50-300m range
            distancePercent = Mathf.Clamp01(distancePercent);
            GUI.color = SLIDER_FILL;
            GUI.DrawTexture(new Rect(MENU_X + SLIDER_X_OFFSET, MENU_Y + SLIDER_Y_OFFSET, SLIDER_WIDTH * distancePercent, SLIDER_HEIGHT), Texture2D.whiteTexture);
            
            // Draw slider handle
            float handleX = MENU_X + SLIDER_X_OFFSET + (SLIDER_WIDTH * distancePercent);
            GUI.color = Color.white;
            GUI.DrawTexture(new Rect(handleX - 4, MENU_Y + SLIDER_Y_OFFSET - 2, 8, SLIDER_HEIGHT + 4), Texture2D.whiteTexture);
            
            // Instructions with better styling
            GUI.color = new Color(1.0f, 0.8f, 0.0f, 1.0f); // Bright yellow
            Render.DrawString(MENU_X + 10, MENU_Y + 110, "F1: Toggle ESP", new Color(1.0f, 0.8f, 0.0f, 1.0f));
            Render.DrawString(MENU_X + 10, MENU_Y + 130, "F3: Force Update", new Color(1.0f, 0.8f, 0.0f, 1.0f));
            Render.DrawString(MENU_X + 10, MENU_Y + 150, "END: Unload Hack", new Color(1.0f, 0.8f, 0.0f, 1.0f));
            
            // Entity count with enhanced styling
            GUI.color = new Color(0.0f, 1.0f, 0.5f, 1.0f); // Bright green
            Render.DrawString(MENU_X + 10, MENU_Y + 175, $"Enemies: {Hacks.eEnemy.Count}", new Color(0.0f, 1.0f, 0.5f, 1.0f));
            
            // Performance info
            GUI.color = new Color(0.7f, 0.7f, 0.7f, 1.0f);
            Render.DrawString(MENU_X + 10, MENU_Y + 195, $"FPS: {(1.0f / Time.deltaTime):F0}", new Color(0.7f, 0.7f, 0.7f, 1.0f));
            
            // Handle mouse interactions
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Vector2 mousePos = Event.current.mousePosition;
                
                // Check Enemy ESP toggle click
                if (mousePos.x >= MENU_X + 10 && mousePos.x <= MENU_X + 150 &&
                    mousePos.y >= MENU_Y + 40 && mousePos.y <= MENU_Y + 55)
                {
                    SevenDtDAibot.ESPSettings.ShowEnemyESP = !SevenDtDAibot.ESPSettings.ShowEnemyESP;
                    SevenDtDAibot.ESPSettings.SaveSettings();
                    Event.current.Use();
                }
                
                // Check slider click
                if (mousePos.x >= MENU_X + SLIDER_X_OFFSET && mousePos.x <= MENU_X + SLIDER_X_OFFSET + SLIDER_WIDTH &&
                    mousePos.y >= MENU_Y + SLIDER_Y_OFFSET - 5 && mousePos.y <= MENU_Y + SLIDER_Y_OFFSET + SLIDER_HEIGHT + 5)
                {
                    isDraggingSlider = true;
                    UpdateSliderFromMouse(mousePos);
                    Event.current.Use();
                }
            }
            else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                if (isDraggingSlider)
                {
                    isDraggingSlider = false;
                    SevenDtDAibot.ESPSettings.SaveSettings();
                    Event.current.Use();
                }
            }
            else if (Event.current.type == EventType.MouseDrag && isDraggingSlider)
            {
                UpdateSliderFromMouse(Event.current.mousePosition);
                Event.current.Use();
            }
        }
        
        private static void UpdateSliderFromMouse(Vector2 mousePos)
        {
            // Calculate slider value from mouse position
            float sliderX = mousePos.x - (MENU_X + SLIDER_X_OFFSET);
            float percent = Mathf.Clamp01(sliderX / SLIDER_WIDTH);
            
            // Convert percent to distance (50-300m range)
            float newDistance = 50f + (percent * 250f);
            SevenDtDAibot.ESPSettings.MaxESPDistance = newDistance;
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
