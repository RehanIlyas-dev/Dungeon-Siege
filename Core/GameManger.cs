using System;
using System.Collections.Generic;
using System.Drawing;

namespace Dungeon_Siege
{
    public enum GameDifficulty { Easy, Medium, Hard }

    public class GameManger
    {
        public Player player { get; set; }
        public List<Enemy> enemies { get; set; }
        public int KillCount { get; set; }
        public int KillLimit { get; set; }
        public bool IsGameOver { get; set; }
        public GameDifficulty CurrentDifficulty { get; set; } = GameDifficulty.Medium;

        private readonly Random rand = new Random();
        private int spawnTimer;
        private bool waitingForFirstSpawn = true;

        public GameManger(Player player, List<Enemy> enemies, int killLimit)
        {
            this.player = player;
            this.enemies = enemies;
            KillLimit = killLimit;
        }

        public void StartGame(Rectangle screenBounds)
        {
            player.ResetStats();
            Rectangle playable = Utils.GetPlayableBounds(screenBounds.Width, screenBounds.Height);
            player.Position = new Point(playable.Left + playable.Width / 2, playable.Top + playable.Height / 2);
            player.ProjectileManager.Clear();

            enemies.Clear();
            KillCount = 0;
            IsGameOver = false;
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    KillLimit = 5;
                    break;
                case GameDifficulty.Hard:
                    KillLimit = 15;
                    break;
                default:
                    KillLimit = 10;
                    break;
            }

            waitingForFirstSpawn = true;
            spawnTimer = 0;
            SpawnEnemy(screenBounds);
        }

        public void UpdateGame(Rectangle bounds)
        {
            if (IsGameOverCheck()) return;

            Rectangle playableBounds = Utils.GetPlayableBounds(bounds.Width, bounds.Height);
            player.Move(player, playableBounds);

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                Enemy enemy = enemies[i];
                if (!enemy.IsAlive()) { enemies.RemoveAt(i); continue; }

                enemy.SetArenaSize(bounds.Width, bounds.Height);
                enemy.SetPlayableBounds(playableBounds);
                enemy.Move(player);
                enemy.Attack(player);
            }

            CheckPlayerProjectiles();
            CheckEnemyProjectiles();
            KillCount = player.Kills;

