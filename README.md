# 7 Days to Die - Enhanced ESP Mod

## Overview
A comprehensive and optimized ESP (wallhack) mod for 7 Days to Die. This enhanced implementation provides clean enemy visualization with advanced features, performance optimizations, and robust debugging capabilities. The project has been significantly improved with better UI, performance optimizations, and enhanced stability.

## Features

### Core ESP Functionality
- **Enemy ESP Boxes** - Red bounding boxes around enemies with entity names
- **Enemy Bone ESP** - Green skeletal structure showing enemy bone positions
- **Distance-based Filtering** - Configurable render distance (50-300m)
- **Real-time Entity Tracking** - Efficient entity detection and updates

### Enhanced UI System
- **Centered Menu Interface** - Clean, responsive IMGUI-based menu
- **Live Status Display** - Game status, entity count, FPS counter
- **Interactive Controls** - Toggle switches and sliders for settings
- **Debug Log Integration** - In-menu log viewing and export functionality

### Performance Optimizations
- **Coroutine-based Entity Updates** - Smooth entity processing without frame drops
- **Chunked Entity Processing** - Processes 10 entities per frame to maintain performance
- **Throttled Scanning** - 5-second interval updates to reduce overhead
- **Optimized Rendering** - Efficient OnGUI drawing system

### Advanced Debugging
- **RobustDebugger System** - Comprehensive logging with thread safety
- **Automatic Log Export** - Session data preservation on unload
- **Memory Buffer** - Recent log entries for quick debugging
- **Error Handling** - Graceful error recovery and user feedback

## Technical Architecture

### Injection System
- **Framework**: C# with Unity/Mono runtime
- **Injection Method**: Mono DLL injection
- **Target**: 7 Days to Die (Unity-based game)
- **Entry Point**: `Game_7D2D.Loader.init()`

### Core Components

#### Main Controller (`Hacks.cs`)
- **Entity Management**: Coroutine-based entity detection and filtering
- **Game State Detection**: Multi-method game loading detection
- **Rendering Pipeline**: OnGUI-based ESP and menu rendering
- **Lifecycle Management**: Safe initialization and cleanup

#### Configuration System (`ESPConfiguration.cs`)
- **Settings Persistence**: JSON-based configuration storage
- **Performance Tuning**: Optimized defaults for smooth gameplay
- **Feature Toggles**: Granular control over ESP features
- **Hotkey Management**: Customizable key bindings

#### Enhanced ESP Manager (`EnhancedESPManager.cs`)
- **Advanced Rendering**: Sophisticated ESP drawing algorithms
- **Performance Monitoring**: Real-time performance metrics
- **Entity Filtering**: Smart entity classification and filtering
- **Visual Enhancements**: Improved box calculations and bone rendering

#### Debug Framework (`RobustDebugger.cs`)
- **Comprehensive Logging**: Multi-level logging system
- **Thread Safety**: Concurrent log writing
- **Export Functionality**: Automatic and manual log exports
- **Memory Management**: Efficient log buffer management

### Module System

#### ESP Module (`Modules/ESP.cs`)
- **Box Rendering**: Accurate bounding box calculations
- **Bone Skeleton**: Enemy skeletal structure visualization
- **Distance Filtering**: Range-based entity visibility
- **World-to-Screen**: 3D to 2D coordinate transformation

#### Hotkeys Module (`Modules/Hotkeys.cs`)
- **Input Handling**: Responsive key detection
- **Menu Toggle**: Insert key for menu visibility
- **Feature Controls**: F1-F5 keys for various functions
- **Safe Unload**: End key for clean DLL removal

#### UI Module (`Modules/UI.cs`)
- **Menu Rendering**: Clean IMGUI interface
- **Status Display**: Real-time information panels
- **Control Elements**: Interactive toggles and sliders
- **Debug Integration**: In-menu log viewing

## Controls

### Primary Controls
- **Insert Key** - Toggle menu visibility
- **End Key** - Safely unload DLL
- **F1 Key** - Toggle Enemy ESP
- **F3 Key** - Force entity update
- **F4 Key** - Export debug logs
- **F5 Key** - Custom log export

### Menu Interaction
- **Mouse Click** - Toggle ESP features
- **Slider** - Adjust render distance
- **Buttons** - Export logs and access functions

## Installation

### Prerequisites
- Visual Studio 2019/2022 with .NET Framework 4.7.1
- 7 Days to Die game installation
- Mono injector (MonoSharpInjector recommended)

### Build Instructions
1. Open `7D2D.sln` in Visual Studio
2. Ensure all references in `Assm` folder are valid
3. Select Release configuration, Any CPU
4. Build Solution (F6)
5. Locate compiled `Game_7D2D.dll` in `bin/Release/`

### Injection Steps
1. Launch 7 Days to Die
2. Attach MonoSharpInjector to game process
3. Configure injection settings:
   - **DLL Path**: Path to `Game_7D2D.dll`
   - **Namespace**: `Game_7D2D`
   - **Class**: `Loader`
   - **Method**: `init`
