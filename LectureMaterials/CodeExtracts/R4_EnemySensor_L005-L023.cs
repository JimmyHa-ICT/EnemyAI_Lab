    public sealed class EnemySensor : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private LayerMask playerLayer;
        public Transform CurrentTarget { get; private set; }
        public bool HasTarget => CurrentTarget != null;

        public void Scan()
        {
            if (config == null) { CurrentTarget = null; return; }
            Vector2 detectionSize = new Vector2(config.DetectionRange * 2f,
                config.ChaseVerticalTolerance * 2f);
            Collider2D hit = Physics2D.OverlapBox(transform.position, detectionSize, 0f, playerLayer);
            CurrentTarget = hit != null ? hit.transform : null;
        }

        public bool IsTargetInAttackRange() => IsInAttackRange(CurrentTarget);
        public bool IsInAttackRange(Transform target) => target != null && config != null &&
            Vector2.Distance(transform.position, target.position) <= config.AttackRange;
