# 7dtDAibot - 7 Days to Die Advanced AI Bot v3.2.0

## Overview

This is a clean, modern C# AI bot framework for 7 Days to Die featuring advanced ESP, aimbot, and utility functions with enterprise-grade architecture and performance optimizations. The project demonstrates advanced game AI techniques, performance optimization patterns, and clean software architecture.

**Version 3.2.0**: Complete legacy code cleanup with modern config integration and enhanced performance.

**Note**: This mod works with WeMod loaded as well.

## Architecture v3.2.0

### Core Components (13 Modules)
- **HacksManager**: Central singleton managing all mod systems
- **Config**: Centralized configuration management with JSON persistence
- **ErrorHandler**: Centralized error handling and reporting
- **DetailedLogger**: Comprehensive logging system with multiple levels

### Entity System
- **EntityTracker**: Generic entity tracking with optimized scanning
- **EntityManager**: Entity lifecycle management
- **EntitySubscription**: Event-driven entity updates
- **SpatialGrid**: Spatial indexing for performance

### Rendering System
- **ESPRenderer**: Specialized ESP rendering with caching
- **BatchedRenderer**: High-performance batched draw calls
- **ESP**: Modular ESP system with entity-specific rendering
- **ESPPool**: Object pooling for performance optimization

### User Interface
- **UI**: Clean user interface with direct config integration
- **Hotkeys**: Responsive hotkey system

### Combat System
- **Aimbot**: Clean aimbot implementation with target selection

## Enhanced Features v3.2.0

### ESP (Extra Sensory Perception)
- **Multi-target ESP**: Animals, Zombies/Enemies, Players, Dropped Items, NPCs
- **Enhanced Bone ESP**: Advanced skeleton detection with adaptive colors
- **Distance Display**: Shows exact distance in meters for all entities
- **Extended Range**: 200m render distance for enemies
- **FOV Culling**: Only renders entities within configurable field of view
- **Direct Rendering**: No pooling overhead for better performance
- **Enhanced Bounds Checking**: 50px margin for better edge detection
- **Adaptive Quality**: Bone colors change based on distance

### Aimbot System
- **Multi-target Support**: Enemies, Players, Animals
- **Smart Targeting**: Head detection and tracking
- **FOV Control**: Configurable aimbot field of view
- **Smooth Aiming**: Adjustable smoothness for natural movement
- **Visual FOV Indicator**: Optional circle showing aimbot range
- **Raw Mode**: Direct cursor positioning option

### Performance Optimizations
- **Direct Rendering**: Eliminated object pooling overhead
- **Early Distance Culling**: Faster rejection of distant entities
- **Enhanced Bounds Checking**: More efficient screen coordinate validation
- **Reduced Bone Threshold**: Lower minimum bones required (6 instead of 8)
- **Spatial Grid**: Efficient spatial indexing for entity queries
- **Batched Operations**: Reduced draw call overhead

### Configuration System
- **JSON-based Settings**: Persistent configuration storage
- **Real-time Updates**: Changes apply immediately without restart
- **Named Constants**: All magic numbers replaced with configurable values
- **Validation**: Automatic validation and correction of invalid settings

## Installation

### Prerequisites
- 7 Days to Die (latest version)
- MonoSharpInjector or compatible Mono injector
- .NET Framework compatibility

### Build Instructions
1. Clone the repository
2. Open `7D2D.sln` in Visual Studio or use `dotnet build`
3. Build in Release configuration
4. Locate `SevenDtDAibot.dll` in the output folder

### Injection
1. Launch 7 Days to Die
2. Use MonoSharpInjector or compatible Mono injector
3. Select the game process
4. Browse to `SevenDtDAibot.dll`
5. Use these injection settings:
   - **Namespace**: `SevenDtDAibot`
   - **Class name**: `Loader`
   - **Method name**: `init`
6. Press Inject

## Controls

### Menu Navigation
- **Insert**: Toggle mod menu on/off
- **End**: Safe unload mod DLL
- **Mouse**: Navigate menu options

### In-Game Shortcuts
- **Numpad 1**: Enable Creative Menu (works in online servers)
- **Right Mouse**: Activate aimbot (when enabled)

### Menu Options

#### ESP Configuration
- **Enable ESP**: Master toggle for all ESP features
- **Enemy ESP**: Toggle zombie/enemy detection
- **Player ESP**: Toggle player detection
- **Animal ESP**: Toggle animal detection
- **Item ESP**: Toggle item detection
- **NPC ESP**: Toggle NPC detection
- **Draw Boxes**: Toggle bounding box rendering
- **Draw Lines**: Toggle corner-to-center lines
- **Enemy Bones**: Toggle skeleton rendering
- **Enable FOV-Aware ESP**: Only show entities in view

