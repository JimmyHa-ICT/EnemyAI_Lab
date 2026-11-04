        public void CancelAttack()
        {
            if (validationMode == AttackValidationMode.UnsafeTeachingDemo) return;
            IsWindingUp = false;
            pendingTarget = null;
        }
