            float leftBoundary = Mathf.Min(leftPatrolPoint.position.x, rightPatrolPoint.position.x);
            float rightBoundary = Mathf.Max(leftPatrolPoint.position.x, rightPatrolPoint.position.x);

            if (movingRight && body.position.x >= rightBoundary - arrivalDistance)
            {
                movingRight = false;
            }
            else if (!movingRight && body.position.x <= leftBoundary + arrivalDistance)
            {
                movingRight = true;
            }

            float direction = movingRight ? 1f : -1f;
            body.linearVelocity = new Vector2(direction * patrolSpeed, body.linearVelocity.y);
            visual.flipX = direction < 0f;
