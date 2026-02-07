<p align="center">

[![GitHub Release](https://img.shields.io/github/v/release/karpitony/eft-where-am-i?include_prereleases&logo=github)](https://github.com/karpitony/eft-where-am-i/releases/latest)
[![GitHub total downloads](https://img.shields.io/github/downloads/karpitony/eft-where-am-i/total.svg?include_prerelease&logo=github)](https://github.com/karpitony/eft-where-am-i/releases)
[![GitHub License](https://img.shields.io/github/license/karpitony/eft-where-am-i)](./LICENSE)
[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)

</p>

> **English (Current)** | **[한국어](README.md)**

# EFT: Where Am I

A companion tool that parses EFT screenshot filenames containing 3D coordinates and quaternion rotation data to show your exact location and facing direction on interactive [tarkov-market.com](https://tarkov-market.com/) maps.

> [!WARNING]
> This program is distributed under the [MIT License](./LICENSE). **We are not responsible for any consequences (including BSG bans or sanctions) resulting from the use of this program.**
> This program is not officially affiliated with [tarkov-market.com](https://tarkov-market.com/) and utilizes its map data and UI.

## Table of Contents

- [Features](#features)
- [Supported Maps](#supported-maps)
- [System Requirements](#system-requirements)
- [Download & Installation](#download--installation)
- [How to Use](#how-to-use)
- [Settings](#settings)
- [Contributing](#contributing)
- [Bug Reports & Suggestions](#bug-reports--suggestions)
- [Credits & Acknowledgments](#credits--acknowledgments)
- [License](#license)

## Features

### Screenshot-Based Location Display

**Pinpoint your exact position from a single screenshot.**

EFT screenshot filenames contain the player's 3D coordinates and rotation data. This tool parses that data and displays your position as a red dot on the tarkov-market.com map.

```
2026-01-10[03-59]_-318.44, 24.84, -107.49_0.00000, 0.82497, 0.00000, 0.56518_3.98 (0).png
│      Date       │    X,    Z,      Y    │     Quaternion Rotation (X,Y,Z,W)  │Spd │
```

<!-- TODO: Add location marker screenshot -->

### Auto Screenshot Detection

**Your location updates automatically every time you take a screenshot.**

Uses `FileSystemWatcher` to monitor the screenshot folder in real time. When a new screenshot is created, coordinates are automatically parsed and the location is refreshed — no need to click `Force Run` each time.

<!-- TODO: Add auto screenshot detection screenshot -->

### Auto Map Detection

**The map switches automatically when you enter a raid.**

Periodically monitors EFT game log files to detect which map you've loaded into. When you enter a raid, the tool automatically switches to the correct map without any manual action.

<!-- TODO: Add auto map detection screenshot -->

### Multi-Floor Auto Detection

**Automatically determines your current floor based on Z-coordinate.**

On multi-floor maps like Reserve, Factory, and Streets, the tool analyzes the Z-coordinate (height) from the screenshot to automatically switch between floors (e.g., ground / underground). Manual switching via `Ctrl+Numpad` hotkeys is also available.

<!-- TODO: Add floor detection screenshot -->

### Direction Indicator

**See which way you're facing with an arrow overlay.**

Converts the quaternion rotation data from the screenshot filename into Euler angles, then renders an SVG triangle arrow on the map showing the player's facing direction.

<!-- TODO: Add direction indicator screenshot -->

### Deadzone Auto-Panning

**The map automatically scrolls to keep your marker in view.**

When the location marker reaches the edge of the viewport (deadzone), the map automatically pans to keep the marker visible on screen. The deadzone percentage is configurable in settings.

<!-- TODO: Add deadzone panning screenshot -->

### Quest Tracking

**Selected quests are saved and restored per map.**

Quest selections made in the tarkov-market.com quest panel are stored in a SQLite database per map. The next time you open the same map, your previously selected quests are automatically restored.

<!-- TODO: Add quest tracking screenshot -->

### Floor Zone Editor

**Visually edit polygon-based floor zones on the map.**

Click on the map to define polygonal floor zones. Holes (exclusion areas) are supported for complex multi-level buildings. Results are saved to `floor_db.json` for precise floor detection.

<!-- TODO: Add floor zone editor screenshot -->

## Supported Maps

| # | Map Name | Internal Name |
|---|----------|---------------|
| 1 | Ground Zero | `ground-zero` |
| 2 | Factory | `factory` |
| 3 | Customs | `customs` |
| 4 | Interchange | `interchange` |
| 5 | Woods | `woods` |
| 6 | Shoreline | `shoreline` |
| 7 | Lighthouse | `lighthouse` |
| 8 | Reserve | `reserve` |
| 9 | Streets of Tarkov | `streets` |
| 10 | The Lab | `lab` |
| 11 | Labyrinth | `labyrinth` |
| 12 | Terminal | `terminal` |

## System Requirements

| Component | Requirement |
|-----------|-------------|
| OS | Windows 10 1809+ / Windows 11 |
| Runtime | [.NET 8.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0) |
| Browser | [WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) (pre-installed on most systems) |
| Network | Internet connection required (connects to tarkov-market.com) |
| Disk | ~50 MB |

## Download & Installation

### Download

Download the latest `.zip` file from [Releases](https://github.com/karpitony/eft-where-am-i/releases/latest).

1. Download the `.zip` file.
2. Extract it to your preferred location.
3. Run `eft-where-am-i.exe`.

<img src="assets/eft-wmi-exe.png" width="700">

### Windows Security Warning

Windows SmartScreen may display a warning. Click `More info` → `Run anyway`. The program contains no viruses — you can verify with the [VirusTotal scan results](https://www.virustotal.com/gui/file/4aa4768640a4c29ddc42ad1bc736d70c98630149985477e153bdae93aa91f010/detection).

### First Launch

On first launch, the tool automatically detects your EFT screenshot and log folder paths. If the detected paths are incorrect, you can change them manually in Settings or use the `Auto Find` button.

## How to Use

### Basic Usage

**1. Select a map.**

Choose a map from the top-left dropdown. If Auto Map Detection is enabled, the map switches automatically when you enter a raid.

<img src="assets/screenshot01.png" width="700">

**2. Take a screenshot during a raid.**

The default EFT screenshot key is `PrtSc`.

**3. Check your position on the map.**

A red dot and direction arrow will show your current location and facing direction.

**4. With Auto Screenshot Detection enabled, your position updates automatically.**

No need to click `Force Run` — just take screenshots and see your location in real time.

### Advanced Features

#### Floor Switching Hotkeys

When Escape From Tarkov is the active window, use the following hotkeys to manually switch floors:

| Hotkey | Action |
|--------|--------|
| `Ctrl + Numpad 0` | Basement / Bunker |
| `Ctrl + Numpad 1` | Ground Floor |
| `Ctrl + Numpad 2` | Level 2 |
| `Ctrl + Numpad 3` | Level 3 |
| `Ctrl + Numpad 4` | Level 4 |
| `Ctrl + Numpad 5` | Level 5 |

#### Deadzone Configuration

Adjust the deadzone percentage (default: 93%) in Settings to control how close the marker can get to the viewport edge before auto-panning triggers.

#### Floor Zone Editor

1. Click the `Floor Zone Editor` button in Settings.
2. Click on the map to add polygon vertices.
3. Configure floor name and Z-range.
4. Save — changes are written to `floor_db.json`.

Community contributions of floor zone data are welcome!

## Settings

<img src="assets/screenshot02.png" width="700">

| Setting | Description |
|---------|-------------|
| Language | Korean / English |
| Screenshot Path | Path to EFT screenshot folder (`Change` / `Auto Find`) |
| Log Path | Path to EFT game log folder |
| Auto Screenshot Detection | Automatically update location when screenshots are taken |
| Auto Map Detection | Automatically switch maps when entering a raid |
| Auto Panning | Deadzone-based automatic map scrolling |
| Floor Zone Editor | Edit floor zones for multi-level maps |

## Contributing

PRs and Issues are always welcome!

1. Fork this repository.
2. Create a feature branch. (`git checkout -b feature/my-feature`)
3. Commit your changes.
4. Open a Pull Request.

Contributions to `floor_db.json` with floor data for new maps are highly appreciated.

## Bug Reports & Suggestions

Please report bugs, suggest features, or ask questions via [Issues](https://github.com/karpitony/eft-where-am-i/issues).

## Credits & Acknowledgments

- [Tarkov-Market](https://tarkov-market.com/) — Map data and UI
- [Tarkov-Client](https://github.com/byeong1/Tarkov-Client) by byeong1 — Direction indicator code (MIT License)
- [Freepik - Flaticon](https://www.flaticon.com/free-icons/map) — Map icon
- [Microsoft WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) — Browser component

## License

MIT License — Copyright (c) 2024 karpitony

See the [LICENSE](./LICENSE) file for details.

> [!CAUTION]
> This program is not affiliated with BSG/Battlestate Games. The developer assumes no responsibility for any in-game penalties, bans, or other consequences resulting from its use. Use at your own risk.
