using System.Drawing;

namespace Dungeon_Siege
{
    // ABSTRACTION + INHERITANCE: base class for Player and Enemy.
    public abstract class Entity : IDamageable
    {
        public int Health { get; set; }
        public Point Position { get; set; }
        public int Speed { get; set; }
        public Image? Sprite { get; set; }

        // COMPOSITION: Entity "has-a" HealthBar. The HealthBar is a component that makes up the Entity.
        protected HealthBar healthBar;

        public Entity(int health, Point position, int speed, Image? sprite)
        {
            Health = health;
            Position = position;
            Speed = speed;
            Sprite = sprite;
            healthBar = new HealthBar(health, Brushes.Green, Brushes.Red);
        }

        public virtual void TakeDamage(int amount)
        {
            Health -= amount;
            if (Health < 0) Health = 0;
            healthBar.Sync(Health);
        }

        public virtual void TakeDamage(int amount, string type)
        {
            TakeDamage(amount);
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public abstract void Move(Player target);
        public abstract void SpecialAbility();
        public abstract void Draw(Graphics g);
    }
}
