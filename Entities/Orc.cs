using System;

namespace Dungeon_Siege
{
    public class Orc : Enemy
    {
        public int Armor { get; set; }
        private bool hasEnraged;

        public Orc(int health, Point position, int speed, Image? sprite, int armor)
            : base(health, position, speed, sprite, 20, 7, 50)
        {
            Armor = armor;
            Level = 3;
        }

        public override void TakeDamage(int amount)
        {
            base.TakeDamage(Math.Max(1, amount - Armor));

            if (!hasEnraged && Health < MaxHealth / 2)
                SpecialAbility();
        }

        public override void Move(Player target)
        {
            ChaseTarget(target);
        }

        public override void SpecialAbility()
        {
            if (hasEnraged) return;
            hasEnraged = true;
            Speed += 1;
            Armor += 2;
        }
    }
}
