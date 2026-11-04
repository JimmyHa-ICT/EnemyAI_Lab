        private void UpdatePatrol()
        {
            if (CanSeePlayer()) ChangeState(EnemyState.Chase);
        }

        private void UpdateChase()
        {
            if (!CanSeePlayer())
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (IsPlayerInAttackRange()) ChangeState(EnemyState.Attack);
        }
