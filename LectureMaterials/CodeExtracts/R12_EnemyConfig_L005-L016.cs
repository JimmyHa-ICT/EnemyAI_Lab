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
