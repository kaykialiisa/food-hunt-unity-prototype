# Food Hunt – Unity Prototype

A 3D mobile puzzle prototype built in Unity, inspired by *Food Hunt: Pixel Puzzle*. I started this project for my game development portfolio to explore how a single tap can lead to meaningful decisions about color, move order, and limited space.

## Gameplay concept

Tap an ant box to send it to a free slot. Its ants automatically collect matching pixels they can reach from the outside of the picture. A box frees its slot once it reaches its collection target.

Clearing pixels can expose new colors, allowing waiting boxes to resume collecting. The goal is to clear the entire picture. The player loses if all slots are occupied and no box can make further progress.

## Technology

- **Unity 6 (`6000.3.28f1`)** — game engine
- **Universal Render Pipeline (URP)** — 3D rendering
- **Unity Input System** — configured for project input
- **Unity CLI and Unity Pipeline** — editor automation and verification

## Project status

The Unity project, `Game` scene, and core rules are in place. Gameplay implementation in C# is the next milestone. The full rules are documented in [PROTOTYPE.md](./PROTOTYPE.md).

## Open in Unity

Open the repository root in Unity Hub with Editor version `6000.3.28f1`, then open `Assets/Project/Scenes/Game.unity`.
