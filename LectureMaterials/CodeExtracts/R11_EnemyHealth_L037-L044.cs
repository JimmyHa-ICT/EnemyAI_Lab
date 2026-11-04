        private void Die()
        {
            if (died) return;
            died = true;
            if (visual != null) visual.color = new Color(0.2f, 0.2f, 0.2f, 0.65f);
            foreach (Behaviour item in disableOnDeath) if (item != null) item.enabled = false;
            Died?.Invoke();
        }
