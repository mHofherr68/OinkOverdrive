# OinkOverdrive

## Project Overview
OinkOverdrive is a minimalistic two-player arcade shooter developed from a top-down perspective.
The project focuses on the technical implementation of online multiplayer functionality using Unity and FishNet.

Two players compete against each other by shooting projectiles and reducing the opponent’s score.
The boss is not yet available, but is planned.
---

## Technology Stack
- Unity 6000.0.62f1
- FishNet (Networking Framework)
- Unity Input System
- TextMeshPro

---

## Multiplayer Functionality
- Online multiplayer with Host and Client
- Server-authoritative architecture
- Automatic Host/Client startup in Unity Multiplayer Play Mode
- Players implemented as NetworkObjects with correct ownership handling

---

## Controls
- Movement: W / S (vertical movement only)
- Shooting: Spacebar (continuous fire)

---

## Gameplay Mechanics
- Each player starts with an initial score of 300
- Hitting the opponent reduces their score to -10
- When a player's score reaches zero, a Game Over state is triggered
- A Game Over overlay is displayed
- Player input and shooting are disabled after Game Over.

---

## Projectile System
- Network-synchronized projectiles
- Server-side projectile spawning
- Two bullet patterns:
  - Standard single-shot
  - Spread-shot (unlocked after reaching 300)

---

## Level Elements
- Movement boundaries implemented using colliders
- Static cover elements that destroy projectiles on impact

---

## Project Structure
- Assets/Scripts – Gameplay and networking logic
- Assets/Prefabs – Players, projectiles, covers
- Assets/Scenes – Main game scene

---

## Notes
This project is intentionally kept minimalistic.
The primary focus is on correct multiplayer synchronization, server authority,
and a clear game flow rather than visual complexity.


I hope to get an extension due to illness. However, if a new PIP phase has already started, or if one begins on January 26, 2026, I cannot run two projects at the same time. Thank you!
