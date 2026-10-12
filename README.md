# 💣 MineGrid: Pixel Minesweeper

<p align="center">
  <strong>A tiny pixel world. A grid full of danger.</strong><br>
  Read the numbers, trust your logic, and clear every safe tile.
</p>

<p align="center">
  <img alt="Unity 6" src="https://img.shields.io/badge/Unity-6000.3.5f2-222222?logo=unity&logoColor=white">
  <img alt="C Sharp" src="https://img.shields.io/badge/Made%20with-C%23-68217A?logo=csharp&logoColor=white">
  <img alt="Genre" src="https://img.shields.io/badge/Genre-Puzzle%20%2F%20Logic-65B86E">
  <img alt="Status" src="https://img.shields.io/badge/Status-In%20Development-F2C14E">
</p>

---

**MineGrid** is a pixel-inspired take on the classic Minesweeper. Every number is a clue, every reveal is a choice, and one wrong move can end the run. Pick a difficulty, study the grid, and uncover every safe cell.

> Created by **Binary Suns**.

## ✨ Features

- Three board sizes, from a quick warm-up to a wide, mine-filled challenge.
- Randomly placed mines and automatically calculated neighboring-mine clues.
- Automatic expansion across connected empty areas.
- Win and loss screens with a quick retry or a route back to the menu.
- Live game timer and click counter, with a final run summary.
- Responsive board framing and safe-area handling in the Unity UI.
- Pixel-style visuals and typography.

## 🎮 Difficulty

| Mode | Board | Mines |
|:--|:--:|--:|
| Easy | 9 × 9 | 10 |
| Normal | 16 × 16 | 40 |
| Hard | 30 × 16 | 99 |

## 🕹️ How to play

1. Choose a difficulty from the main menu and start a game.
2. Click a covered tile to reveal it.
3. A number tells you how many mines touch that tile, including diagonals.
4. Revealing an empty area opens connected safe tiles automatically.
5. Reveal every safe tile to win. Reveal a mine and the run is over.

The timer starts with your first valid tile click. Your time and click count are shown during the game and on the result screen.

## 🧰 Open the Unity project

### Requirements

- [Unity Hub](https://unity.com/download)
- Unity Editor **6000.3.5f2**

### Get started

1. Clone or download this repository.
2. In Unity Hub, choose **Add project from disk**.
3. Select the `Buscaminas/` folder inside the repository.
4. Open the project with Unity **6000.3.5f2**.
5. Open `Assets/Scenes/Game.unity` if it is not already open.
6. Press **Play**.

Unity will resolve the packages declared in `Packages/manifest.json` when the project opens.

## 🗂️ Project map

```text
Buscaminas/
├── Assets/
│   ├── Prefabs/       # Reusable cell prefab
│   ├── Scenes/        # Main game scene
│   ├── Scripts/       # Gameplay rules and Unity presentation
│   └── Sprites/       # Pixel-style visual assets
├── Packages/          # Unity package manifest and lock file
└── ProjectSettings/   # Project and editor settings
```

### Gameplay code

- `MinesweeperBoard` and `BoardCell` hold the board rules and cell state.
- `GridHelper` builds the visual grid, creates a mine layout, and connects input to the board.
- `GameManager` coordinates menu, gameplay, restart, victory, and defeat states.
- `GameHud` displays run statistics and keeps the board framed for the available screen area.
- `DifficultySettings` defines the dimensions and mine count for each mode.

## 📦 Builds

This repository contains the Unity project source. It does not currently include ready-to-download game builds. When releases are published, platform-specific downloads will be linked here.

## 🛠️ Built with

- Unity **6000.3.5f2**
- C#
- Unity UI (uGUI)

---

<p align="center">
  <sub>Think carefully. Reveal bravely. Good luck out there. 💥</sub>
</p>
