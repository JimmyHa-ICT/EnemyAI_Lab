        private void Update()
        {
            FindPlayerIfNeeded();
            switch (currentState)
            {
                case EnemyState.Patrol: UpdatePatrol(); break;
                case EnemyState.Chase: UpdateChase(); break;
                case EnemyState.Attack: UpdateAttack(); break;
            }
        }

        private void FixedUpdate()
        {
            switch (currentState)
            {
                case EnemyState.Patrol: MovePatrol(); break;
                case EnemyState.Chase: MoveTowardPlayer(); break;
                case EnemyState.Attack: StopHorizontalMovement(); break;
            }
        }
