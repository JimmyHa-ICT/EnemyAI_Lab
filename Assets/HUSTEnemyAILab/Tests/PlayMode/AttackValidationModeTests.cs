using System.Collections;
using HUST.EnemyAILab.Player;
using HUST.EnemyAILab.Production;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HUST.EnemyAILab.Tests.PlayMode
{
    public sealed class AttackValidationModeTests
    {
        private EnemyConfig config;
        private GameObject enemy;
        private GameObject player;
        private EnemySensor sensor;
        private EnemyCombat combat;
        private PlayerHealth health;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            config = ScriptableObject.CreateInstance<EnemyConfig>();
            config.Configure(0f, 0f, 5f, 1.25f, 0.25f, 0.1f, 1, 3);
            enemy = new GameObject("CombatLifecycleTest");
            sensor = enemy.AddComponent<EnemySensor>();
            combat = enemy.AddComponent<EnemyCombat>();
            sensor.Configure(config, LayerMask.GetMask("Player"));
            combat.Configure(config, sensor);

            player = new GameObject("Player");
            player.layer = LayerMask.NameToLayer("Player");
            player.AddComponent<BoxCollider2D>();
            health = player.AddComponent<PlayerHealth>();
            player.transform.position = new Vector3(0.5f, 0f);
            Physics2D.SyncTransforms();
            sensor.Scan();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(enemy);
            if (player != null) Object.Destroy(player);
            Object.Destroy(config);
            yield return null;
        }

        [UnityTest]
        public IEnumerator UnsafeTeachingDemo_DamagesCapturedTargetAfterLeavingRange()
        {
            combat.SetValidationMode(AttackValidationMode.UnsafeTeachingDemo);
            Assert.That(combat.TryBeginAttack(), Is.True);
            LeaveRangeAndCancel();
            yield return new WaitForSeconds(0.12f);
            combat.Tick();
            Assert.That(health.CurrentHealth, Is.EqualTo(health.MaxHealth - 1));
        }

        [UnityTest]
        public IEnumerator Safe_CancelsPendingAttackAfterLeavingRange()
        {
            Assert.That(combat.ValidationMode, Is.EqualTo(AttackValidationMode.Safe));
            Assert.That(combat.TryBeginAttack(), Is.True);
            LeaveRangeAndCancel();
            yield return new WaitForSeconds(0.12f);
            combat.Tick();
            Assert.That(health.CurrentHealth, Is.EqualTo(health.MaxHealth));
            Assert.That(combat.IsWindingUp, Is.False);
        }

        [UnityTest]
        public IEnumerator ValidSafeAttack_DamagesOnceAndHonorsCooldown()
        {
            Assert.That(combat.TryBeginAttack(), Is.True);
            yield return new WaitForSeconds(0.12f);
            combat.Tick();
            combat.Tick();
            Assert.That(health.CurrentHealth, Is.EqualTo(health.MaxHealth - 1));
            Assert.That(combat.TryBeginAttack(), Is.False);
            yield return new WaitForSeconds(0.27f);
            Assert.That(combat.TryBeginAttack(), Is.True);
        }

        [UnityTest]
        public IEnumerator DestroyedCapturedTarget_DoesNotThrowInEitherMode()
        {
            combat.SetValidationMode(AttackValidationMode.UnsafeTeachingDemo);
            Assert.That(combat.TryBeginAttack(), Is.True);
            Object.Destroy(player);
            yield return null;
            yield return new WaitForSeconds(0.12f);
            Assert.DoesNotThrow(combat.Tick);
        }

        [UnityTest]
        public IEnumerator PlayerDamage_FlashesVisualThenRestoresItsColor()
        {
            SpriteRenderer visual = player.AddComponent<SpriteRenderer>();
            visual.color = Color.white;
            Object.Destroy(health);
            yield return null;
            health = player.AddComponent<PlayerHealth>();
            yield return null;

            health.TakeDamage(1);
            Assert.That(visual.color, Is.Not.EqualTo(Color.white));
            yield return new WaitForSeconds(0.15f);
            Assert.That(visual.color, Is.EqualTo(Color.white));
        }

        private void LeaveRangeAndCancel()
        {
            player.transform.position = new Vector3(3f, 0f);
            Physics2D.SyncTransforms();
            sensor.Scan();
            combat.CancelAttack();
        }
    }
}
