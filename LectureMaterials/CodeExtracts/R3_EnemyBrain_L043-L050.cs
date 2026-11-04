        private void ChangeState(EnemyState state)
        {
            if (state == CurrentState) return;
            if (CurrentState == EnemyState.Attack) combat.CancelAttack();
            CurrentState = state;
            if (state == EnemyState.Attack) movement.Stop();
            ApplyColor();
        }
