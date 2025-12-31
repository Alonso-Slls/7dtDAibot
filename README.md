# 7 Days to Die - Enhanced ESP Mod

## Overview
A lightweight and enhanced ESP (wallhack) mod for 7 Days to Die beta version. This mod provides clean enemy visualization with box ESP and bone skeleton drawing, optimized for performance and simplicity. The project has been significantly improved with better UI, performance optimizations, and enhanced stability.

## Features
- **Enemy ESP Boxes** - Red bounding boxes around enemies with entity names
- **Enemy Bone ESP** - Green skeletal structure showing enemy bone positions
- **Enhanced Menu System** - Improved UI with better positioning and responsiveness
- **Performance Optimized** - Removed unnecessary features for better performance
- **Stable Entity Detection** - Efficient 5-second interval entity scanning
- **Safe Unloading** - Clean DLL unloading mechanism

## Implemented Improvements
- **UI Enhancements**: Better menu positioning, improved click detection, cleaner interface
- **Performance Optimizations**: Optimized entity scanning and rendering
- **Code Quality**: Improved modularity and maintainability
- **Stability**: Enhanced error handling and safe unloading

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
   - Class name: `Loader`
   - Method name: `init`
6. Press inject

## Technical Details
- **Framework**: C# with Unity/Mono
- **Target**: 7 Days to Die (Beta version)
- **Rendering**: Enhanced OnGUI drawing system
- **Entity Detection**: GameObject.FindObjectsOfType
- **Update Rate**: 5-second intervals for entity scanning
- **UI System**: Improved positioning and responsiveness

## File Structure
```
7Days2Die-ESP-Aimbot--Internal-/
├── Class1.cs                 # Entry point loader
├── Hacks.cs                  # Main controller
├── ESPConfiguration.cs       # ESP settings management
├── EnhancedESPManager.cs     # Enhanced ESP management
├── RobustDebugger.cs         # Debug utilities
├── Modules/
│   ├── ESP.cs               # ESP rendering
│   ├── Hotkeys.cs           # Input handling
│   └── UI.cs                # Menu system
├── bin/Release/
│   ├── Game_7D2D.dll        # Compiled mod
│   └── Game_7D2D.pdb        # Debug symbols
└── Documentation/
    ├── README.md
    ├── BUILD_INSTRUCTIONS.md
    ├── Technical_Analysis_Report.md
    └── Comprehensive_DLL_Analysis.md
```