# The Forgotten Temple

**A 3D temple exploration and puzzle adventure built with Unity.**

Explore an ancient temple, discover environmental clues, solve its mechanisms, claim the Crown of Eternity, and find a way out.

> **Project status:** Game prototype  
> **Engine:** Unity 2022.3 LTS  
> **Target platform:** Windows desktop

---

## Table of Contents

- [About the Game](#about-the-game)
- [Gameplay Highlights](#gameplay-highlights)
- [Controls](#controls)
- [Download and Installation](#download-and-installation)
- [Running the Windows Build](#running-the-windows-build)
- [Building from Source](#building-from-source)
- [Project Structure](#project-structure)
- [Troubleshooting](#troubleshooting)
- [Credits and Third-Party Assets](#credits-and-third-party-assets)
- [License](#license)

## About the Game

*The Forgotten Temple* is a 3D exploration game set inside an ancient stone temple. Players explore connected chambers, interpret visual clues, interact with objects, and solve puzzle mechanisms to progress through the temple.

The central objective is to obtain the **Crown of Eternity** and reach the exit. The experience focuses on exploration, environmental storytelling, puzzle-solving, and a tense escape sequence.

## Gameplay Highlights

- **Temple exploration:** Navigate a series of atmospheric stone chambers and corridors.
- **Object interaction:** Pick up, carry, place, and drop objects as required by the puzzles.
- **Colour clue puzzle:** Use the `PRB` clue to identify the Purple, Red, and Blue objects.
- **Lever sequence puzzle:** Interpret the `101` clue and activate the levers in the correct pattern.
- **Crown objective:** Find and collect the Crown of Eternity.
- **Escape challenge:** Progress towards the temple exit and complete the final escape objective.

*The feature list describes the intended/current prototype experience. Exact content may differ slightly between development builds.*

## Game Visuals

<img width="1119" height="523" alt="image" src="https://github.com/user-attachments/assets/484eb52e-ab24-47dc-9098-4dd88590ff3a" />

<img width="1091" height="537" alt="image" src="https://github.com/user-attachments/assets/a99cab02-e875-4599-b8de-ac43579a9bd5" />

<img width="1026" height="518" alt="image" src="https://github.com/user-attachments/assets/4b551a1c-d9c7-462f-940a-06a2fbc9d3ab" />

<img width="1102" height="535" alt="image" src="https://github.com/user-attachments/assets/40175fd6-afb1-466c-b1c4-ad5bd061ecad" />

## Controls

| Action | Control |
|---|---|
| Move | `W` `A` `S` `D` |
| Look around | Mouse |
| Interact / pick up / place | `E` |
| Drop held object | `Q` |
| Pause | `Esc` |

Controls may vary if they have been changed in a particular build. The in-game controls screen, if available, takes precedence.

## Download and Installation

The Windows build is distributed as a complete folder, usually packaged as a `.zip` file.

1. Download the Windows build archive from the project's release or submission link.
2. Extract the **entire archive** to a folder on your computer.
3. Open the extracted game folder.
4. Run `The Forgotten Temple.exe`.

**Important:** Do not move or copy only the `.exe` file. Unity builds rely on supporting files and folders that must remain beside the executable.

A typical Windows build folder looks like this:

```text
The-Forgotten-Temple-Windows/
├── The Forgotten Temple.exe
├── The Forgotten Temple_Data/
├── UnityPlayer.dll                 
├── GameAssembly.dll                
├── MonoBleedingEdge/               
└── other Unity-generated files    
```

## Running the Windows Build

- Use a supported 64-bit Windows computer.
- Extract the build before launching it; do not try to run the executable from inside the compressed `.zip` archive.
- Double-click `The Forgotten Temple.exe` to start the game.
- For the best experience, close unnecessary applications if your computer is running low on memory.

## Building from Source

### Requirements

- Unity **2022.3 LTS**
- Unity Hub
- A Windows development environment for creating the Windows standalone build

### Open the project

1. Clone or download the source repository.
2. Open Unity Hub.
3. Select **Add** and choose the project folder (the folder containing `Assets`, `Packages`, and `ProjectSettings`).
4. Open the project using Unity 2022.3 LTS.
5. Wait for Unity to finish importing assets and compiling scripts.                             

## Troubleshooting

| Issue | What to check |
|---|---|
| The game does not launch | Extract the complete build folder and try running the `.exe` again. |
| A missing data or DLL error appears | Restore the complete original Unity build output. Do not use the `.exe` by itself. |
| The game opens but shows a black screen | Confirm that the required scenes were included in the build and that the correct starting scene is first. |
| Controls do not respond as expected | Check the controls listed above and confirm that the game window has focus. |

