using HUST.EnemyAILab.Shared.Combat;
using UnityEngine;

namespace HUST.EnemyAILab.Production
{
    public sealed class EnemyCombat : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private EnemySensor sensor;
        private Transform pendingTarget;
        private float impactTime;
        private float nextAttackTime;
        public bool IsWindingUp { get; private set; }

        public bool TryBeginAttack()
        {
            if (IsWindingUp || config == null || sensor == null || Time.time < nextAttackTime || !sensor.IsTargetInAttackRange()) return false;
            pendingTarget = sensor.CurrentTarget;
            impactTime = Time.time + config.AttackWindUp;
            IsWindingUp = true;
            return true;
        }

        public void Tick()
        {
            if (!IsWindingUp || Time.time < impactTime) return;
            IsWindingUp = false;
            Transform target = pendingTarget;
            pendingTarget = null;
            if (target != null && sensor.CurrentTarget == target && sensor.IsInAttackRange(target))
                target.GetComponent<IDamageable>()?.TakeDamage(config.Damage);
            nextAttackTime = Time.time + config.AttackCooldown;
        }

        public void CancelAttack() { IsWindingUp = false; pendingTarget = null; }
        public void Configure(EnemyConfig value, EnemySensor valueSensor) { config = value; sensor = valueSensor; }
    }
}
