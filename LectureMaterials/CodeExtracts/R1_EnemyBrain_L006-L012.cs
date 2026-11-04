    public sealed class EnemyBrain : MonoBehaviour
    {
        [SerializeField] private EnemySensor sensor;
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private EnemyCombat combat;
        [SerializeField] private SpriteRenderer visual;
        public EnemyState CurrentState { get; private set; } = EnemyState.Patrol;
