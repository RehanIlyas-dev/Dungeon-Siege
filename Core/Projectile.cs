using System.Drawing;

namespace Dungeon_Siege
{
    public class Projectile
    {
        public int Damage { get; set; }
        public Point Position { get; set; }
        public int Speed { get; set; }
        public Image? Sprite { get; set; }
        public double DirX { get; set; }
        public double DirY { get; set; }

        private double preciseX;
        private double preciseY;
        private bool initialized;

        public Projectile(int damage, Point position, int speed, Image? sprite)
        {
            Damage = damage;
            Position = position;
            Speed = speed;
            Sprite = sprite;
        }

        public void Move()
        {
            if (!initialized)
            {
                preciseX = Position.X;
                preciseY = Position.Y;
                initialized = true;
            }
            preciseX += DirX * Speed;
            preciseY += DirY * Speed;
            Position = new Point((int)preciseX, (int)preciseY);
        }

        public void Draw(Graphics g)
        {
            if (Sprite != null)
                g.DrawImage(Sprite, Position.X, Position.Y, 20, 20);
            else
                g.FillEllipse(Brushes.Red, Position.X, Position.Y, 15, 15);
        }

        public bool IsOutOfBounds(int width, int height)
        {
            return Position.X < 0 || Position.X > width || Position.Y < 0 || Position.Y > height;
        }
    }
}