4. Click "Inject"

## Configuration

### Settings File Location
```
%AppData%\..\LocalLow\The Fun Pimps\7 Days to Die\7dtDAibot\esp_config.json
```

### Default Settings
- **Enemy ESP**: Enabled
- **Render Distance**: 200m
- **Entity Scan Interval**: 0.2s
- **Verbose Logging**: Disabled
- **Performance Features**: Optimized for gameplay

## Performance Specifications

### System Requirements
- **OS**: Windows 7/8/10/11
- **Framework**: .NET Framework 4.7.1
- **Memory**: Minimal overhead (<50MB)
- **CPU Impact**: <2% during normal operation

### Optimization Features
- **Entity Chunking**: 10 entities per frame processing
- **Update Throttling**: 5-second entity refresh intervals
- **Coroutine Management**: Smooth frame-by-frame processing
- **Memory Efficiency**: Automatic cleanup and garbage collection

## Troubleshooting

### Common Issues

#### Injection Failures
- **Solution**: Run injector as administrator
- **Check**: Game process is running and accessible
- **Verify**: Injection parameters match exactly

#### ESP Not Displaying
- **Check**: Press Insert to show menu
- **Verify**: Enemy ESP is enabled in menu
- **Confirm**: Game is fully loaded (status shows "Loaded")
- **Test**: Entities are within render distance

#### Performance Issues
- **Adjust**: Reduce render distance slider
- **Disable**: Non-essential features in configuration
- **Check**: Entity count in menu status

#### Compilation Errors
- **Verify**: .NET Framework 4.7.1 is installed
- **Check**: All DLL references in Assm folder
- **Ensure**: Visual Studio has required workloads

### Debug Information
- **Console**: Unity Console shows real-time logs
- **Log Files**: Automatic export to logs directory
- **Menu**: Recent logs displayed in debug section
- **Export**: F4/F5 keys for manual log export

## File Structure

### Core Files
```
7Days2Die-ESP-Aimbot--Internal-/
├── Class1.cs                 # Entry point loader
├── Hacks.cs                  # Main controller
├── ESPConfiguration.cs       # Settings management
├── EnhancedESPManager.cs     # Advanced ESP system
├── RobustDebugger.cs         # Debug framework
├── PerformanceMonitor.cs    # Performance tracking
├── 7D2D.csproj              # Project configuration
├── 7D2D.sln                 # Solution file
└── Modules/                 # Component modules
    ├── ESP.cs               # ESP rendering
    ├── Hotkeys.cs           # Input handling
    └── UI.cs                # Menu interface
```

### Build Output
```
bin/Release/
├── Game_7D2D.dll            # Main compiled mod
├── Game_7D2D.pdb            # Debug symbols
└── (Other build artifacts)
```

### Dependencies
```
Assm/
├── UnityEngine.CoreModule.dll
├── UnityEngine.dll
├── UnityEngine.UIModule.dll
├── Assembly-CSharp.dll
├── Assembly-CSharp-firstpass.dll
└── (Other Unity assemblies)
```

## Development Notes

### Code Architecture
- **Separation of Concerns**: Modular design with clear responsibilities
- **Error Handling**: Comprehensive try-catch blocks with user feedback
- **Performance First**: Coroutine-based processing to maintain frame rates
- **Debug Friendly**: Extensive logging and monitoring capabilities

### Enhancement History
The project has undergone significant improvements:
- ✅ **UI Enhancements**: Enhanced menu system with better positioning
- ✅ **Performance Optimizations**: Optimized entity scanning and rendering
- ✅ **Code Quality**: Improved modularity and maintainability
- ✅ **Stability**: Enhanced error handling and safe unloading
- ✅ **Documentation**: Comprehensive documentation consolidation

### Removed Features (Optimization)
- Aimbot functionality (removed for focus on ESP)
- Creative menu activation (non-essential)
- Non-enemy ESP (players, animals, items - disabled for performance)
- Complex menu systems (simplified for usability)
- Resource-intensive visual effects

## Legal Notice

**Educational Purpose Only**: This project is intended for educational purposes and modding research. Ensure compliance with:
- Game's End-User License Agreement
- Local laws regarding game modifications
- Respect for other players in multiplayer environments
- Fair use principles for educational content

## Support and Contributing

### Getting Help
1. Review the troubleshooting section
2. Check debug logs for specific errors
3. Verify game version compatibility
4. Ensure correct injection parameters

### Technical Support
- **Logs**: Export debug logs using F4/F5 keys
- **Console**: Check Unity Console for real-time errors
- **Configuration**: Verify settings file integrity
- **Performance**: Monitor entity count and FPS in menu

---

**Version**: Enhanced and Optimized  
**Target**: 7 Days to Die (Unity 2018-2020 era)  
**Framework**: .NET Framework 4.7.1 with Mono runtime  
**Status**: Production Ready with Advanced Debugging