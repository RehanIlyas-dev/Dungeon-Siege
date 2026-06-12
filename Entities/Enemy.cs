using System;
using System.Drawing;

namespace Dungeon_Siege
{
    public abstract class Enemy : Entity
    {
        public int Damage { get; set; }
        public int DetectionRange { get; set; }
        public int KillValue { get; set; }
        public int MaxHealth { get; set; }
        public int Level { get; protected set; }
        public double MoveSpeed { get; set; }

        protected double preciseX;
        protected double preciseY;

        protected int meleeAttackCooldown;
        protected const int MeleeAttackInterval = 45;
        protected int arenaWidth = 800;
        protected int arenaHeight = 600;
        protected Rectangle playableBounds;

        public Enemy(int health, Point position, int speed, Image? sprite, int damage, int detectionRange, int killValue)
            : base(health, position, speed, sprite)
        {
            Damage = damage;
            DetectionRange = detectionRange;
            KillValue = killValue;
            MaxHealth = health;
            healthBar.MaxValue = MaxHealth;
            healthBar.YOffset = -8;
            healthBar.BarHeight = 3;

            MoveSpeed = speed;
            preciseX = position.X;
            preciseY = position.Y;
        }

        public void SetArenaSize(int width, int height)
        {
            arenaWidth = width;
            arenaHeight = height;
        }

        public void SetPlayableBounds(Rectangle bounds)
        {
            playableBounds = bounds;
        }

        protected void ClampToPlayableBounds()
        {
            if (playableBounds.Width <= 0) return;
            int clampedX = Math.Clamp(Position.X, playableBounds.Left, playableBounds.Right);
            int clampedY = Math.Clamp(Position.Y, playableBounds.Top, playableBounds.Bottom);
            if (clampedX != Position.X || clampedY != Position.Y)
            {
                Position = new Point(clampedX, clampedY);
                preciseX = clampedX;
                preciseY = clampedY;
            }
        }

        protected void ChaseTarget(Player target, double stopDistance = 10)
        {
            if (target == null || !IsAlive()) return;

            double dx = target.Position.X - preciseX;
            double dy = target.Position.Y - preciseY;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance <= stopDistance) return;

            preciseX += (dx / distance) * MoveSpeed;
            preciseY += (dy / distance) * MoveSpeed;
            Position = new Point((int)preciseX, (int)preciseY);
            ClampToPlayableBounds();
        }

        public virtual void Attack(Player target)
        {
            if (meleeAttackCooldown > 0) meleeAttackCooldown--;
            if (target == null || !target.IsAlive() || !IsAlive()) return;

            if (Position.DistanceTo(target.Position) <= 50 && meleeAttackCooldown <= 0)
            {
                target.TakeDamage(Damage);
                meleeAttackCooldown = MeleeAttackInterval;
            }
        }

        public override void Draw(Graphics g)
        {
            if (Sprite != null)
                g.DrawImage(Sprite, Position.X, Position.Y, 40, 40);
            else
                g.FillEllipse(Brushes.Red, Position.X, Position.Y, 40, 40);

            healthBar.Sync(Health);
            healthBar.Draw(g, Position.X, Position.Y);

            using Font font = new Font("Arial", 8, FontStyle.Bold);
            g.DrawString("Lvl " + Level, font, Brushes.White, Position.X, Position.Y - 22);
        }
    }
}
