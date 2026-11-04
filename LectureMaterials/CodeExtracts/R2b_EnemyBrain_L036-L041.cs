        private void FixedUpdate()
        {
            if (CurrentState == EnemyState.Patrol) movement.Patrol();
            else if (CurrentState == EnemyState.Chase) movement.Chase(sensor.CurrentTarget);
            else movement.Stop();
        }
