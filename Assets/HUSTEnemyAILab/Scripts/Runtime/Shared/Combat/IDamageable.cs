namespace HUST.EnemyAILab.Shared.Combat
{
    /// <summary>
    /// Minimal contract shared by objects that can receive gameplay damage.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int amount);
    }
}
