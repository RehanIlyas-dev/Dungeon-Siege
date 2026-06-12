using System;
using System.Drawing;

namespace Dungeon_Siege
{
    public class Player : Entity
    {
        public int Kills { get; set; }
        public int Shields { get; set; }
        public int Stamina { get; set; }
        public bool MoveUp { get; set; }
        public bool MoveDown { get; set; }
        public bool MoveLeft { get; set; }
        public bool MoveRight { get; set; }
        public ProjectileManager ProjectileManager { get; } = new ProjectileManager();

        private readonly HealthBar shieldBar;
        private int fireCooldown;
        private int abilityCooldown;
        private double lastDirX = 1;
        private double lastDirY;

        public Player(int health, Point position, int speed, Image? sprite)
            : base(health, position, speed, sprite)
        {
            Shields = 100;
            Stamina = 100;
            shieldBar = new HealthBar(100, Brushes.DeepSkyBlue, Brushes.Transparent, 40, 2, -5);
            shieldBar.Sync(Shields);
        }

        public void ResetStats()
        {
            Health = 100;
            Shields = 100;
            Stamina = 100;
            Kills = 0;
            abilityCooldown = 0;
            healthBar.Sync(Health);
            shieldBar.Sync(Shields);
        }

        public void SyncShieldBar()
        {
            shieldBar.Sync(Shields);
        }

        public void ResetMovement()
        {
            MoveUp = MoveDown = MoveLeft = MoveRight = false;
        }

        public override void TakeDamage(int amount)
        {
            if (Shields > 0)
            {
                Shields -= amount;
                if (Shields < 0)
                {
                    base.TakeDamage(-Shields);
                    Shields = 0;
                }
            }
            else
            {
                base.TakeDamage(amount);
            }
            shieldBar.Sync(Shields);
        }

        public void Fire()
        {
            if (!IsAlive() || fireCooldown > 0) return;

            Projectile p = new Projectile(40, new Point(Position.X + 10, Position.Y + 10), 10, TextureManager.ProjectileSprite);
            p.DirX = lastDirX;
            p.DirY = lastDirY;
            ProjectileManager.Add(p);
            fireCooldown = 8;
        }

        public void Attack(Enemy target)
        {
            if (target == null || !target.IsAlive() || !IsAlive()) return;
            if (Position.DistanceTo(target.Position) > 80) return;

            target.TakeDamage(50);
            if (!target.IsAlive()) Kills++;
        }

        public void Move(Player target, Rectangle bounds)
        {
            if (!IsAlive()) return;

            if (fireCooldown > 0) fireCooldown--;
            if (abilityCooldown > 0) abilityCooldown--;

            ProjectileManager.Update(bounds.Right + Utils.EntitySize, bounds.Bottom + Utils.EntitySize);

            int dx = 0, dy = 0;
            if (MoveUp) dy -= Speed;
            if (MoveDown) dy += Speed;
            if (MoveLeft) dx -= Speed;
            if (MoveRight) dx += Speed;

            if (dx != 0 || dy != 0)
            {
                double mag = Math.Sqrt(dx * dx + dy * dy);
                lastDirX = dx / mag;
                lastDirY = dy / mag;
            }
            if (dx != 0 && dy != 0)
            {
                dx = (int)(dx * 0.707);
                dy = (int)(dy * 0.707);
            }

            int newX = Math.Clamp(Position.X + dx, bounds.Left, bounds.Right);
            int newY = Math.Clamp(Position.Y + dy, bounds.Top, bounds.Bottom);
            Position = new Point(newX, newY);

            if (Stamina < 100) Stamina++;
        }

        public override void Move(Player target) { }

        public override void SpecialAbility()
        {
            if (!IsAlive() || abilityCooldown > 0 || Stamina < 30) return;

            Shields = Math.Min(100, Shields + 30);
            Stamina -= 30;
            shieldBar.Sync(Shields);
            abilityCooldown = 45;
        }

        public override void Draw(Graphics g)
        {
            ProjectileManager.Draw(g);

            if (Sprite != null)
                g.DrawImage(Sprite, Position.X, Position.Y, 40, 40);
            else
                g.FillEllipse(Brushes.Blue, Position.X, Position.Y, 40, 40);

            healthBar.Draw(g, Position.X, Position.Y);
            shieldBar.Draw(g, Position.X, Position.Y);
        }
    }
}
 
