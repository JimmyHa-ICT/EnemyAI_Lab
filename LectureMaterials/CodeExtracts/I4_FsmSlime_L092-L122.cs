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

            if (attackWindingUp)
            {
                if (Time.time >= impactTime)
                {
                    attackWindingUp = false;
                    ApplyAttackImpact();
                    nextAttackTime = Time.time + attackCooldown;
                }
                return;
            }

            if (Time.time >= nextAttackTime)
            {
                attackWindingUp = true;
                impactTime = Time.time + attackWindUp;
            }
        }
