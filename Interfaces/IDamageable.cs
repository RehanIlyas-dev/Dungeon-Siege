namespace Dungeon_Siege
{
    // ABSTRACTION: damage contract used by GameManger.ApplyDamage.
    public interface IDamageable
    {
        int Health { get; set; }
        void TakeDamage(int amount);
        bool IsAlive();
    }
}
