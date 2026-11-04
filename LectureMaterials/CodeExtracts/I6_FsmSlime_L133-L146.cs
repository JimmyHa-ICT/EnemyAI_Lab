        private void MovePatrol()
        {
            if (leftPatrolPoint == null || rightPatrolPoint == null)
            {
                StopHorizontalMovement();
                return;
            }

            float left = Mathf.Min(leftPatrolPoint.position.x, rightPatrolPoint.position.x);
            float right = Mathf.Max(leftPatrolPoint.position.x, rightPatrolPoint.position.x);
            if (movingRight && body.position.x >= right - arrivalDistance) movingRight = false;
            else if (!movingRight && body.position.x <= left + arrivalDistance) movingRight = true;
            MoveHorizontally(movingRight ? 1f : -1f, patrolSpeed);
        }
