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