            spawnTimer++;
            int spawnDelay = waitingForFirstSpawn ? GetInitialSpawnDelay() : GetSpawnThreshold();
            if (spawnTimer >= spawnDelay && enemies.Count < GetMaxConcurrentEnemies())
            {
                SpawnEnemy(bounds);
                spawnTimer = 0;
                waitingForFirstSpawn = false;
            }
        }

        public void SpawnEnemy(Rectangle screenBounds)
        {
            Point pos = GetSpawnPosition(screenBounds);
            int type = rand.Next(3);
            double speed = GetEnemySpeed(type);
            int damage = GetScaledDamage(type == 0 ? (5, 10, 20) : type == 1 ? (10, 20, 40) : (7, 15, 30));

            Enemy newEnemy;
            switch (type)
            {
                case 0:
                    newEnemy = new Goblin(60, pos, 1, TextureManager.GoblinSprite);
                    break;
                case 1:
                    newEnemy = new Orc(120, pos, 1, TextureManager.OrcSprite, GetOrcArmor());
                    break;
                default:
                    newEnemy = new DarkMage(80, pos, 1, TextureManager.DarkMageSprite);
                    break;
            }
            newEnemy.MoveSpeed = speed;
            newEnemy.Damage = damage;
            enemies.Add(newEnemy);
        }

        private Point GetSpawnPosition(Rectangle screenBounds) // Get a random spawn position on the edge of the playable area.
        {
            Rectangle playable = Utils.GetPlayableBounds(screenBounds.Width, screenBounds.Height);
            switch (rand.Next(4))
            {
                case 0:
                    return new Point(rand.Next(playable.Left, playable.Right + 1), playable.Top);
                case 1:
                    return new Point(playable.Right, rand.Next(playable.Top, playable.Bottom + 1));
                case 2:
                    return new Point(rand.Next(playable.Left, playable.Right + 1), playable.Bottom);
                default:
                    return new Point(playable.Left, rand.Next(playable.Top, playable.Bottom + 1));
            }
        }

        private void CheckPlayerProjectiles() // Check for collisions between player projectiles and enemies.
        {
            for (int i = player.ProjectileManager.All.Count - 1; i >= 0; i--)
            {
                Point projCenter = new Point(player.ProjectileManager.All[i].Position.X + 10, player.ProjectileManager.All[i].Position.Y + 10);

                for (int j = enemies.Count - 1; j >= 0; j--)
                {
                    Enemy enemy = enemies[j];
                    if (!enemy.IsAlive()) continue;

                    Point enemyCenter = new Point(enemy.Position.X + 20, enemy.Position.Y + 20);
                    if (projCenter.DistanceTo(enemyCenter) >= 25) continue;

                    ApplyDamage(enemy, player.ProjectileManager.All[i].Damage);
                    if (!enemy.IsAlive()) player.Kills++;
                    player.ProjectileManager.RemoveAt(i);
                    break;
                }
            }
        }

        private void CheckEnemyProjectiles() // Check for collisions between enemy projectiles and the player.
        {
            Point playerCenter = new Point(player.Position.X + 20, player.Position.Y + 20);

            foreach (Enemy enemy in enemies)
            {
                if (enemy is not DarkMage mage) continue;

                for (int i = mage.ProjectileManager.All.Count - 1; i >= 0; i--)
                {
                    Projectile proj = mage.ProjectileManager.All[i];
                    Point projCenter = new Point(proj.Position.X + 10, proj.Position.Y + 10);
                    if (projCenter.DistanceTo(playerCenter) >= 25) continue;

                    int damage = proj.Damage;
                    if (CurrentDifficulty == GameDifficulty.Easy) damage /= 2;
                    if (CurrentDifficulty == GameDifficulty.Hard) damage *= 2;

                    ApplyDamage(player, damage);
                    mage.ProjectileManager.RemoveAt(i);
                }
            }
        }

        private void ApplyDamage(IDamageable target, int damage) // Apply damage to the target.
        {
            target.TakeDamage(damage);
        }

        public bool IsGameOverCheck()
        {
            if (player.Health <= 0 || KillCount >= KillLimit) IsGameOver = true;
            return IsGameOver;
        }

        private int GetMaxConcurrentEnemies()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 5;
                case GameDifficulty.Hard:
                    return 12;
                default:
                    return 8;
            }
        }

        private int GetInitialSpawnDelay() // Get the initial delay before the first enemy spawns.
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 180;
                case GameDifficulty.Hard:
                    return 45;
                default:
                    return 90;
            }
        }

        private int GetSpawnThreshold() // Get the delay between enemy spawns.
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 150;
                case GameDifficulty.Hard:
                    return 35;
                default:
                    return 70;
            }
        }

        private int GetOrcArmor()
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return 3;
                case GameDifficulty.Hard:
                    return 8;
                default:
                    return 5;
            }
        }

        private double GetEnemySpeed(int enemyType)
        {
            // enemyType: 0 = Goblin, 1 = Orc (tank, always slower), 2 = DarkMage
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    if (enemyType == 1) return 0.8;  // Orc
                    return 1.2;                      // Goblin / DarkMage

                case GameDifficulty.Hard:
                    // Fast and aggressive, but slightly toned down
                    if (enemyType == 1) return 1.6;  // Orc
                    return 2.6;                      // Goblin / DarkMage

                default: // Medium
                    if (enemyType == 1) return 0.9;  // Orc
                    return 1.4;                      // Goblin / DarkMage
            }
        }

        private int GetScaledDamage((int easy, int medium, int hard) values)
        {
            switch (CurrentDifficulty)
            {
                case GameDifficulty.Easy:
                    return values.easy;
                case GameDifficulty.Hard:
                    return values.hard;
                default:
                    return values.medium;
            }
        }
    }
}
