        private void UpdateAttack()
        {
            if (!CanSeePlayer())
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (!IsPlayerInAttackRange())
            {
                ChangeState(EnemyState.Chase);
                return;
            }
