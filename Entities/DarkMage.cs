using System;

namespace Dungeon_Siege
{
    public class DarkMage : Enemy
    {
        public int Mana { get; set; }
        public ProjectileManager ProjectileManager { get; } = new ProjectileManager();

        private Player? lastTarget;
        private int attackCooldown;
        private readonly Random rand = new Random();

        public DarkMage(int health, Point position, int speed, Image? sprite)
            : base(health, position, speed, sprite, 15, 10, 40)
        {
            Mana = 100;
            Level = 2;
        }

        public override void Move(Player target)
        {
            if (target == null || !IsAlive()) return;

            ProjectileManager.Update(arenaWidth, arenaHeight);

            double dx = target.Position.X - preciseX;
            double dy = target.Position.Y - preciseY;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            double moveX = 0, moveY = 0;
            if (distance > 250)
            {
                moveX = (dx / distance) * MoveSpeed;
                moveY = (dy / distance) * MoveSpeed;
            }
            else if (distance < 150)
            {
                moveX = -(dx / distance) * MoveSpeed;
                moveY = -(dy / distance) * MoveSpeed;
            }

            if (moveX != 0 || moveY != 0)
            {
                preciseX += moveX;
                preciseY += moveY;
                Position = new Point((int)preciseX, (int)preciseY);
                ClampToPlayableBounds();
            }

            if (rand.Next(4) == 0 && Mana < 100) Mana++;
        }

        public override void Attack(Player target)
        {
            if (target == null || !target.IsAlive() || !IsAlive()) return;

            lastTarget = target;
            if (attackCooldown > 0) { attackCooldown--; return; }

            double dx = target.Position.X - Position.X;
            double dy = target.Position.Y - Position.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance > DetectionRange * 40) return;

            if (Mana >= 60 && distance <= 250)
            {
                SpecialAbility();
                attackCooldown = 240;
            }
            else if (Mana >= 25)
            {
                Mana -= 25;
                FireProjectile(dx, dy, distance);
                attackCooldown = 120;
            }
        }

        public override void SpecialAbility()
        {
            if (lastTarget == null || !lastTarget.IsAlive() || Mana < 60) return;

            Mana -= 60;
            double dx = lastTarget.Position.X - Position.X;
            double dy = lastTarget.Position.Y - Position.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance == 0) distance = 1;

            FireProjectile(dx, dy, distance);
        }

        public override void Draw(Graphics g)
        {
            base.Draw(g);
            ProjectileManager.Draw(g);
        }

        private void FireProjectile(double dx, double dy, double distance)
        {
            Projectile p = new Projectile(Damage, new Point(Position.X + 10, Position.Y + 10), 6, TextureManager.ProjectileSprite);
            p.DirX = dx / distance;
            p.DirY = dy / distance;
            ProjectileManager.Add(p);
        }
    }
}
