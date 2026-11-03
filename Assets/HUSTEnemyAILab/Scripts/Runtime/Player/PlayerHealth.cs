using System;
using System.Collections;
using HUST.EnemyAILab.Shared.Combat;
using UnityEngine;

namespace HUST.EnemyAILab.Player
{
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1)] private int maxHealth = 3;
        [SerializeField, Min(0f)] private float hitInvulnerabilitySeconds = 0.5f;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Color damageFlashColor = new Color(1f, 0.25f, 0.25f, 1f);
        [SerializeField, Min(0f)] private float damageFlashSeconds = 0.12f;

        private float nextDamageTime;
        private Color normalColor = Color.white;
        private Coroutine damageFlashRoutine;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsDead => CurrentHealth <= 0;

        public event Action<int, int> HealthChanged;
        public event Action Died;

        private void Awake()
        {
            visual ??= GetComponent<SpriteRenderer>();
            if (visual != null) normalColor = visual.color;
            ResetHealth();
        }

        public void TakeDamage(int amount)
        {
            if (amount <= 0 || IsDead || Time.time < nextDamageTime)
            {
                return;
            }

            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            nextDamageTime = Time.time + hitInvulnerabilitySeconds;
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
            PlayDamageFlash();

            if (IsDead)
            {
                Died?.Invoke();
            }
        }

        public void ResetHealth()
        {
            CurrentHealth = maxHealth;
            nextDamageTime = 0f;
            RestoreVisual();
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }

        private void PlayDamageFlash()
        {
            if (visual == null || !Application.isPlaying) return;
            if (damageFlashRoutine != null) StopCoroutine(damageFlashRoutine);
            damageFlashRoutine = StartCoroutine(DamageFlash());
        }

        private IEnumerator DamageFlash()
        {
            visual.color = damageFlashColor;
            yield return new WaitForSeconds(damageFlashSeconds);
            damageFlashRoutine = null;
            RestoreVisual();
        }

        private void OnDisable() { RestoreVisual(); }

        private void RestoreVisual()
        {
            if (damageFlashRoutine != null && Application.isPlaying)
            {
                StopCoroutine(damageFlashRoutine);
                damageFlashRoutine = null;
            }
            if (visual != null) visual.color = normalColor;
        }
    }
}
