using System;

namespace Dungeon_Siege
{
    public class Goblin : Enemy
    {
        public float StealChance { get; set; }
        private Player? lastTarget;
        private readonly Random rand = new Random();

        public Goblin(int health, Point position, int speed, Image? sprite)
            : base(health, position, speed, sprite, 10, 5, 25)
        {
            StealChance = 0.3f;
            Level = 1;
        }

        public override void Move(Player target)
        {
            ChaseTarget(target);
        }

        public override void Attack(Player target)
        {
            if (target == null || !target.IsAlive() || !IsAlive()) return;
            if (Position.DistanceTo(target.Position) > 50) return;

            lastTarget = target;
            base.Attack(target);

            if (rand.NextDouble() < StealChance)
                SpecialAbility();
        }

        public override void SpecialAbility()
        {
            if (lastTarget == null || !lastTarget.IsAlive()) return;

            lastTarget.Stamina = Math.Max(0, lastTarget.Stamina - 15);
            lastTarget.Shields = Math.Max(0, lastTarget.Shields - 10);
            lastTarget.SyncShieldBar();
        }
    }
}
