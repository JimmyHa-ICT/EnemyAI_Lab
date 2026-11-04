        private void Update()
        {
            sensor.Scan();
            combat.Tick();
            switch (CurrentState)
            {
                case EnemyState.Patrol:
                    if (sensor.HasTarget) ChangeState(EnemyState.Chase);
                    break;
                case EnemyState.Chase:
                    if (!sensor.HasTarget) ChangeState(EnemyState.Patrol);
                    else if (sensor.IsTargetInAttackRange()) ChangeState(EnemyState.Attack);
                    break;
                case EnemyState.Attack:
                    if (!sensor.HasTarget) ChangeState(EnemyState.Patrol);
                    else if (!sensor.IsTargetInAttackRange()) ChangeState(EnemyState.Chase);
                    else combat.TryBeginAttack();
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (CurrentState == EnemyState.Patrol) movement.Patrol();
            else if (CurrentState == EnemyState.Chase) movement.Chase(sensor.CurrentTarget);
            else movement.Stop();
        }
