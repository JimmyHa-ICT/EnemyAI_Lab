using System;
using System.Collections;
using HUST.EnemyAILab.Shared.Combat;
using UnityEngine;

namespace HUST.EnemyAILab.Production
{
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

        private IEnumerator FlashHit()
        {
            if (visual == null) yield break;
            Color restore = visual.color; visual.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            if (!died) visual.color = restore;
        }

        private void Die()
        {
            if (died) return;
            died = true;
            if (visual != null) visual.color = new Color(0.2f, 0.2f, 0.2f, 0.65f);
            foreach (Behaviour item in disableOnDeath) if (item != null) item.enabled = false;
            Died?.Invoke();
        }

        public void Configure(EnemyConfig value, SpriteRenderer renderer, Behaviour[] behaviours)
        { config = value; visual = renderer; disableOnDeath = behaviours; }
    }
}
