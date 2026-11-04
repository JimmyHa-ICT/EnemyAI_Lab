        private void ChangeState(EnemyState newState)
        {
            if (currentState == newState) return;
            if (currentState == EnemyState.Attack) attackWindingUp = false;
            currentState = newState;
            if (newState == EnemyState.Attack) StopHorizontalMovement();
            ApplyStateVisual();
        }
