using UnityEngine;

namespace HUST.EnemyAILab.Production
{
    [CreateAssetMenu(menuName = "HUST Enemy AI Lab/Enemy Config", fileName = "EnemyConfig")]
    public sealed class EnemyConfig : ScriptableObject
    {
        [field: SerializeField, Min(0f)] public float MoveSpeed { get; private set; } = 2f;
        [field: SerializeField, Min(0f)] public float ChaseSpeed { get; private set; } = 3f;
        [field: SerializeField, Min(0f)] public float DetectionRange { get; private set; } = 5f;
        [field: SerializeField, Min(0f)] public float ChaseVerticalTolerance { get; private set; } = 1.25f;
        [field: SerializeField, Min(0f)] public float AttackRange { get; private set; } = 1.25f;
        [field: SerializeField, Min(0f)] public float AttackCooldown { get; private set; } = 1f;
        [field: SerializeField, Min(0f)] public float AttackWindUp { get; private set; } = 0.5f;
        [field: SerializeField, Min(1)] public int Damage { get; private set; } = 1;
        [field: SerializeField, Min(1)] public int MaximumHealth { get; private set; } = 3;

        public void Configure(float move, float chase, float detection, float attack,
            float cooldown, float windUp, int damage, int health, float chaseVerticalTolerance = 1.25f)
        {
            MoveSpeed = Mathf.Max(0f, move);
            ChaseSpeed = Mathf.Max(0f, chase);
            DetectionRange = Mathf.Max(0f, detection);
            ChaseVerticalTolerance = Mathf.Max(0f, chaseVerticalTolerance);
            AttackRange = Mathf.Clamp(attack, 0f, DetectionRange);
            AttackCooldown = Mathf.Max(0f, cooldown);
            AttackWindUp = Mathf.Max(0f, windUp);
            Damage = Mathf.Max(1, damage);
            MaximumHealth = Mathf.Max(1, health);
        }
    }
}
