using HUST.EnemyAILab.Player;
using NUnit.Framework;
using UnityEngine;

namespace HUST.EnemyAILab.Tests.EditMode
{
    public sealed class PlayerHealthTests
    {
        private GameObject player;
        private PlayerHealth health;

        [SetUp]
        public void SetUp()
        {
            player = new GameObject("PlayerHealthTest");
            health = player.AddComponent<PlayerHealth>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(player);
        }

        [Test]
        public void FatalDamage_ClampsHealthToZero()
        {
            health.TakeDamage(100);

            Assert.That(health.CurrentHealth, Is.EqualTo(0));
            Assert.That(health.IsDead, Is.True);
        }

        [Test]
        public void ResetHealth_RestoresMaximumHealth()
        {
            health.TakeDamage(100);
            health.ResetHealth();

            Assert.That(health.CurrentHealth, Is.EqualTo(health.MaxHealth));
            Assert.That(health.IsDead, Is.False);
        }
    }
}
