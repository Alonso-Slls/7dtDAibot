# 7D2D Basic ESP Framework

A minimal ESP (Extra Sensory Perception) mod for 7 Days to Die that provides basic entity visual assistance.

## Features

- ESP for different entity types:
  - **Enemy ESP** - Red boxes around zombies
  - **Player ESP** - Green boxes around other players
  - **Animal ESP** - Yellow boxes around animals
  - **Item ESP** - Cyan boxes around items

## Controls

- **Insert** - Toggle menu on/off
- **F1** - Toggle ESP on/off

## Installation

1. Build the project using Visual Studio or dotnet build
2. Inject the SevenDtDAibot.dll into the game
3. Press Insert to open the menu

## Technical Details

### Architecture

- **Loader.cs** - Entry point that initializes the ESP system
- **ESPManager.cs** - Main component that handles ESP rendering and input

### Dependencies

- .NET Framework 4.7.2
- Unity Engine
- 7 Days to Die Assembly-CSharp

## Configuration

The ESP framework is minimal with no external configuration files. All settings are controlled through the in-game menu.

## Performance

- Lightweight implementation with minimal performance impact
- Simple rendering using Unity's OnGUI system
- Basic distance-based filtering

## Troubleshooting

### ESP Not Showing
- Make sure you're in a game (not in main menu)
- Press F1 to toggle ESP on
- Check that the camera is available

### Menu Not Appearing
- Press Insert key to toggle menu
- Ensure the mod is properly injected

## Building

`ash
dotnet build 7D2D.csproj --configuration Release
`

The output DLL will be located at in\Release\SevenDtDAibot.dll

## Version History

### v1.0.0
- Initial minimal ESP framework
- Basic ESP for enemies, players, animals, and items
- Simple toggle menu system
- Distance display

## License

This project is for educational purposes only.
