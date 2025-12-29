UI Improvement Analysis and Recommendations for 7 Days to Die AIBOT Project

This report provides a comprehensive analysis of the user interface (UI) implementation in the *7 Days to Die AIBot* mod project hosted at [https://github.com/Alonso-Slls/7dtDAibot](https://github.com/Alonso-Slls/7dtDAibot). The goal is to enhance usability, performance, scalability, and maintainability of the UI system within this Unity-based ESP (Extra-Sensory Perception) mod.


The analysis draws directly from source code inspection, project documentation, and established best practices in Unity UI development for game mods. All recommendations are prioritized to maximize impact with minimal risk, and beginner-friendly resources are included to support implementation.


⸻


1. Initial Assessment

The current UI system of the 7dtDAibot mod demonstrates a clear focus on functionality and performance optimization, leveraging Unity’s legacy `OnGUI` system for rendering overlays directly on screen. This is consistent with common practices in stealth modding, where minimal overhead and compatibility are essential[1].


Strengths
• **Performance-Oriented Design**: The mod uses a lightweight drawing utility class (`Render.cs`) that creates only a single 1x1 white texture for all shapes (`DrawBox`, `DrawLine`, `DrawString`, etc.), minimizing GPU texture memory usage and draw calls during render[2].

• **Clear Separation of Concerns**: Code is logically split into modular files:
- `Render.cs`: Handles low-level drawing primitives.
- `UI.cs`: Manages menu layout and interaction.
- `ESP.cs`: Implements entity visualization logic.
- `Hacks.cs`: Acts as the main game loop controller.
These files are well-contained and follow a functional decomposition pattern[5][4].

• **Simplified UX for Core Purpose**: The UI menu is minimalist, showing only essential toggles (Enemy ESP, Enemy Bones) and utility info (entity count, hotkey hints), which aligns with the project's goal of being a "simplified" ESP mod[1][4].

• **Efficient Entity Scanning**: The use of `GameObject.FindObjectsOfType<T>()` in `updateObjects()`—called only every 5 seconds—reduces CPU load compared to per-frame scanning[5].

• **Clean Hotkey Integration**: Input handling is abstracted into a `Hotkeys.cs` module, providing loose coupling and room for future expansion.


⸻


Weaknesses

Despite its functional strengths, the UI has several significant limitations in terms of usability, maintainability, and scalability:


1. Use of Legacy `OnGUI` System

The reliance on Unity’s `OnGUI()` method, while fast and injectable, is problematic for several reasons:
• `OnGUI` runs multiple times per frame (layout, repaint, etc.), increasing CPU overhead unnecessarily.
• It lacks layout systems, making responsive or dynamic positioning complex.
• It's deprecated in favor of **Unity UI (uGUI)** or **IMGUI** in new Unity versions.
• Debug spam is generated if used improperly (e.g., layout calls during repaint).
> *Current pattern:* Direct `GUI.DrawTexture`, `GUI.Label`, `GUI.color` state manipulation in `OnGUI()`.


2. Hardcoded Values in UI Layout

The menu position and dimensions are hardcoded:

private static readonly int MENU_X = 50;
private static readonly int MENU_Y = 50;
private static readonly int MENU_WIDTH = 200;
private static readonly int MENU_HEIGHT = 150;

[4]


This leads to:
• Poor responsiveness across different screen resolutions.
• Potential overlap with HUD elements or other mods.


3. Brittle Click Detection

The menu uses manual pixel-accurate mouse collision checks:

if (mousePos.x >= MENU_X + 10 && mousePos.x <= MENU_X + 150 && 
    mousePos.y >= MENU_Y + 40 && mousePos.y <= MENU_Y + 55)

[4]


This is:
• Highly fragile (sensitive to layout changes).
• Difficult to maintain (must update coordinates in multiple places).
• Not scalable (adding new buttons multiplies error-prone code).


4. State and UI Logic Coupling

UI toggle states (`enemyESP`, `enemyBones`) are stored as public static fields in `Hacks.cs`, directly accessed by `UI.cs`. This breaks encapsulation and leads to tight coupling.


5. No Theming or Styling System

All colors and fonts are hardcoded (e.g., `Color.cyan`, `Color.red`). There's no centralized style system, making global theme changes laborious.


6. Redundant Entity Updates

While `updateObjects()` runs every 5 seconds, it collects **players** and **items** even though the mod only visualizes enemies—a minor but avoidable performance drain[1][5].


⸻


2. Step-by-Step UI Improvements

Below is a prioritized, beginner-friendly improvement plan, listed from **most critical** to **nice-to-have**.


⸻


Step 1: Transition to IMGUI (Immediate Mode GUI)

**Priority**: High  
**Difficulty**: Beginner to Intermediate  
**Reasoning**: IMGUI fixes `OnGUI`’s inefficiencies without requiring UI components (GameObjects) or Canvas systems, making it ideal for injected ESP mods.


IMGUI calls are stateless and run only when needed. Unity supports it via `GUILayout` and `EditorWindow`-like patterns, even at runtime.


**Action**:
Replace `OnGUI()` in `Hacks.cs` with a structured IMGUI block.


void OnGUI()
{
    if (!isLoaded) return;
    
    // Begin an automatic layout area
    GUILayout.BeginArea(new Rect(MENU_X, MENU_Y, MENU_WIDTH, MENU_HEIGHT));
    
    GUILayout.Label("7D2D ESP Menu", GUI.skin.box);
    
    // Draw toggles automatically with IMGUI
    enemyESP = GUILayout.Toggle(enemyESP, $"Enemy ESP {(enemyESP ? "[ON]" : "[OFF]")}");
    enemyBones = GUILayout.Toggle(enemyBones, $"Enemy Bones {(enemyBones ? "[ON]" : "[OFF"])]");
    
    GUILayout.Space(10);
    GUILayout.Label($"Enemies: {eEnemy.Count}", GUI.skin.label);
    GUILayout.Label("Insert: Toggle Menu", GUI.skin.label);
    GUILayout.Label("End: Unload", GUI.skin.label);
    
    GUILayout.EndArea();
}

[5]


**Benefits**:
• IMGUI handles layout and event routing automatically.
• No manual coordinate math for clicks.
• Future-proof and cleaner.


**Note**: Ensure `Use IMGUI` is enabled in Unity build settings if compiling the mod in a Unity environment.


⸻


Step 2: Make UI Positioning Dynamic

**Priority**: High  
**Difficulty**: Beginner  
**Reasoning**: Improves usability across aspect ratios and resolutions.


**Action**: Center the menu or anchor it to screen edges.


// Example: Center the menu
private static Vector2 GetMenuPosition()
{
    return new Vector2(
        (Screen.width - MENU_WIDTH) / 2,
        (Screen.height - MENU_HEIGHT) / 2
    );
}


Apply in `OnGUI`:

Vector2 pos = GetMenuPosition();
GUILayout.BeginArea(new Rect(pos.x, pos.y, MENU_WIDTH, MENU_HEIGHT));


⸻


Step 3: Decouple UI from Game State

**Priority**: Medium  
**Difficulty**: Intermediate  
**Reasoning**: Makes code more testable, reusable, and maintainable.


**Action**: Introduce a central settings class.


public static class ESPSettings
{
    public static bool ShowEnemyESP { get; set; } = true;
    public static bool ShowEnemyBones { get; set; } = true;
    public static float MaxESPDistance { get; set; } = 100f;
}


Update `Hacks.cs`:

if (ESPSettings.ShowEnemyESP)
{
    foreach (var enemy in eEnemy)
    {
        if (enemy != null && enemy.IsAlive())
        {
            Modules.ESP.esp_drawBox(enemy, Color.red);
        }
    }
}


Update UI:

ESPSettings.ShowEnemyESP = GUILayout.Toggle(ESPSettings.ShowEnemyESP, "Enemy ESP");


⸻


Step 4: Add Configuration Persistence

**Priority**: Medium  
**Difficulty**: Intermediate  
**Reasoning**: Users keep their preferences across sessions.


**Action**: Use `PlayerPrefs` to save/load settings.


public static void LoadSettings()
{
    ShowEnemyESP = PlayerPrefs.GetInt("EnemyESP", 1) == 1;
    ShowEnemyBones = PlayerPrefs.GetInt("EnemyBones", 1) == 1;
}

public static void SaveSettings()
{
    PlayerPrefs.SetInt("EnemyESP", ShowEnemyESP ? 1 : 0);
    PlayerPrefs.SetInt("EnemyBones", ShowEnemyBones ? 1 : 0);
    PlayerPrefs.Save();
}


Call `LoadSettings()` in `Hacks.Start()`.


⸻


Step 5: Optimize Entity Scanning

**Priority**: Medium  
**Difficulty**: Beginner  
**Reasoning**: Reduce CPU usage.


**Action**: Only collect what's needed.


In `updateObjects()`:

// Remove collection of players and items
var enemies = GameObject.FindObjectsOfType<EntityEnemy>();
eEnemy.Clear();
foreach (var enemy in enemies)
{
    if (enemy != null && enemy.IsAlive())
    {
        eEnemy.Add(enemy);
    }
}

[5]


⸻


Step 6: Add a Basic Configuration Menu

**Priority**: Low  
**Difficulty**: Intermediate  
**Reasoning**: Allows runtime adjustment of ESP features.


**Action**: Add sliders or input fields in IMGUI layout:

ESPSettings.MaxESPDistance = GUILayout.HorizontalSlider(ESPSettings.MaxESPDistance, 10f, 200f, GUILayout.Width(100));
GUILayout.Label($"Max Distance: {ESPSettings.MaxESPDistance:F0}m");


⸻


Step 7: Introduce a Style Manager

**Priority**: Low  
**Difficulty**: Intermediate  
**Reasoning**: Promotes consistent visual design.


public static class UIStyle
{
    public static Color Background => new Color(0.1f, 0.1f, 0.1f, 0.9f);
    public static Color TextNormal => Color.white;
    public static Color TextActive => Color.green;
    public static Color TextInactive => Color.red;
}


Apply in `UI.DrawMenu` or IMGUI styles.


⸻


3. Basic UI Framework

To support a clean, efficient, and consistent UI, adopt this minimal framework structure:


Core Principles
• Use **IMGUI** for overlays instead of raw `OnGUI()`.
• **Separate logic from presentation** using service or settings classes.
• Keep rendering stateless where possible.
• Minimize object allocations and texture usage.


Framework Structure

UI/
├── UIStyle.cs      — Central color/font/texture references
├── UIManager.cs    — Controls menu visibility, hotkey binding
├── ESPRenderer.cs  — New component: draws entity ESP with configuration
└── Settings.cs     — Holds mod settings (static or scriptable object)


Example: `UIManager.cs`

public static class UIManager
{
    public static bool IsVisible { get; private set; }

    public static void ToggleMenu()
    {
        IsVisible = !IsVisible;
    }

    public static void OnGUI()
    {
        if (!IsVisible || !Hacks.isLoaded) return;

        // Use custom skin if needed
        DrawMainMenu();
    }

    private static void DrawMainMenu()
    {
        GUILayout.BeginArea(new Rect(GetCenteredRect()));
        // ... IMGUI content
        GUILayout.EndArea();
    }

    private static Rect GetCenteredRect()
    {
        return new Rect(
            (Screen.width - 250) / 2,
            (Screen.height - 200) / 2,
            250, 200
        );
    }
}


Then integrate into `Hacks.OnGUI()`:

void OnGUI()
{
    UIManager.OnGUI();
}


This structure ensures minimal overhead while supporting clean, scalable UI rendering.


⸻


4. Scalability Considerations

To ensure the UI remains maintainable and performant as features grow:


1. Use Modular Pattern

Break UI into distinct sections:
• `MenuSystem` – Core visibility and navigation.
• `HUD` – In-game overlays (e.g., health bars, crosshair).
• `DebugUI` – Developer tools (render stats, logs).
• `SettingsUI` – Configuration panels.


Each module has its own `OnGUI()` or IMGUI handler.


2. Event-Driven Updates

Instead of polling, use events:

public static class ESPEvents
{
    public static Action<bool> OnEnemyESPChanged;
}


Fire when settings change. UI modules can subscribe for dynamic updates.


3. Component-Based Design

If transitioning to Unity UI later, create `MonoBehaviour` components:
• `ESPBoxComponent` – Attached to enemies.
• `PlayerESP` – Configures behavior.


4. Performance Guards
• Skip rendering if `Camera.main == null`.
• Use occlusion checks (e.g., `entity.IsVisible()`).
• Throttle expensive operations (e.g., bone rendering) based on distance.


5. Configuration Over Code

Use JSON, XML, or `ScriptableObject` (in Unity editor) to externalize settings like:

{
  "enemyBoxColor": [1.0, 0.0, 0.0],
  "maxDistance": 100.0,
  "showNames": true
}


⸻


5. Beginner-Friendly Resources

Here are curated resources to help implement these improvements:

• [**Unity IMGUI Documentation**](https://docs.unity3d.com/Manual/GUIScriptingGuide.html)[1] — Official guide to immediate mode GUI system.
• [**Unity UI (uGUI) Tutorial Series**](https://learn.unity.com/tutorial/ui) — Free interactive lessons from Unity Learn.
• [**C# for Beginners – Microsoft Learn**](https://learn.microsoft.com/en-us/dotnet/csharp/) — Learn C# syntax and object-oriented patterns.
• [**Unity 2D Game Development Course** (freeCodeCamp)](https://www.youtube.com/watch?v=PMVYjCGvy2w) — Covers UI, input, and scene management.
• [**GitHub: Unity Scripting Reference**](https://docs.unity3d.com/ScriptReference/) — Essential API lookup for `GUI`, `GUILayout`, `Camera`, etc.
• [**Modding 7 Days to Die – Community Wiki**](https://7daystodie.com/forums/forum/25-modding/) — Tips, reverse-engineering, and DLL injection techniques.


⸻


Conclusion

The 7dtDAibot project already excels in performance and simplicity, perfect for a lightweight ESP. However, upgrading from raw `OnGUI()` to a structured IMGUI approach, decoupling logic from presentation, and introducing configuration will dramatically improve maintainability and user experience. These changes are achievable by a developer with basic C# and Unity knowledge and will future-proof the mod for additional features.


Adopting the recommended UI framework and scalability patterns will ensure that as new ESP features (e.g., player tracking, item ESP) are added, the codebase remains clean, efficient, and easy to refine.


References:
[1]: https://raw.githubusercontent.com/Alonso-Slls/7dtDAibot/main/README.md
[2]: https://raw.githubusercontent.com/Alonso-Slls/7dtDAibot/main/Render.cs
[3]: https://raw.githubusercontent.com/Alonso-Slls/7dtDAibot/main/Class1.cs
[4]: https://raw.githubusercontent.com/Alonso-Slls/7dtDAibot/main/Modules/UI.cs
[5]: https://raw.githubusercontent.com/Alonso-Slls/7dtDAibot/main/Hacks.cs
[6]: https://github.com/Alonso-Slls/7dtDAibot
[7]: https://raw.githubusercontent.com/Alonso-Slls/7dtDAibot/main/Modules/ESP.cs