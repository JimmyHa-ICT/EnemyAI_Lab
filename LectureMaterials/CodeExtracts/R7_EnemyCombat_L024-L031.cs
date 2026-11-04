        public bool TryBeginAttack()
        {
            if (IsWindingUp || config == null || sensor == null || Time.time < nextAttackTime || !sensor.IsTargetInAttackRange()) return false;
            pendingTarget = sensor.CurrentTarget;
            impactTime = Time.time + config.AttackWindUp;
            IsWindingUp = true;
            return true;
        }
