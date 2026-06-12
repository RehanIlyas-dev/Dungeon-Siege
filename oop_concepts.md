# OOP Concepts in Dungeon Siege

This document identifies where and how the four core Object-Oriented Programming concepts — **Inheritance**, **Abstraction**, **Polymorphism**, and **Composition** — are applied throughout the Dungeon Siege codebase.

---

## 1. 🧬 Inheritance

> **Inheritance** allows a class to acquire properties and methods from a parent (base) class, enabling code reuse and logical class hierarchies.

### Hierarchy Overview

```
Entity  (abstract base)
├── Player
└── Enemy  (abstract)
    ├── Goblin
    ├── Orc
    └── DarkMage
```

---

### `Entity` → `Player`

**File**: [Player.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Player.cs#L7)

`Player` inherits from `Entity`, gaining `Health`, `Position`, `Speed`, `Sprite`, `TakeDamage()`, and `IsAlive()` for free. The constructor chains up with `: base(...)`.

```csharp
// Player.cs — line 7
public class Player : Entity   // ← inherits from Entity
{
    public Player(int health, Point position, int speed, Image? sprite)
        : base(health, position, speed, sprite)  // ← calls Entity's constructor
    { ... }
}
```

---

### `Entity` → `Enemy`

**File**: [Enemy.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Enemy.cs#L7)

`Enemy` inherits from `Entity` and adds combat-specific properties like `Damage`, `DetectionRange`, `KillValue`, `Level`, and `MaxHealth`.

```csharp
// Enemy.cs — line 7
public abstract class Enemy : Entity   // ← inherits from Entity
{
    public int Damage { get; set; }
    public int Level { get; protected set; }
    public int MaxHealth { get; set; }
}
```

---

### `Enemy` → `Goblin`, `Orc`, `DarkMage`

**Files**: [Goblin.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Goblin.cs#L7), [Orc.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Orc.cs#L7), [DarkMage.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/DarkMage.cs#L7)

All three enemy types inherit from `Enemy`, which itself inherits from `Entity`. They each call `: base(...)` with their own stats.

```csharp
// Goblin.cs — Level 1 enemy
public class Goblin : Enemy
{
    public Goblin(...) : base(health, position, speed, sprite, 10, 5, 25)
    { Level = 1; }
}

// Orc.cs — Level 3 enemy
public class Orc : Enemy
{
    public Orc(..., int armor) : base(health, position, speed, sprite, 20, 7, 50)
    { Armor = armor; Level = 3; }
}

// DarkMage.cs — Level 2 enemy
public class DarkMage : Enemy
{
    public DarkMage(...) : base(health, position, speed, sprite, 15, 10, 40)
    { Mana = 100; Level = 2; }
}
```

---

## 2. 🔷 Abstraction

> **Abstraction** hides implementation details behind a contract (abstract class or interface), forcing subclasses to provide their own concrete implementations.

### `Entity` — The Abstract Base Class

**File**: [Entity.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Entity.cs#L7)

`Entity` is declared `abstract` and defines three **abstract methods** that every subclass *must* implement. This enforces a consistent contract across all game characters.

```csharp
// Entity.cs
public abstract class Entity
{
    // ── Concrete shared properties (reused by all subclasses) ──
    public int Health { get; set; }
    public Point Position { get; set; }
    public int Speed { get; set; }

    // ── Concrete shared methods ──
    public virtual void TakeDamage(int Amount) { Health -= Amount; }
    public bool IsAlive() { return Health > 0; }

    // ── Abstract methods — MUST be implemented by every subclass ──
    public abstract void Move(Player target);
    public abstract void SpecialAbility();
    public abstract void Draw(Graphics g);
}
```

| Abstract Method | Purpose |
|---|---|
| `Move(Player target)` | Each character defines its own movement logic |
| `SpecialAbility()` | Each character has a unique special skill |
| `Draw(Graphics g)` | Each character renders itself differently |

### `Enemy` — A Second Layer of Abstraction

**File**: [Enemy.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Enemy.cs#L7)

`Enemy` is itself `abstract`, sitting between `Entity` and the concrete enemy classes. It provides a shared implementation of `Attack()` and `Draw()` (health bar + level label) while still leaving `Move()` and `SpecialAbility()` open for each enemy type.

```csharp
public abstract class Enemy : Entity   // ← is itself abstract
{
    // Shared implementation for all enemies
    public virtual void Attack(Player target) { ... }
    public override void Draw(Graphics g) { /* draws sprite + HP bar + "Lvl X" */ }

    // Still left abstract (from Entity): Move(), SpecialAbility()
}
```

### `IDamageable` — Interface-Based Abstraction

**File**: [IDamageable.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Interfaces/IDamageable.cs)

An **interface** defines a pure contract — only method/property signatures, no implementation. `IDamageable` specifies that any damageable game object must expose `Health`, `TakeDamage()`, and `IsAlive()`.

```csharp
// IDamageable.cs
public interface IDamageable
{
    int Health { get; set; }
    void TakeDamage(int amount);
    bool IsAlive();
}
```

`Entity` **implements** this interface, so every subclass (`Player`, `Goblin`, `Orc`, `DarkMage`) automatically satisfies the contract:

```csharp
// Entity.cs
public abstract class Entity : IDamageable
{
    public int Health { get; set; }
    public virtual void TakeDamage(int Amount) { Health -= Amount; }
    public bool IsAlive() { return Health > 0; }
}
```

**Why use an interface here?** `GameManger` can deal damage through the `IDamageable` type without knowing whether the target is a `Player` or an `Enemy` — enabling **interface polymorphism**:

```csharp
// GameManger.cs — damage applied via interface, not concrete class
private void ApplyDamage(IDamageable target, int damage)
{
    target.TakeDamage(damage);  // calls Player.TakeDamage or Enemy.TakeDamage at runtime
}
```

| Abstraction Type | Mechanism | Example in Project |
|---|---|---|
| Abstract class | Shared state + partial implementation | `Entity`, `Enemy` |
| Interface | Pure contract, no implementation | `IDamageable` |

---

## 3. 🔄 Polymorphism

> **Polymorphism** allows the same method call to behave differently depending on the actual type of the object at runtime (method overriding) or compile time (method overloading).

### Runtime Polymorphism — `override` keyword

Every concrete class overrides the abstract methods from `Entity`/`Enemy` with its own specific behaviour.

#### `Move()` — Different movement strategies

| Class | Behaviour |
|---|---|
| `Player` | Moves with WASD keys, clamps to screen bounds |
| `Goblin` | Chases player directly if in detection range |
| `Orc` | Chases player; enrages (gains speed) below 50% HP |
| `DarkMage` | Maintains a safe shooting distance from player |

```csharp
// Goblin.cs — chases player
public override void Move(Player target)
{
    // Move towards target if in detection range
    int moveX = (int)((dx / distance) * Speed);
    Position = new Point(Position.X + moveX, Position.Y + moveY);
}

// DarkMage.cs — kites player
public override void Move(Player target)
{
    if (distance > 250) { /* move closer */ }
    else if (distance < 150) { /* move away */ }
}
```

---

#### `SpecialAbility()` — Unique abilities per class

| Class | Special Ability |
|---|---|
| `Player` | Spends 30 Stamina to regenerate 30 Shield |
| `Goblin` | Steals 15 Stamina + 10 Shield from the player |
| `Orc` | Enrages when below 50% HP: +1 Speed, +2 Armor |
| `DarkMage` | Fires a powerful projectile at the player |

```csharp
// Orc.cs — override SpecialAbility
public override void SpecialAbility()
{
    Speed += 1;   // enrage!
    Armor += 2;
}

// Player.cs — override SpecialAbility
public override void SpecialAbility()
{
    Shields = Math.Min(100, Shields + 30);
    Stamina -= 30;
}
```

---

#### `TakeDamage()` — Overridden with extra logic

**`Player`** overrides `TakeDamage()` to absorb damage through its shield first before reducing Health — the base `Entity.TakeDamage()` only reduces health directly.

```csharp
// Player.cs — shields absorb damage first
public override void TakeDamage(int amount)
{
    if (Shields > 0)
    {
        Shields -= amount;
        if (Shields < 0) { base.TakeDamage(-Shields); Shields = 0; }
    }
    else { base.TakeDamage(amount); }  // ← calls Entity's version
}

// Orc.cs — armor reduces incoming damage
public override void TakeDamage(int Amount)
{
    int finalDamage = Math.Max(1, Amount - Armor);  // armor absorbs
    base.TakeDamage(finalDamage);
}
```

---

#### `Draw()` — Each character draws itself differently

`GameManager` calls `enemy.Draw(g)` on every `Enemy` in the list — it doesn't know or care whether it is a `Goblin`, `Orc`, or `DarkMage`. The correct `Draw()` is resolved at **runtime**.

```csharp
// GameManger.cs / Form1.cs — polymorphic call
foreach (var enemy in gameManager.enemies)  // List<Enemy>
{
    enemy.Draw(g);  // calls Goblin.Draw, Orc.Draw, or DarkMage.Draw automatically
}
```

| Class | Extra draw behaviour |
|---|---|
| `Enemy` (base) | Sprite + HP bar + `"Lvl X"` label |
| `DarkMage` | Calls `base.Draw(g)` then additionally draws all its projectiles |

---

### Compile-time Polymorphism — Method Overloading

**File**: [Entity.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Entity.cs#L22)

`TakeDamage` is **overloaded** in `Entity` — same name, different parameters. The compiler picks the right version based on what arguments are passed.

```csharp
// Entity.cs — two versions of the same method
public virtual void TakeDamage(int Amount) { ... }
public virtual void TakeDamage(int Amount, string Type) { ... }  // typed damage
```

---

## 4. 🧩 Composition

> **Composition** models a **"has-a"** relationship: a class contains other objects as parts rather than inheriting from them. This keeps responsibilities separate and makes components reusable.

### Composition vs Inheritance

| Relationship | Keyword | Example |
|---|---|---|
| **is-a** (Inheritance) | `: Entity` | `Goblin : Enemy` — a Goblin *is* an Enemy |
| **has-a** (Composition) | field/property | `Player` *has a* `ProjectileManager` |

### `HealthBar` — Reusable UI Component

**File**: [HealthBar.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Components/HealthBar.cs)

`HealthBar` is a standalone class that knows how to draw a bar. It is **not** part of the inheritance tree — instead, entities **own** it:

```csharp
// Entity.cs — COMPOSITION: Entity has-a HealthBar
protected HealthBar healthBar;

public Entity(int health, ...)
{
    healthBar = new HealthBar(health, Brushes.Green, Brushes.Red);
}

// Enemy.Draw() delegates bar rendering to the composed component
healthBar.Sync(Health);
healthBar.Draw(g, Position.X, Position.Y);
```

`Player` composes **two** bar components — one inherited `healthBar` for HP and a separate `shieldBar` for shields:

```csharp
// Player.cs
private HealthBar shieldBar;

shieldBar = new HealthBar(100, Brushes.DeepSkyBlue, Brushes.Transparent, 40, 2, -5);
```

### `ProjectileManager` — Behaviour Component

**File**: [ProjectileManager.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Components/ProjectileManager.cs)

Projectile logic (add, update, draw, remove) is extracted into its own class. Both `Player` and `DarkMage` compose a `ProjectileManager` instead of duplicating list-management code:

```csharp
// Player.cs — COMPOSITION: Player has-a ProjectileManager
public ProjectileManager ProjectileManager { get; private set; } = new ProjectileManager();

ProjectileManager.Add(p);
ProjectileManager.Update(screenWidth, screenHeight);
ProjectileManager.Draw(g);

// DarkMage.cs — same component, different owner
public ProjectileManager ProjectileManager { get; private set; } = new ProjectileManager();
```

### `GameManger` — Composing Game Objects

**File**: [GameManger.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Core/GameManger.cs)

The game manager **has-a** `Player` and **has-a** `List<Enemy>`. It orchestrates them without inheriting from either:

```csharp
// GameManger.cs — COMPOSITION: GameManger has-a Player and has-a enemies list
public Player player { get; set; }
public List<Enemy> enemies { get; set; }
```

### `Form1` — Composing the Game Manager

**File**: [Form1.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Form1.cs)

The Windows Form **has-a** `GameManger` and delegates all game logic to it:

```csharp
// Form1.cs — COMPOSITION: Form1 has-a GameManger
private GameManger gameManager;
```

### Composition Map

```
Form1
 └── GameManger
      ├── Player
      │    ├── HealthBar (healthBar)
      │    ├── HealthBar (shieldBar)
      │    └── ProjectileManager
      └── List<Enemy>
           ├── Goblin  → HealthBar (healthBar)
           ├── Orc     → HealthBar (healthBar)
           └── DarkMage → HealthBar (healthBar) + ProjectileManager
```

---

## Summary Table

| Concept | Where Used | Files |
|---|---|---|
| **Inheritance** | `Player : Entity` | [Player.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Player.cs) |
| **Inheritance** | `Enemy : Entity` | [Enemy.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Enemy.cs) |
| **Inheritance** | `Goblin : Enemy` | [Goblin.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Goblin.cs) |
| **Inheritance** | `Orc : Enemy` | [Orc.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Orc.cs) |
| **Inheritance** | `DarkMage : Enemy` | [DarkMage.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/DarkMage.cs) |
| **Abstraction** | `abstract class Entity` with 3 abstract methods | [Entity.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Entity.cs) |
| **Abstraction** | `abstract class Enemy` (second layer) | [Enemy.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Enemy.cs) |
| **Abstraction** | `IDamageable` interface implemented by `Entity` | [IDamageable.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Interfaces/IDamageable.cs), [Entity.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Entity.cs) |
| **Polymorphism** | `ApplyDamage(IDamageable)` — interface-based damage calls | [GameManger.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Core/GameManger.cs) |
| **Polymorphism** | `override Move()` — 4 different movement behaviours | Goblin, Orc, DarkMage, Player |
| **Polymorphism** | `override SpecialAbility()` — 4 unique special skills | Goblin, Orc, DarkMage, Player |
| **Polymorphism** | `override TakeDamage()` — Shield (Player) & Armor (Orc) logic | Player, Orc |
| **Polymorphism** | `override Draw()` — each character renders itself | All entities |
| **Polymorphism** | `TakeDamage(int)` vs `TakeDamage(int, string)` overloading | [Entity.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Entity.cs) |
| **Composition** | `Entity` has-a `HealthBar` | [Entity.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Entities/Entity.cs), [HealthBar.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Components/HealthBar.cs) |
| **Composition** | `Player` / `DarkMage` has-a `ProjectileManager` | [ProjectileManager.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Components/ProjectileManager.cs) |
| **Composition** | `GameManger` has-a `Player` + `List<Enemy>` | [GameManger.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Core/GameManger.cs) |
| **Composition** | `Form1` has-a `GameManger` | [Form1.cs](file:///d:/2nd%20Semester/OOPL/Dungeon%20Siege/Form1.cs) |
