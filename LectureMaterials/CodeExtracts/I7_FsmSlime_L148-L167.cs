        private void MoveTowardPlayer()
        {
            if (playerTarget == null)
            {
                StopHorizontalMovement();
                return;
            }
            MoveHorizontally(Mathf.Sign(playerTarget.position.x - body.position.x), chaseSpeed);
        }

        private void MoveHorizontally(float direction, float speed)
        {
            body.linearVelocity = new Vector2(direction * speed, body.linearVelocity.y);
            if (!Mathf.Approximately(direction, 0f) && visual != null) visual.flipX = direction < 0f;
        }

        private void StopHorizontalMovement()
        {
            if (body != null) body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
        }
