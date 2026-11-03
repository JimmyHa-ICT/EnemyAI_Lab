using HUST.EnemyAILab.Shared.Combat;
using UnityEngine;

namespace HUST.EnemyAILab.Production
{
    public enum AttackValidationMode
    {
        Safe,
        UnsafeTeachingDemo
    }

    public sealed class EnemyCombat : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private EnemySensor sensor;
        [SerializeField, Tooltip("Safe is the production default. UnsafeTeachingDemo intentionally skips range revalidation for Milestone 5 teaching only.")]
        private AttackValidationMode validationMode = AttackValidationMode.Safe;
        private Transform pendingTarget;
        private float impactTime;
        private float nextAttackTime;
        public bool IsWindingUp { get; private set; }
        public AttackValidationMode ValidationMode => validationMode;

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
            bool canDamage = validationMode == AttackValidationMode.UnsafeTeachingDemo
                ? target != null
                : target != null && sensor.CurrentTarget == target && sensor.IsInAttackRange(target);
            if (canDamage)
                target.GetComponent<IDamageable>()?.TakeDamage(config.Damage);
            nextAttackTime = Time.time + config.AttackCooldown;
        }

        public void CancelAttack()
        {
            if (validationMode == AttackValidationMode.UnsafeTeachingDemo) return;
            IsWindingUp = false;
            pendingTarget = null;
        }

        public void SetValidationMode(AttackValidationMode mode) { validationMode = mode; }
        public void Configure(EnemyConfig value, EnemySensor valueSensor) { config = value; sensor = valueSensor; }
    }
}
