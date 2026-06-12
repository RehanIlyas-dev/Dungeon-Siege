# Dungeon Siege — Complete Game Guide

This document explains **every class and every function** in the project so you can fully understand how the game works.

---

## Table of Contents

1. [How the Game Works (Big Picture)](#how-the-game-works-big-picture)
2. [Class Hierarchy](#class-hierarchy)
3. [Controls](#controls)
4. [Difficulty Settings](#difficulty-settings)
5. [Enums](#enums)
6. [Entry Point](#entry-point)
7. [UI Layer — Form1](#ui-layer--form1)
8. [Game Logic — GameManger](#game-logic--gamemanger)
9. [Entity System](#entity-system)
10. [Enemy Types](#enemy-types)
11. [Components](#components)
12. [Core Utilities](#core-utilities)
13. [Interface — IDamageable](#interface--idamageable)
14. [Game Loop Flow](#game-loop-flow)

---

## How the Game Works (Big Picture)

Dungeon Siege is a **top-down action game** built with Windows Forms (C#).

1. The player spawns in the center of a walled dungeon.
2. Enemies appear on the edges of the playable area.
3. The player moves with **WASD**, shoots with **Space**, melee attacks with **mouse click**, and restores shield with **Shift**.
4. Kill enough enemies to **win**, or die to **lose**.
5. Three difficulty levels change enemy speed, damage, spawn rate, and kill goal.

**Architecture in one sentence:** `Form1` handles input and drawing → `GameManger` runs game rules → `Player` and `Enemy` subclasses handle character behaviour → `Components` (`HealthBar`, `ProjectileManager`) handle reusable parts.

---

## Class Hierarchy

```
IDamageable (interface)
    └── Entity (abstract)
            ├── Player
            └── Enemy (abstract)
                    ├── Goblin
                    ├── Orc
                    └── DarkMage

Form1 ──has──► GameManger ──has──► Player + List<Enemy>
Player / DarkMage ──has──► ProjectileManager
Entity ──has──► HealthBar
```

---

## Controls

| Input | Action |
|---|---|
| W / A / S / D or Arrow Keys | Move player |
| Space | Fire projectile in facing direction |
| Shift | Restore 30 shield (costs 30 stamina, 45-frame cooldown) |
| Mouse Click | Melee attack closest enemy (within 80px) |
| Esc (in game) | Return to main menu |
| Esc (in menu) | Resume game |
| R (on game over) | Restart |

---

## Difficulty Settings

| Setting | Easy | Medium | Hard |
|---|---|---|---|
| Kills to win | 5 | 10 | 15 |
| Max enemies on screen | 5 | 8 | 12 |
| Delay before 2nd enemy | ~3 sec | ~1.5 sec | ~0.75 sec |
| Spawn interval after that | ~2.5 sec | ~1.2 sec | ~0.6 sec |
| Enemy speed | 1 | 1–2 | 2–3 |
| Mage projectile damage | ÷2 | normal | ×2 |

---

## Enums

### `GameDifficulty` — `Core/GameManger.cs`

| Value | Meaning |
|---|---|
| `Easy` | Slower enemies, fewer kills needed, less damage |
| `Medium` | Default balanced mode |
| `Hard` | Faster enemies, more kills, more damage |

### `GameState` — `Form1.cs`

| Value | Meaning |
|---|---|
| `MainMenu` | Title screen with buttons |
| `Playing` | Active gameplay |
| `GameOver` | Win or lose screen overlay |

---

## Entry Point

### `Program` — `Program.cs`

The application starts here.

| Function | What it does |
|---|---|
| `Main()` | Initializes Windows Forms and launches `Form1`. This is the first code that runs when you start the game. |

---

## UI Layer — Form1

### `Form1` — `Form1.cs`

The main game window. Handles **input**, **drawing**, and **screen states** (menu / playing / game over).

#### Fields

| Field | Purpose |
|---|---|
| `gameManager` | Owns all game logic (player, enemies, rules) |
| `gameTimer` | Fires every 16ms (~60 FPS) to update and redraw |
| `currentState` | Current screen: menu, playing, or game over |
| `hasActiveGame` | `true` after first New Game — enables Continue button |

#### Properties

| Property | What it does |
|---|---|
| `ScreenBounds` | Returns a `Rectangle(0, 0, width, height)` for the current window size |

#### Functions

| Function | What it does |
|---|---|
| `Form1()` (constructor) | Sets window to 800×600, loads textures, creates player and game manager, starts the timer |
| `GameTimer_Tick()` | Called every frame. If playing, runs `UpdateGame()`. Checks for game over. Calls `Invalidate()` to redraw |
| `Form1_KeyDown()` | Handles key presses: Esc (menu), R (restart), WASD (movement flags), Shift (ability), Space (fire) |
| `Form1_KeyUp()` | Sets movement flags to `false` when keys are released |
| `Form1_MouseDown()` | Menu: handles button clicks. Playing: melee attacks closest enemy |
| `HandleMenuClick()` | Checks which menu button was clicked: Difficulty, Continue, New Game, or Exit |
| `StartPlaying()` | Calls `gameManager.StartGame()`, sets `hasActiveGame = true`, switches to Playing state |
| `Form1_Paint()` | Draws everything on screen based on current state (menu or gameplay) |
| `DrawBackground()` | Tiles floor/preview image across the screen, or fills gray if no image |
| `DrawWalls()` | Draws wall images on all four edges of the screen |
| `DrawHud()` | Draws Health, Shield, Stamina, and Kills text at the top |
| `DrawGameOverOverlay()` | Dark overlay with "GAME OVER" or "VICTORY" message |
| `DrawButton()` | Draws a menu button (red if enabled, gray if disabled) with centered text |

---

## Game Logic — GameManger

### `GameManger` — `Core/GameManger.cs`

The **brain of the game**. Updates player, enemies, projectiles, spawning, and win/lose conditions.

> Note: class name is spelled `GameManger` (typo from original project).

#### Fields

| Field | Purpose |
|---|---|
| `player` | The player character |
| `enemies` | List of all active enemies |
| `KillCount` | Current kills (synced from player) |
| `KillLimit` | Kills needed to win |
| `IsGameOver` | `true` when player dies or wins |
| `CurrentDifficulty` | Easy / Medium / Hard |
| `rand` | Random number generator for spawning |
| `spawnTimer` | Counts frames until next enemy spawn |
| `waitingForFirstSpawn` | `true` until the 2nd enemy appears (uses longer initial delay) |

#### Functions

| Function | What it does |
|---|---|
| `GameManger()` (constructor) | Stores player, enemy list, and initial kill limit |
| `StartGame(screenBounds)` | Resets player stats, clears enemies, sets kill limit by difficulty, spawns 1 enemy |
| `UpdateGame(bounds)` | **Main game tick.** Moves player, updates all enemies, checks collisions, spawns new enemies |
| `SpawnEnemy(screenBounds)` | Creates a random enemy (Goblin/Orc/DarkMage) at a random edge position |
| `GetSpawnPosition(screenBounds)` | Picks a random point on top, bottom, left, or right edge of playable area |
| `CheckPlayerProjectiles()` | Tests player bullets against enemies. On hit: damage enemy, count kill, remove bullet |
| `CheckEnemyProjectiles()` | Tests DarkMage bullets against player. Damage scaled by difficulty |
| `ApplyDamage(target, damage)` | Calls `TakeDamage()` on any `IDamageable` (player or enemy) |
| `IsGameOverCheck()` | Sets `IsGameOver = true` if player health ≤ 0 or kills ≥ kill limit |
| `GetMaxConcurrentEnemies()` | Max enemies allowed on screen (5 / 8 / 12) |
| `GetInitialSpawnDelay()` | Frames before 2nd enemy spawns (180 / 90 / 45) |
| `GetSpawnThreshold()` | Frames between later spawns (150 / 70 / 35) |
| `GetOrcArmor()` | Orc armor value by difficulty (3 / 5 / 8) |
| `GetEnemySpeed(type)` | Movement speed by enemy type and difficulty |
| `GetScaledDamage(values)` | Returns easy/medium/hard damage from a tuple |

---

## Entity System

### `Entity` — `Entities/Entity.cs`

**Abstract base class** for all characters. Implements `IDamageable`.

#### Properties

| Property | Purpose |
|---|---|
| `Health` | Current HP (0 = dead) |
| `Position` | Top-left corner on screen (Point) |
| `Speed` | Pixels moved per frame |
| `Sprite` | Character image (or null for colored shape) |
| `healthBar` | Composed `HealthBar` component for drawing HP |

#### Functions

| Function | What it does |
|---|---|
| `Entity()` (constructor) | Sets health, position, speed, sprite. Creates green health bar |
| `TakeDamage(amount)` | Reduces health, clamps to 0, updates health bar |
| `TakeDamage(amount, type)` | Overloaded version — currently just calls `TakeDamage(amount)` |
| `IsAlive()` | Returns `true` if Health > 0 |
| `Move(target)` | **Abstract** — each subclass defines its own movement |
| `SpecialAbility()` | **Abstract** — each subclass defines its own ability |
| `Draw(g)` | **Abstract** — each subclass draws itself |

---

### `Player` — `Entities/Player.cs`

The **human-controlled character**.

#### Properties

| Property | Purpose |
|---|---|
| `Kills` | Number of enemies killed this round |
| `Shields` | Shield points (absorb damage before health) |
| `Stamina` | Used for special ability; regenerates over time |
| `MoveUp/Down/Left/Right` | Set by keyboard — direction flags |
| `ProjectileManager` | Composed component that owns player's bullets |

#### Functions

| Function | What it does |
|---|---|
| `Player()` (constructor) | Starts with 100 shield, 100 stamina, creates blue shield bar |
| `ResetStats()` | Resets health, shield, stamina, kills to starting values (on new game) |
| `SyncShieldBar()` | Updates shield bar UI after shield value changes |
| `ResetMovement()` | Clears all movement flags (when pressing Esc) |
| `TakeDamage(amount)` | **Shield first:** reduces shields, overflow hits health. Updates shield bar |
| `Fire()` | Creates a projectile in last movement direction. 8-frame cooldown |
| `Attack(target)` | Melee hit for 25 damage if enemy within 80px. Counts kill if enemy dies |
| `Move(target, bounds)` | **Player movement:** reads WASD flags, moves, clamps to walls, updates projectiles, regens stamina |
| `Move(target)` | Empty override (player uses the `Move(target, bounds)` overload instead) |
| `SpecialAbility()` | Spends 30 stamina to restore 30 shield. 45-frame cooldown |
| `Draw(g)` | Draws projectiles, sprite, health bar, and shield bar |

---

### `Enemy` — `Entities/Enemy.cs`

**Abstract base class** for all enemies. Adds combat stats and shared behaviour.

#### Properties

| Property | Purpose |
|---|---|
| `Damage` | Damage dealt per attack |
| `DetectionRange` | How far enemy can "see" player (used by DarkMage) |
| `KillValue` | Score value (reserved, not used in UI yet) |
| `MaxHealth` | Starting health (for health bar scaling) |
| `Level` | Display level shown above enemy |
| `meleeAttackCooldown` | Frames until next melee hit allowed |
| `arenaWidth/Height` | Screen size for projectile bounds |
| `playableBounds` | Area enemies are clamped inside |

#### Functions

| Function | What it does |
|---|---|
| `Enemy()` (constructor) | Sets combat stats and configures smaller enemy health bar |
| `SetArenaSize(width, height)` | Updates screen dimensions (called each frame) |
| `SetPlayableBounds(bounds)` | Sets the rectangle enemies cannot leave |
| `ClampToPlayableBounds()` | Keeps enemy position inside playable area |
| `ChaseTarget(target, stopDistance)` | Moves toward player at `Speed`. Stops when within 10px by default |
| `Attack(target)` | Melee attack if within 50px. Uses 45-frame cooldown between hits |
| `Draw(g)` | Draws sprite, health bar, and "Lvl X" label |

---

## Enemy Types

### `Goblin` — `Entities/Goblin.cs`

**Level 1** — Fast, weak, can steal from player.

| Property | Value |
|---|---|
| Health | 60 |
| StealChance | 30% on melee hit |

| Function | What it does |
|---|---|
| `Goblin()` (constructor) | Sets Level 1, base stats |
| `Move(target)` | Chases player using `ChaseTarget()` |
| `Attack(target)` | Melee attack + 30% chance to trigger steal ability |
| `SpecialAbility()` | Steals 15 stamina and 10 shield from player |

---

### `Orc` — `Entities/Orc.cs`

**Level 3** — Tanky, armored, enrages at low health.

| Property | Value |
|---|---|
| Health | 120 |
| Armor | 3 / 5 / 8 (by difficulty) |

| Function | What it does |
|---|---|
| `Orc()` (constructor) | Sets Level 3 and armor value |
| `TakeDamage(amount)` | Reduces damage by armor (minimum 1). Triggers enrage below 50% HP |
| `Move(target)` | Chases player using `ChaseTarget()` |
| `SpecialAbility()` | **Enrage (once):** +1 speed, +2 armor |

---

### `DarkMage` — `Entities/DarkMage.cs`

**Level 2** — Ranged enemy that shoots projectiles and kites the player.

| Property | Value |
|---|---|
| Health | 80 |
| Mana | 100 (used to cast spells) |
| ProjectileManager | Composed component for mage bullets |

| Function | What it does |
|---|---|
| `DarkMage()` (constructor) | Sets Level 2, full mana |
| `Move(target)` | Updates projectiles. Moves closer if far (>250px), moves away if too close (<150px). Slowly regens mana |
| `Attack(target)` | If in range: fires normal shot (25 mana) or special shot (60 mana). Has attack cooldown |
| `SpecialAbility()` | Fires a powerful projectile at the player (costs 60 mana) |
| `Draw(g)` | Draws base enemy + all active projectiles |
| `FireProjectile(dx, dy, distance)` | Creates a projectile aimed at the target direction |

---

## Components

### `HealthBar` — `Components/HealthBar.cs`

Reusable **HP/shield bar** drawn above entities. Used via composition (not inheritance).

| Property | Purpose |
|---|---|
| `MaxValue` | Full bar value |
| `CurrentValue` | Current fill amount |
| `BarWidth/BarHeight` | Size of the bar in pixels |
| `YOffset` | How far above the entity to draw |
| `FillBrush` | Color of filled portion (green, blue, etc.) |
| `BackgroundBrush` | Color behind the bar (red, transparent) |

| Function | What it does |
|---|---|
| `HealthBar()` (constructor) | Sets size, colors, and max value |
| `Sync(currentValue)` | Updates current value before drawing |
| `Draw(g, entityX, entityY)` | Draws background bar, then filled portion based on current/max ratio |

---

### `ProjectileManager` — `Components/ProjectileManager.cs`

Manages a **list of projectiles** for Player and DarkMage.

| Property | Purpose |
|---|---|
| `All` | Read-only list of all active projectiles |

| Function | What it does |
|---|---|
| `Add(projectile)` | Adds a new projectile to the list |
| `RemoveAt(index)` | Removes projectile at index (after hit or out of bounds) |
| `Clear()` | Removes all projectiles (on new game) |
| `Update(screenWidth, screenHeight)` | Moves every projectile; removes ones that left the screen |
| `Draw(g)` | Draws all projectiles |

---

### `Projectile` — `Core/Projectile.cs`

A single **bullet** fired by player or DarkMage.

| Property | Purpose |
|---|---|
| `Damage` | Damage dealt on hit |
| `Position` | Current location on screen |
| `Speed` | Pixels moved per frame |
| `Sprite` | Bullet image |
| `DirX, DirY` | Normalized direction vector (where bullet travels) |

| Function | What it does |
|---|---|
| `Projectile()` (constructor) | Sets damage, start position, speed, and sprite |
| `Move()` | Moves bullet along direction vector using precise float math (smoother than int) |
| `Draw(g)` | Draws sprite or red circle |
| `IsOutOfBounds(width, height)` | Returns `true` if bullet left the screen |

---

## Core Utilities

### `Utils` — `Core/Utils.cs`

Static helper methods used across the project.

| Constant | Value | Purpose |
|---|---|---|
| `EntitySize` | 40 | Width/height of every character sprite |
| `DefaultWallThickness` | 32 | Wall size if no wall image loaded |

| Function | What it does |
|---|---|
| `DistanceTo(p1, p2)` | Extension method: calculates distance between two Points |
| `GetWallThickness()` | Returns wall image width, or 32 as fallback |
| `GetPlayableBounds(width, height)` | Returns the inner rectangle where the player can move (inside walls) |

---

### `TextureManager` — `Core/TextureManager.cs`

Loads and stores **all game images** from the `Resources/` folder.

| Property | Image file |
|---|---|
| `PlayerSprite` | `player.png` |
| `FloorImage` | `floor.png` |
| `WallImage` | `wall.png` |
| `PreviewImage` | `preview.png` (menu background) |
| `ProjectileSprite` | `projectile.png` |
| `GoblinSprite` | `goblin.png` |
| `OrcSprite` | `orc.png` |
| `DarkMageSprite` | `dark_mage.png` |

| Function | What it does |
|---|---|
| `LoadAll()` | Loads all images at game start |
| `Load(path)` | Loads one image if file exists, otherwise returns `null` |

---

## Interface — IDamageable

### `IDamageable` — `Interfaces/IDamageable.cs`

A **contract** that any damageable object must follow.

| Member | Purpose |
|---|---|
| `Health` | Must have a health property |
| `TakeDamage(amount)` | Must define how damage is applied |
| `IsAlive()` | Must define what "alive" means |

**Who implements it:** `Entity` (and therefore `Player`, `Goblin`, `Orc`, `DarkMage`).

**Why it exists:** `GameManger.ApplyDamage()` can damage any entity through `IDamageable` without knowing if it's a player or enemy — this is **interface polymorphism**.

---

## Game Loop Flow

Every **16 milliseconds** (~60 FPS), this happens:

```
GameTimer_Tick()
    │
    ├─ if Playing → GameManger.UpdateGame()
    │       │
    │       ├─ IsGameOverCheck() → stop if dead or won
    │       ├─ Player.Move() → WASD movement, projectiles, stamina
    │       ├─ For each enemy:
    │       │     ├─ Remove if dead
    │       │     ├─ Set bounds
    │       │     ├─ enemy.Move(player)
    │       │     └─ enemy.Attack(player)
    │       ├─ CheckPlayerProjectiles() → bullet vs enemy hits
    │       ├─ CheckEnemyProjectiles() → mage bullet vs player hits
    │       ├─ Update KillCount
    │       └─ Maybe SpawnEnemy() if timer reached
    │
    └─ Invalidate() → Form1_Paint() redraws everything
```

### New Game Flow

```
Click "New Game"
    → StartPlaying()
        → GameManger.StartGame()
            → Reset player stats
            → Clear enemies
            → Set kill limit by difficulty
            → Spawn 1 enemy
        → currentState = Playing
```

### Win / Lose

| Condition | Result |
|---|---|
| `player.Health <= 0` | Game Over (defeat) |
| `KillCount >= KillLimit` | Victory |

---

## File Map

| File | Role |
|---|---|
| `Program.cs` | App entry point |
| `Form1.cs` | Window, input, rendering, menus |
| `Core/GameManger.cs` | Game rules, spawning, collisions |
| `Core/Projectile.cs` | Single bullet behaviour |
| `Core/Utils.cs` | Distance and bounds helpers |
| `Core/TextureManager.cs` | Image loading |
| `Entities/Entity.cs` | Abstract character base |
| `Entities/Player.cs` | Player character |
| `Entities/Enemy.cs` | Abstract enemy base |
| `Entities/Goblin.cs` | Goblin enemy |
| `Entities/Orc.cs` | Orc enemy |
| `Entities/DarkMage.cs` | Ranged mage enemy |
| `Components/HealthBar.cs` | HP/shield bar UI |
| `Components/ProjectileManager.cs` | Bullet list manager |
| `Interfaces/IDamageable.cs` | Damage contract |
| `oop_concepts.md` | OOP concepts reference |

---

## Quick Reference — Who Calls What

| Action | Triggered by | Handled by |
|---|---|---|
| Move player | WASD keys | `Form1` → `Player.Move()` |
| Shoot | Space | `Form1` → `Player.Fire()` |
| Shield heal | Shift | `Form1` → `Player.SpecialAbility()` |
| Melee attack | Mouse click | `Form1` → `Player.Attack()` |
| Enemy spawns | Timer in `UpdateGame` | `GameManger.SpawnEnemy()` |
| Bullet hits enemy | Each frame | `GameManger.CheckPlayerProjectiles()` |
| Mage hits player | Each frame | `GameManger.CheckEnemyProjectiles()` |
| Draw everything | Timer `Invalidate` | `Form1.Form1_Paint()` |