#### Aimbot Configuration
- **Activate Aimbot**: Master toggle for aimbot
- **Auto Aim**: Toggle automatic aiming
- **Target Selection**: Choose target types (Enemies, Players, Animals)
- **Show FOV Circle**: Visual aimbot range indicator
- **Aim FOV**: Adjust aimbot field of view
- **Aim Smooth**: Adjust aiming smoothness
- **Raw Mode**: Direct cursor positioning

#### Visual Settings
- **FOV Threshold**: Adjust field of view angle (60-180 degrees)
- **Colors**: Customize colors for each entity type

## Configuration

### Settings File
Configuration is saved to `SevenDtDAibot_config.json` in the game directory:

```json
{
  "ESPEnabled": true,
  "EnemyESP": true,
  "PlayerESP": false,
  "AnimalESP": false,
  "ItemESP": true,
  "NPCESP": false,
  "ESPBoxes": true,
  "ESPLines": false,
  "EnemyBones": true,
  "FOVAwareESP": true,
  "FOVThreshold": 120.0,
  "AimbotEnabled": false,
  "AutoAim": false,
  "AimbotRaw": false,
  "TargetEnemies": false,
  "TargetAnimals": false,
  "TargetPlayers": false,
  "ShowFOVCircle": false,
  "AimFOV": 150,
  "AimSmooth": 5.0,
  "EnemyColor": [1.0, 0.0, 0.0, 1.0],
  "PlayerColor": [0.0, 1.0, 0.0, 1.0],
  "AnimalColor": [1.0, 1.0, 0.0, 1.0],
  "ItemColor": [0.0, 1.0, 1.0, 1.0],
  "NPCColor": [1.0, 0.0, 1.0, 1.0]
}
```

## Technical Details

### Entity Tracking
The mod uses a subscription-based entity tracking system that:
- Monitors entity spawn/despawn events
- Maintains efficient spatial indexing
- Provides O(1) lookup for nearby entities
- Automatically cleans up destroyed entities

### Rendering Pipeline
- **Direct Rendering**: Bypasses Unity's immediate mode for speed
- **Batched Rendering**: Groups similar draw calls for performance
- **Smart Culling**: Only renders visible entities
- **Distance-based LOD**: Reduces quality for distant objects

### Memory Management
- **No Pooling Overhead**: Direct rendering eliminates allocation/deallocation cycles
- **Better Error Handling**: Prevents memory leaks from exceptions
- **Optimized Entity Tracking**: Reduced memory footprint with spatial grid

### Error Handling
- **Graceful Degradation**: Mod continues working even if individual features fail
- **Comprehensive Logging**: Detailed error reporting for debugging
- **Safe Unloading**: Clean removal without game crashes

## Troubleshooting

### Common Issues

**Injection Failed**
- Ensure correct namespace: `SevenDtDAibot`
- Verify class name: `Loader`
- Check method name: `init`

**ESP Not Working**
- Press Insert to open menu
- Enable "ESP" master toggle
- Enable specific ESP types (Enemy ESP, etc.)
- Enable "Draw Boxes" for visual rendering

**Performance Issues**
- Reduce ESP range in settings
- Disable bone ESP for distant entities
- Turn off FOV-aware ESP if not needed

**Crashes on Load**
- Check game version compatibility
- Ensure all dependencies are loaded
- Review error logs in `mod_errors.log`

### Debug Information
Enable debug overlay in the menu to see:
- Entity counts
- Render statistics
- Performance metrics
- Error information

## Performance Architecture

### Rendering Optimizations
- **Direct GPU Calls**: Bypasses Unity's immediate mode for speed
- **Early Distance Culling**: Faster rejection of distant entities
- **Enhanced Bounds Checking**: More efficient screen coordinate validation
- **Adaptive Quality**: Performance scales with distance

### CPU Optimizations
- **Fewer W2S Calls**: Optimized bone rendering to reduce expensive transformations
- **Early Rejection**: Faster culling of off-screen entities
- **Batched Operations**: Reduced draw call overhead

### Memory Management
- **No Pooling Overhead**: Direct rendering eliminates allocation/deallocation cycles
- **Better Error Handling**: Prevents memory leaks from exceptions
- **Optimized Entity Tracking**: Reduced memory footprint with spatial grid

## Changelog

### v3.2.0 (Current)
- Complete legacy code cleanup
- Removed all t_ variables from UI
- Direct config integration throughout
- Enhanced performance optimizations
- Better error handling
- Modern architecture patterns

### v3.1.0
- Enhanced ESP rendering with distance display
- Improved bone ESP with adaptive colors
- Extended render distance to 200m
- Removed object pooling for better performance
- Better error handling and null checks

### v3.0.0
- Complete architectural cleanup
- Removed all legacy code
- Streamlined functionality
- Improved performance
- Better error handling

## License

This project is for educational purposes only. Use responsibly and at your own risk.

## Contributing

Contributions are welcome! Please ensure:
- Code follows existing patterns
- Features include error handling
- Performance is considered
- Documentation is updated
