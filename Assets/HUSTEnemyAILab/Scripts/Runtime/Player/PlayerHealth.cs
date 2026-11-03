using System;
using HUST.EnemyAILab.Shared.Combat;
using UnityEngine;

namespace HUST.EnemyAILab.Player
{
    public sealed class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField, Min(1)] private int maxHealth = 3;
        [SerializeField, Min(0f)] private float hitInvulnerabilitySeconds = 0.5f;

        private float nextDamageTime;

        public int CurrentHealth { get; private set; }
        public int MaxHealth => maxHealth;
        public bool IsDead => CurrentHealth <= 0;

        public event Action<int, int> HealthChanged;
        public event Action Died;

        private void Awake()
        {
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

            if (IsDead)
            {
                Died?.Invoke();
            }
        }

        public void ResetHealth()
        {
            CurrentHealth = maxHealth;
            nextDamageTime = 0f;
            HealthChanged?.Invoke(CurrentHealth, maxHealth);
        }
    }
}
