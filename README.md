# Dungeon Siege

Dungeon Siege is a premier two-dimensional top-down action game developed in C# utilizing the Windows Forms framework. Engineered as a comprehensive object-oriented programming project, it implements fundamental software architecture principles to deliver a pristine codebase and a highly responsive gameplay experience. The project serves as both a fully functional entertainment application and an academic demonstration of advanced structural design.

## Project Overview

In Dungeon Siege, players command a heroic protagonist navigating a perilous arena continuously invaded by monstrous adversaries. The primary objective is to vanquish a required quota of enemies to secure victory. The application boasts diverse enemy archetypes with distinct behavioral patterns, dynamic difficulty scaling, and seamless viewport adaptability to ensure an impeccable user experience across varying display resolutions. It combines reflex-based combat with strategic positioning in an enclosed environment.

## Object-Oriented Programming Principles Applied

This software was meticulously constructed to demonstrate core object-oriented paradigms. The architecture heavily relies on these methodologies to ensure scalability and maintainability.

### Inheritance
The entity hierarchy is deeply rooted in inheritance. A singular base entity class defines universal characteristics such as health pools, spatial coordinates, rendering boundaries, and movement velocities. Specialized characters, including the player and distinct enemy species, inherit these fundamental traits while overriding specific behaviors to suit their unique roles.

### Polymorphism
Polymorphism is extensively utilized within the combat and rendering loops. The core game manager processes collections of generic enemy objects, uniformly executing their overridden attack and movement algorithms. This allows the system to seamlessly interact with varied adversaries, from heavily armored melee units to ranged spellcasters, without requiring rigid conditional type-checking.

### Encapsulation
Data integrity is strictly enforced through encapsulation. Internal logic, such as shield regeneration variables, projectile boundary mathematics, and health bar synchronization, is concealed within specific components. External classes communicate exclusively through securely defined public properties and methods, preventing unintended interference with internal game states.

### Composition
Complex entities are constructed using structural composition. Rather than hardcoding visual indicators into the player or enemy classes, independent modular components, such as health and shield bars, are instantiated as standalone properties within the entities. This approach promotes high modularity and code reusability.

## Core Gameplay Features

### Interactive Main Menu
The application initializes with an elegant main menu overlay. Players can seamlessly select their preferred difficulty tier, commence a new session, resume an ongoing engagement, or exit the program gracefully.

### Scalable Difficulty Configurations
Users can select between Easy, Medium, and Hard operational modes. The chosen difficulty tier dynamically calibrates enemy spawn frequencies, adversary movement velocity, damage output multipliers, and the total elimination quota required for a triumphant conclusion.

### Directional Combat Mechanics
The combat system integrates highly responsive movement controls with directional firing capabilities. Projectiles are discharged strictly in the direction of the player's most recent movement vector, facilitating advanced tactical positioning and fluid combat maneuvering.

### Dynamic Viewport Adaptability
The graphical interface launches in a maximized window state by default. It intelligently calculates and scales all playable boundaries, environmental barriers, and Heads-Up Display readouts based exclusively on the host machine's client dimensions.

## Player Controls

The application requires a keyboard for movement and combat initiation.

* W or Up Arrow: Ascend vertically
* S or Down Arrow: Descend vertically
* A or Left Arrow: Traverse horizontally left
* D or Right Arrow: Traverse horizontally right
* Spacebar: Discharge primary weapon projectile
* Shift: Activate special defensive ability
* Escape: Pause the application and summon the main menu

## Architectural Structure

### Core Systems Integration
The overarching architecture is governed by a robust game manager class. This component oversees the primary gameplay loop, monitors player statistics, regulates enemy spawn intervals, and processes complex collision algorithms. Furthermore, graphical assets are preloaded at application startup by a dedicated texture manager to drastically optimize memory allocation and rendering performance.

### Polymorphic Entity System
All active on-screen characters inherit from a unified base entity class. This encompasses the player character, which leverages custom projectile managers for offensive capabilities, alongside various enemy subclasses including goblins, dark mages, and orcs. Each enemy archetype encapsulates its own distinct level parameters, health capacities, and movement speed calibrations.

### Modular Component Design
Reusable visual elements, specifically health and shield indicators, are structured as independent modular components. These elements automatically synchronize state data and render themselves proportionally relative to their parent entities.

## System Requirements and Execution

This software requires the .NET 10 framework for Windows.

To compile the application from a terminal environment, execute the following instruction:

```
dotnet build
```

To initialize the compiled executable, run:

```
dotnet run
```

## Author

Rehan Ilyas
