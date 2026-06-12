# Dungeon Siege

Dungeon Siege is a 2D top down action game developed in C# utilizing the Windows Forms framework. Built as an object oriented programming project, it implements fundamental software engineering principles to deliver a clean architecture and responsive gameplay loop.

## Overview

In Dungeon Siege, players control a hero navigating an arena infested with waves of monsters. The objective is to defeat a set number of enemies to achieve victory. The game features multiple enemy types with distinct behaviors, dynamic difficulty levels, and viewport adaptability to ensure a consistent experience across different monitors.

## Core Features

### Interactive Main Menu
The game starts with a main menu overlay where players can select their difficulty level, start a new game, resume an active session, or exit the application.

### Scalable Difficulty Settings
Players can choose between Easy, Medium, and Hard modes. The selected difficulty dynamically alters enemy spawn rates, movement speed, damage scaling, and the total kill count required for victory.

### Directional Combat
The game features responsive movement controls combined with directional firing. Projectiles are shot in the direction of the player's last movement vector, allowing for strategic positioning and combat.

### Dynamic Viewport Adaptability
The interface initiates in a maximized window and dynamically scales all playable bounds, walls, and UI HUD readouts based on the host monitor's client dimensions.

## Project Structure

### Core Systems
The core architecture is managed by the game manager class which controls the game loop, keeps track of player stats, manages enemy spawns, and handles collisions. Texture assets are preloaded at startup by the texture manager to optimize memory usage.

### Entity System
All active characters inherit from a base entity class. This includes the player, which utilizes custom projectile managers for combat, and various enemy subclasses such as goblins, dark mages, and orcs. Each enemy type has its own distinct levels, health parameters, and movement speed scaling.

### Component Design
Reusable visual elements, including health bars and shields, are implemented as modular components that automatically synchronize and draw themselves relative to their parent entities.

## Requirements and Execution

This project is built on the .NET 10 framework for Windows.

To build the project from the command line, run:
dotnet build

To launch the compiled game, run:
dotnet run
