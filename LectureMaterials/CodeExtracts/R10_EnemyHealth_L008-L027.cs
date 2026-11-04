    public sealed class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Behaviour[] disableOnDeath;
        private Color normalColor;
        private bool died;
        public int CurrentHealth { get; private set; }
        public bool IsDead => died;
        public event Action Died;

        private void Awake() { visual ??= GetComponent<SpriteRenderer>(); normalColor = visual != null ? visual.color : Color.white; ResetHealth(); }
        public void ResetHealth() { died = false; CurrentHealth = config != null ? config.MaximumHealth : 1; }

        public void TakeDamage(int amount)
        {
            if (amount <= 0 || died) return;
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            if (CurrentHealth == 0) Die(); else if (isActiveAndEnabled) StartCoroutine(FlashHit());
        }
