using HUST.EnemyAILab.Shared.Combat;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HUST.EnemyAILab.Player
{
    public sealed class PlayerCombat : MonoBehaviour
    {
        [SerializeField] private LayerMask enemyLayer;
        [SerializeField, Min(0f)] private float attackRange = 1.35f;
        [SerializeField, Min(1)] private int damage = 1;
        [SerializeField, Min(0f)] private float cooldown = 0.35f;
        private float nextAttackTime;

        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.fKey.wasPressedThisFrame || Time.time < nextAttackTime) return;
            nextAttackTime = Time.time + cooldown;
            Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, enemyLayer);
            hit?.GetComponent<IDamageable>()?.TakeDamage(damage);
        }

        public void Configure(LayerMask mask) { enemyLayer = mask; }
    }
}
