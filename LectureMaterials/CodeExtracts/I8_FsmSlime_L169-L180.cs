        private bool CanSeePlayer() => playerTarget != null &&
            Mathf.Abs(playerTarget.position.x - transform.position.x) <= detectionRange &&
            Mathf.Abs(playerTarget.position.y - transform.position.y) <= chaseVerticalTolerance;

        private bool IsPlayerInAttackRange() => playerTarget != null &&
            Vector2.Distance(transform.position, playerTarget.position) <= attackRange;

        private void ApplyAttackImpact()
        {
            if (!IsPlayerInAttackRange()) return;
            playerTarget.GetComponent<IDamageable>()?.TakeDamage(attackDamage);
        }
