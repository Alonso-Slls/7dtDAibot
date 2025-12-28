# 7 Days to Die - Simplified ESP Mod

## Overview
A lightweight ESP (wallhack) mod for 7 Days to Die beta version. This mod provides clean enemy visualization with box ESP and bone skeleton drawing, optimized for performance and simplicity.

## Features
- **Enemy ESP Boxes** - Red bounding boxes around enemies with entity names
- **Enemy Bone ESP** - Green skeletal structure showing enemy bone positions
- **Simple Menu** - Clean toggle interface with just essential options
- **Performance Optimized** - Removed unnecessary features for better performance

## Removed Features (from original)
-

# 7 Days to Die - Simplified ESP Mod

## Overview
A lightweight ESP (wallhack) mod for 7 Days to Die beta version. This mod provides clean enemy visualization with box ESP and bone skeleton drawing, optimized for performance and simplicity.

## Features
- **Enemy ESP Boxes** - Red bounding boxes around enemies with entity names
- **Enemy Bone ESP** - Green skeletal structure showing enemy bone positions
- **Simple Menu** - Clean toggle interface with just essential options
- **Performance Optimized** - Removed unnecessary features for better performance

## Removed Features (from original)
- Aimbot functionality (completely removed)
- Creative menu activation
- Non-enemy ESP (items, NPCs, players, animals)
- ESP lines drawing
- Complex menu system

## Controls
- **Insert Key** - Show/Hide menu
- **End Key** - Unload DLL safely
- **Mouse** - Use menu toggles to enable/disable features

## Installation
1. Compile the project using Visual Studio
2. Use a Mono injector (like MonoSharpInjector)
3. Select 7 Days to Die process
4. Browse to `Game_7D2D.dll`
5. Use injection settings:
   - Namespace: `Game_7D2D`
   - Class name: [Loader](cci:2://file:///c:/Users/anoni/OneDrive/Escritorio/7/7Days2Die-ESP-Aimbot--Internal-/Class1.cs:4:4-24:5)
   - Method name: [init](cci:1://file:///c:/Users/anoni/OneDrive/Escritorio/7/7Days2Die-ESP-Aimbot--Internal-/Class1.cs:6:8-11:9)
6. Press inject

## Technical Details
- **Framework**: C# with Unity/Mono
- **Target**: 7 Days to Die (Beta version)
- **Rendering**: Custom OnGUI drawing system
- **Entity Detection**: GameObject.FindObjectsOfType
- **Update Rate**: 5-second intervals for entity scanning

## File Structure
```

# 7 Days to Die - Simplified ESP Mod

## Overview
Lightweight ESP mod for 7 Days to Die beta with enemy boxes and bone drawing.

## Features
- Enemy ESP boxes (red with names)
- Enemy bone skeleton (green)
- Simple toggle menu
- Performance optimized

## Controls
- Insert: Show/Hide menu
- End: Unload DLL

## Installation
1. Compile in Visual Studio
2. Use Mono injector
3. Settings: Namespace `Game_7D2D`, Class [Loader](cci:2://file:///c:/Users/anoni/OneDrive/Escritorio/7/7Days2Die-ESP-Aimbot--Internal-/Class1.cs:4:4-24:5), Method [init](cci:1://file:///c:/Users/anoni/OneDrive/Escritorio/7/7Days2Die-ESP-Aimbot--Internal-/Class1.cs:6:8-11:9)

## Files
- [Hacks.cs](cci:7://file:///c:/Users/anoni/OneDrive/Escritorio/7/7Days2Die-ESP-Aimbot--Internal-/Hacks.cs:0:0-0:0) - Main controller
- [ESP.cs](cci:7://file:///c:/Users/anoni/OneDrive/Escritorio/7/7Days2Die-ESP-Aimbot--Internal-/Modules/ESP.cs:0:0-0:0) - ESP rendering
- [UI.cs](cci:7://file:///c:/Users/anoni/OneDrive/Escritorio/7/7Days2Die-ESP-Aimbot--Internal-/Modules/UI.cs:0:0-0:0) - Menu system
- [Render.cs](cci:7://file:///c:/Users/anoni/OneDrive/Escritorio/7/7Days2Die-ESP-Aimbot--Internal-/Render.cs:0:0-0:0) - Drawing utilities
- [Hotkeys.cs](cci:7://file:///c:/Users/anoni/OneDrive/Escritorio/7/7Days2Die-ESP-Aimbot--Internal-/Modules/Hotkeys.cs:0:0-0:0) - Input handling