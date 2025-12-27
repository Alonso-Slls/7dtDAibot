using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SevenDtDAibot.Modules
{
    /// <summary>
    /// Modernized UI system using Unity's IMGUI with improved styling and layout.
    /// Features responsive design, better visual hierarchy, and enhanced user experience.
    /// </summary>
    public static class UI
    {
        public static bool Menu = false;
        //******* ESP Toggle Variables ********










        //******* Aimbot Toggle Variables ********







        
        // Aim tuning





        // FOV-Aware ESP Settings


        
        // Legacy compatibility
        public static string dbg = "debug";

        // UI Style and Layout Configuration
        private static GUIStyle windowStyle;
        private static GUIStyle buttonStyle;
        private static GUIStyle labelStyle;
        private static GUIStyle toggleStyle;
        private static GUIStyle sliderStyle;
        private static GUIStyle boxStyle;
        
        // Layout Constants
        private const float WINDOW_WIDTH = Config.WINDOW_WIDTH;
        private const float WINDOW_HEIGHT = Config.WINDOW_HEIGHT;
        private const float PADDING = Config.PADDING;
        private const float BUTTON_HEIGHT = Config.BUTTON_HEIGHT;
        private const float SPACING = Config.SPACING;
        
        // Colors
        private static Color primaryColor = Config.PrimaryColor;
        private static Color accentColor = Config.AccentColor;
        private static Color textColor = Config.TextColor;
        private static Color disabledColor = Config.DisabledColor;
        
        // Animation and Interpolation
        private static float menuAlpha = 0f;
        private static bool menuVisible = false;
        private const float FADE_SPEED = Config.FADE_SPEED;

        /// <summary>
        /// Initialize UI styles and colors for modern appearance.
        /// </summary>
        private static void InitializeStyles()
        {
            if (windowStyle != null) return; // Already initialized
            
            // Window Style
            windowStyle = new GUIStyle(GUI.skin.window)
            {
                normal = { background = CreateColorTexture(primaryColor) },
                fontSize = Config.WINDOW_FONT_SIZE,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };
            
            // Button Style
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                normal = { textColor = textColor },
                hover = { textColor = Color.white },
                active = { textColor = accentColor },
                fontSize = Config.BUTTON_FONT_SIZE,
                fontStyle = FontStyle.Bold
            };
            
            // Label Style
            labelStyle = new GUIStyle(GUI.skin.label)
            {
                normal = { textColor = textColor },
                fontSize = Config.LABEL_FONT_SIZE,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.UpperLeft
            };
            
            // Toggle Style
            toggleStyle = new GUIStyle(GUI.skin.toggle)
            {
                normal = { textColor = textColor },
                fontSize = Config.TOGGLE_FONT_SIZE,
                fontStyle = FontStyle.Normal
            };
            
            // Box Style
            boxStyle = new GUIStyle(GUI.skin.box)
            {
                normal = { background = CreateColorTexture(Config.BoxBackgroundColor) }
            };
        }
        
        /// <summary>
        /// Create a simple colored texture for UI elements.
        /// </summary>
        private static Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(Config.TEXTURE_SIZE, Config.TEXTURE_SIZE);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
        
        /// <summary>
        /// Main menu drawing method with modern IMGUI implementation.
        /// </summary>
        public static void DrawMenu()
        {
            if (!Menu || (HacksManager.Instance == null || !HacksManager.Instance.IsLoaded)) return;
            
            InitializeStyles();
            
            // Handle menu fade animation
            if (Menu && !menuVisible)
            {
                menuVisible = true;
            }
            else if (!Menu && menuVisible)
            {
                menuVisible = false;
            }
            
            // Smooth fade in/out
            if (menuVisible && menuAlpha < 1f)
                menuAlpha = Mathf.Min(1f, menuAlpha + Time.deltaTime * Config.FADE_SPEED);
            else if (!menuVisible && menuAlpha > 0f)
                menuAlpha = Mathf.Max(0f, menuAlpha - Time.deltaTime * Config.FADE_SPEED);
            
            if (menuAlpha <= 0f) return;
            
            GUI.color = new Color(1f, 1f, 1f, menuAlpha);
            
            // Main Menu Window
            GUILayout.BeginArea(new Rect(Config.PADDING, Config.PADDING, Config.WINDOW_WIDTH, Config.WINDOW_HEIGHT), windowStyle);
            DrawMainMenu();
            GUILayout.EndArea();
            
            // ESP Submenu
            if (Config.Settings.ESPEnabled)
            {
                GUILayout.BeginArea(new Rect(Config.WINDOW_WIDTH + Config.PADDING * 2, Config.PADDING, Config.SUBMENU_WIDTH, Config.ESP_SUBMENU_HEIGHT), windowStyle);
                DrawESPMenu();
                GUILayout.EndArea();
            }
            
            // Aimbot Submenu
            if (Config.Settings.AimbotEnabled)
            {
                GUILayout.BeginArea(new Rect(Config.WINDOW_WIDTH + Config.PADDING * 2, Config.PADDING, Config.SUBMENU_WIDTH, Config.AIMBOT_SUBMENU_HEIGHT), windowStyle);
                DrawAimbotMenu();
                GUILayout.EndArea();
            }
            
            // Debug Overlay
            if (Config.Settings.DebugOverlay)
            {
                DrawDebugOverlay();
            }
            
            GUI.color = Color.white;
        }
        
        /// <summary>
        /// Draw the main menu with toggle options.
        /// </summary>
        private static void DrawMainMenu()
        {
            GUILayout.Label("7Days2Die - Gh0st Mod Menu", labelStyle);
            GUILayout.Space(SPACING);
            
            // ESP Toggle
            GUI.color = Config.Settings.ESPEnabled ? accentColor : Color.white;
            if (GUILayout.Button("ESP System [ON/OFF]", buttonStyle, GUILayout.Height(BUTTON_HEIGHT)))
            {
                Config.Settings.ESPEnabled = !Config.Settings.ESPEnabled;
                ; // Exclusive menus
            }
            
            // Aimbot Toggle
            GUI.color = Config.Settings.AimbotEnabled ? accentColor : Color.white;
            if (GUILayout.Button("Aimbot System [ON/OFF]", buttonStyle, GUILayout.Height(BUTTON_HEIGHT)))
            {
                Config.Settings.AimbotEnabled = !Config.Settings.AimbotEnabled;
                ; // Exclusive menus
            }
            
            GUI.color = Color.white;
            GUILayout.Space(SPACING * 2);
            
            // Quick Status
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Status:", labelStyle);
            GUILayout.Label($"ESP: {(Config.Settings.ESPEnabled ? "Active" : "Inactive")}", labelStyle);
            GUILayout.Label($"Aimbot: {(Config.Settings.AimbotEnabled ? "Active" : "Inactive")}", labelStyle);
            GUILayout.Label($"FOV-Aware: {(Config.Settings.FOVAwareESP ? "Enabled" : "Disabled")}", labelStyle);
            GUILayout.EndVertical();
            
            GUILayout.Space(SPACING);
            
            // Debug info
            if (Config.Settings.DebugOverlay)
            {
                GUILayout.BeginVertical(boxStyle);
                GUILayout.Label("Debug Info:", labelStyle);
                GUILayout.Label($"FPS: {(int)(1f / Time.deltaTime)}", labelStyle);
                GUILayout.Label($"Entities: {GetTotalEntityCount()}", labelStyle);
                GUILayout.Label(ESPPool.GetStats(), labelStyle);
                GUILayout.Label(SpatialGrid.GetStats(), labelStyle);
                GUILayout.EndVertical();
            }
        }
        
        /// <summary>
        /// Draw the ESP configuration menu.
        /// </summary>
        private static void DrawESPMenu()
        {
            GUILayout.Label("ESP Configuration", labelStyle);
            GUILayout.Space(SPACING);
            
            // Main ESP Toggle
            Config.Settings.ESPEnabled = GUILayout.Toggle(Config.Settings.ESPEnabled, "Enable ESP", toggleStyle);
            GUILayout.Space(SPACING);
            
            // ESP Options - Two columns
            GUILayout.BeginHorizontal();
            
            // Left Column
            GUILayout.BeginVertical();
            Config.Settings.EnemyESP = GUILayout.Toggle(Config.Settings.EnemyESP, "Enemy ESP", toggleStyle);
            Config.Settings.ItemESP = GUILayout.Toggle(Config.Settings.ItemESP, "Item ESP", toggleStyle);
            Config.Settings.NPCESP = GUILayout.Toggle(Config.Settings.NPCESP, "NPC ESP", toggleStyle);
            Config.Settings.PlayerESP = GUILayout.Toggle(Config.Settings.PlayerESP, "Player ESP", toggleStyle);
            Config.Settings.AnimalESP = GUILayout.Toggle(Config.Settings.AnimalESP, "Animal ESP", toggleStyle);
            GUILayout.EndVertical();
            
            GUILayout.Space(SPACING);
            
            // Right Column
            GUILayout.BeginVertical();
            Config.Settings.EnemyBones = GUILayout.Toggle(Config.Settings.EnemyBones, "Enemy Bones", toggleStyle);
            Config.Settings.ESPLines = GUILayout.Toggle(Config.Settings.ESPLines, "Draw Lines", toggleStyle);
            Config.Settings.ESPBoxes = GUILayout.Toggle(Config.Settings.ESPBoxes, "Draw Boxes", toggleStyle);
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(SPACING * 2);
            
            // FOV-Aware Settings
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("FOV-Aware Settings:", labelStyle);
            Config.Settings.FOVAwareESP = GUILayout.Toggle(Config.Settings.FOVAwareESP, "Enable FOV-Aware ESP", toggleStyle);
            
            if (Config.Settings.FOVAwareESP)
            {
                GUILayout.Label($"FOV Threshold: {Config.Settings.FOVThreshold:F0}°", labelStyle);
                Config.Settings.FOVThreshold = GUILayout.HorizontalSlider(Config.Settings.FOVThreshold, 60f, 180f);
            }
            GUILayout.EndVertical();
        }
        
        /// <summary>
        /// Draw the Aimbot configuration menu.
        /// </summary>
        private static void DrawAimbotMenu()
        {
            GUILayout.Label("Aimbot Configuration", labelStyle);
            GUILayout.Space(SPACING);
            
            // Aimbot Controls
            Config.Settings.AimbotEnabled = GUILayout.Toggle(Config.Settings.AimbotEnabled, "Activate Aimbot", toggleStyle);
            GUILayout.Space(SPACING);
            
            // Target Selection
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Target Selection:", labelStyle);
            Config.Settings.TargetEnemies = GUILayout.Toggle(Config.Settings.TargetEnemies, "Target Enemies", toggleStyle);
            Config.Settings.TargetAnimals = GUILayout.Toggle(Config.Settings.TargetAnimals, "Target Animals", toggleStyle);
            Config.Settings.TargetPlayers = GUILayout.Toggle(Config.Settings.TargetPlayers, "Target Players", toggleStyle);
            Config.Settings.ShowFOVCircle = GUILayout.Toggle(Config.Settings.ShowFOVCircle, "Show FOV Circle", toggleStyle);
            GUILayout.EndVertical();
            
            GUILayout.Space(SPACING);
            
            // Aim Settings
            GUILayout.BeginVertical(boxStyle);
            GUILayout.Label("Aim Settings:", labelStyle);
            
            GUILayout.Label($"Aim FOV: {Config.Settings.AimFOV}", labelStyle);
            Config.Settings.AimFOV = (int)GUILayout.HorizontalSlider(Config.Settings.AimFOV, 50f, 400f);
            
            GUILayout.Label($"Aim Smooth: {Config.Settings.AimSmooth:F1}", labelStyle);
            Config.Settings.AimSmooth = GUILayout.HorizontalSlider(Config.Settings.AimSmooth, 1f, 20f);
            
            Config.Settings.AimbotRaw = GUILayout.Toggle(Config.Settings.AimbotRaw, "Raw Aiming (Direct Cursor)", toggleStyle);
            GUILayout.EndVertical();
            
            GUILayout.Space(SPACING);
            
            // Debug Options
            Config.Settings.DebugOverlay = GUILayout.Toggle(Config.Settings.DebugOverlay, "Debug Overlay", toggleStyle);
        }
        
        /// <summary>
        /// Draw debug overlay information.
        /// </summary>
        private static void DrawDebugOverlay()
        {
            GUILayout.BeginArea(new Rect(Screen.width - 200f, PADDING, 190f, 100f), boxStyle);
            GUILayout.Label("Debug Overlay", labelStyle);
            GUILayout.Label($"FPS: {(int)(1f / Time.deltaTime)}", labelStyle);
            GUILayout.Label($"Menu Alpha: {menuAlpha:F2}", labelStyle);
            GUILayout.Label($"Time: {DateTime.Now:HH:mm:ss}", labelStyle);
            GUILayout.EndArea();
        }
        
        /// <summary>
        /// Get total entity count for debug display.
        /// </summary>
        private static int GetTotalEntityCount()
        {
            int count = 0;
            try
            {
                // Use legacy Hacks collections for now
                count += EntityTracker<EntityAnimal>.Instance.GetAllEntities().Length;
                count += EntityTracker<EntityPlayer>.Instance.GetAllEntities().Length;
                count += EntityTracker<EntityEnemy>.Instance.GetAllEntities().Length;
            }
            catch
            {
                count = -1; // Error indicator
            }
            return count;
        }
        
        /// <summary>
        /// Initialize method to initialize resources.
        /// </summary>
        public static void Initialize()
        {
            // Initialize UI system
        }
        
        /// <summary>
        /// Cleanup method to release resources.
        /// </summary>
        public static void Cleanup()
        {
            if (windowStyle?.normal?.background != null)
                UnityEngine.Object.Destroy(windowStyle.normal.background);
            if (boxStyle?.normal?.background != null)
                UnityEngine.Object.Destroy(boxStyle.normal.background);
        }
    }
}
