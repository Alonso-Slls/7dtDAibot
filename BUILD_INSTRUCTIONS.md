# Build Instructions for 7D2D Enhanced ESP Project

## Project Status
**Status: Enhanced and Optimized**
This project has been significantly improved with enhanced UI, performance optimizations, and better stability. All planned improvements from the update plan have been implemented.

## Prerequisites
- Visual Studio 2019/2022 with .NET Framework development tools
- 7 Days to Die game installation (for DLL references)
- Mono injector (e.g., MonoSharpInjector)

## Build Steps

### 1. Open Project
- Open `7D2D.sln` in Visual Studio
- Ensure the solution targets .NET Framework 4.7.1

### 2. Verify References
The project should reference these DLLs in the `Assm` folder:
- `UnityEngine.CoreModule.dll`
- `UnityEngine.dll`
- `UnityEngine.UIModule.dll`
- `UnityEngine.ImageConversionModule.dll`
- `Assembly-CSharp.dll`
- `Assembly-CSharp-firstpass.dll`
- `Unity.Postprocessing.Runtime.dll`

### 3. Build Configuration
- Select **Release** configuration
- Target **Any CPU**
- Build the project (Build > Build Solution)

### 4. Output
The compiled DLL will be generated as:
```
bin/Release/Game_7D2D.dll
```

## Installation

### Using Mono Injector
1. Launch 7 Days to Die
2. Attach MonoSharpInjector to the game process
3. Configure injection settings:
   - **DLL Path**: Path to `Game_7D2D.dll`
   - **Namespace**: `Game_7D2D`
   - **Class**: `Loader`
   - **Method**: `init`
4. Click "Inject"

### Alternative Injection Tools
- Other Mono injectors with similar configuration
- Ensure the injector supports Unity Mono injection

## Usage Controls

### Hotkeys
- **Insert** - Toggle menu visibility
- **F1** - Toggle Enemy ESP
- **F2** - Toggle Enemy Bones
- **F3** - Force update entities
- **End** - Unload hack safely

### Enhanced Features
- **Improved UI**: Better menu positioning and responsiveness
- **Performance Optimizations**: Enhanced entity scanning and rendering
- **Stability**: Improved error handling and safe unloading
- **Code Quality**: Better modularity and maintainability

### Menu Interaction
- Click on menu options to toggle features
- Menu shows current status [ON/OFF]
- Entity count displayed in real-time

## Troubleshooting

### Common Issues

#### "DLL not found" errors
- Verify all required DLLs are in the `Assm` folder
- Check that paths in project file are correct
- Ensure game version matches DLL versions

#### Injection fails
- Run injector as administrator
- Check if game is running before injecting
- Verify injection parameters match exactly

#### ESP not showing
- Press Insert to show menu
- Ensure Enemy ESP is enabled
- Check if game is fully loaded
- Verify entities are within 100m range

#### Compilation errors
- Ensure .NET Framework 4.7.1 is installed
- Check Visual Studio has required workloads
- Verify all references are valid

### Debug Mode
For debugging, use Debug configuration and check Unity console:
- Unity logs appear in game console
- Debug.Log statements help track issues
- Check for error messages in injector

## Technical Notes

### Entity Detection
- Updates every 5 seconds for performance
- Uses `GameObject.FindObjectsOfType<T>()`
- Filters for alive entities only

### Rendering System
- Uses Unity's enhanced OnGUI for overlay
- World-to-screen projection via `Camera.main.WorldToScreenPoint()`
- Custom drawing utilities in `Render.cs`
- Improved UI positioning and responsiveness
- Enhanced menu system with better click detection

### Memory Safety
- No direct memory manipulation
- Uses Unity's managed API
- Safe unloading via GameObject destruction

## Compatibility

### Supported Game Versions
- Designed for 7 Days to Die (Unity 2018-2020 era)
- .NET Framework 4.x compatible
- Mono runtime required

### Performance Impact
- Minimal performance overhead
- Throttled entity updates
- Efficient OnGUI rendering

## Current Implementation Status

### Completed Enhancements
✅ **UI Improvements**: Enhanced menu system with better positioning
✅ **Performance Optimizations**: Optimized entity scanning and rendering
✅ **Code Quality**: Improved modularity and maintainability
✅ **Stability**: Enhanced error handling and safe unloading
✅ **Documentation**: Updated all documentation files

### Project Structure
The project now includes:
- `ESPConfiguration.cs` - ESP settings management
- `EnhancedESPManager.cs` - Enhanced ESP management
- `RobustDebugger.cs` - Debug utilities
- Improved `Modules/` structure
- Enhanced documentation

## Legal Notice

This project is for educational purposes only. Ensure compliance with:
- Game's End-User License Agreement
- Local laws regarding game modifications
- Respect for other players in multiplayer environments

## Support

For issues:
1. Check troubleshooting section
2. Verify game version compatibility
3. Review Unity console for errors
4. Ensure correct injection parameters