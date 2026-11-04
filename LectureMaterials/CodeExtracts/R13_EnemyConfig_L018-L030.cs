        public void Configure(float move, float chase, float detection, float attack,
            float cooldown, float windUp, int damage, int health, float chaseVerticalTolerance = 1.25f)
        {
            MoveSpeed = Mathf.Max(0f, move);
            ChaseSpeed = Mathf.Max(0f, chase);
            DetectionRange = Mathf.Max(0f, detection);
            ChaseVerticalTolerance = Mathf.Max(0f, chaseVerticalTolerance);
            AttackRange = Mathf.Clamp(attack, 0f, DetectionRange);
            AttackCooldown = Mathf.Max(0f, cooldown);
            AttackWindUp = Mathf.Max(0f, windUp);
            Damage = Mathf.Max(1, damage);
            MaximumHealth = Mathf.Max(1, health);
        }
