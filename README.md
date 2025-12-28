# 7D2D Enhanced ESP Framework

A comprehensive ESP (Extra Sensory Perception) and debugging framework for 7 Days to Die that provides advanced entity visual assistance with robust logging and performance monitoring.

## Features

### ESP Functionality
- **Enemy ESP** - Red boxes around zombies with distance display
- **Player ESP** - Green boxes around other players (excludes self)
- **Animal ESP** - Yellow boxes around animals
- **Item ESP** - Cyan boxes around items
- **Dynamic box sizing** - Boxes scale based on distance for better visibility
- **Distance filtering** - Shows exact distance in meters for each entity

### Advanced Debugging System
- **Robust file-based logging** with automatic log rotation
- **Performance monitoring** with millisecond timing
- **Error tracking** with separate error logs
- **Diagnostic reports** with game state information
- **Frame-by-frame logging** for debugging
- **Log file management** with size limits (10MB) and rotation (5 files)

### Build Automation
- **Cross-platform build scripts** (Windows .bat and Unix .sh)
- **Automated dependency checking**
- **Build status monitoring**
- **Injection-ready output**

## Controls

- **Insert** - Toggle menu on/off
- **F1** - Toggle ESP on/off
- **F2** - Generate diagnostic report

## ⚠️ Multithreading Stability Warning

Unity APIs used by this framework (entity discovery, GUI drawing, camera access, etc.) must run on the main Unity thread. Running the ESP inside custom loaders, trainers, or injectors that spawn additional worker threads can lead to **race conditions and hard crashes**. Before experimenting with multithreaded helpers:

1. Keep all ESPManager interactions on the Unity main thread.
2. Avoid invoking `FindObjectsOfType`, `Camera.main`, or GUI calls from background threads.
3. Do not overlap custom timers with the built‑in `InvokeRepeating` calls unless they are synchronized.
4. If you introduce new threaded code, wrap file I/O and log-rotation logic with your own synchronization to prevent corruption.

Proceed at your own risk—instability caused by multithreading is not supported.

## Installation

1. Build the project:
   ```bash
   # Windows
   build.bat build
   
   # Unix/Linux
   ./build.sh build
   ```

2. Inject the `SevenDtDAibot.dll` into the game using your preferred injector
3. Press Insert to open the menu
4. Use F1 to toggle ESP functionality

## Technical Details

### Architecture

- **Loader.cs** - Entry point that initializes the ESP system
- **ESPManager.cs** - Main component handling ESP rendering, input, and entity management
- **RobustDebugger.cs** - Comprehensive logging and debugging system

### Dependencies

- **.NET Framework 4.7.2**
- **Unity Engine** (Multiple modules included)
- **0Harmony** - For game patching
- **7 Days to Die Assembly-CSharp** - Game assemblies
- **70+ Unity modules** - Complete Unity framework integration

## Repository

**GitHub Repository:** https://github.com/Alonso-Slls/7dtDAibot/tree/main

## Build Commands

```bash
# Build project
build.bat build

# Clean build artifacts
build.bat clean

# Rebuild project
build.bat rebuild

# Build and prepare for injection
build.bat run

# Show project status
build.bat status
```

## Performance

- **Optimized rendering** using Unity's OnGUI system
- **Distance-based culling** to improve performance
- **Performance logging** to track rendering times
- **Minimal CPU overhead** with efficient entity scanning
- **Memory-efficient** with proper cleanup and log rotation

## Logging System

The framework includes a comprehensive logging system with three types of logs:

- **Main Log** (`logs/esp_debug.log`) - General debugging information
- **Error Log** (`logs/esp_errors.log`) - Error tracking and exceptions
- **Performance Log** (`logs/esp_performance.log`) - Performance metrics

### Log Features
- Automatic rotation when files exceed 10MB
- Thread-safe logging with proper synchronization
- Timestamped entries with millisecond precision
- Game state tracking and camera information
- Entity scanning statistics

## Troubleshooting

### ESP Not Showing
- Ensure you're in an active game (not main menu)
- Press F1 to toggle ESP on
- Check that the camera is available (menu should show camera status)
- Review logs in `logs/esp_debug.log` for errors

### Menu Not Appearing
- Press Insert key to toggle menu
- Verify the mod is properly injected
- Check logs for initialization errors

### Performance Issues
- Monitor performance logs for rendering times
- Check entity count in the menu
- Ensure log rotation is working to prevent large log files

### Build Issues
- Verify .NET SDK is installed
- Check that all Unity assemblies are present in `Assm/` directory
- Run `build.bat status` to diagnose issues

## Version History

### v2.0.0 (Current)
- Enhanced debugging system with file-based logging
- Performance monitoring and optimization
- Cross-platform build automation
- Robust error handling and recovery
- Diagnostic reporting capabilities
- Log rotation and management

### v1.0.0
- Initial basic ESP framework
- Basic ESP for enemies, players, animals, and items
- Simple toggle menu system
- Distance display

## License

This project is for educational purposes only.

## Contributing

When contributing to this project:
- Follow the existing code style and patterns
- Add appropriate logging for new features
- Test with different game versions
- Update documentation for new functionality

## Support

For issues and support:
  1. Check the logs in the `logs/` directory
2. Generate a diagnostic report with F2
3. Review the troubleshooting section above
4. Ensure all dependencies are properly configured
