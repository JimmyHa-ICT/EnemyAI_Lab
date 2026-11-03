using System.Collections;
using HUST.EnemyAILab.Intermediate;
using HUST.EnemyAILab.Player;
using HUST.EnemyAILab.Production;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HUST.EnemyAILab.Tests.PlayMode
{
    public sealed class ModularEnemyTests
    {
        private EnemyConfig config;
        private GameObject enemy, player, left, right;
        private EnemySensor sensor;
        private EnemyCombat combat;
        private EnemyBrain brain;
        private EnemyHealth enemyHealth;
        private PlayerHealth playerHealth;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            config = ScriptableObject.CreateInstance<EnemyConfig>();
            config.Configure(2f, 3f, 5f, 1.25f, 0.4f, 0.15f, 1, 3);
            left = new GameObject("Left"); right = new GameObject("Right");
            left.transform.position = new Vector3(-4f, 0f); right.transform.position = new Vector3(4f, 0f);
            player = new GameObject("Player"); player.layer = LayerMask.NameToLayer("Player");
            player.AddComponent<BoxCollider2D>(); playerHealth = player.AddComponent<PlayerHealth>();
            player.transform.position = new Vector3(10f, 0f);
            enemy = new GameObject("ProductionSlimeTest"); enemy.SetActive(false);
            var renderer = enemy.AddComponent<SpriteRenderer>();
            enemy.AddComponent<Rigidbody2D>().gravityScale = 0f;
            sensor = enemy.AddComponent<EnemySensor>();
            var movement = enemy.AddComponent<EnemyMovement>();
            combat = enemy.AddComponent<EnemyCombat>();
            brain = enemy.AddComponent<EnemyBrain>();
            enemyHealth = enemy.AddComponent<EnemyHealth>();
            sensor.Configure(config, LayerMask.GetMask("Player"));
            movement.Configure(config, left.transform, right.transform, renderer);
            combat.Configure(config, sensor);
            brain.Configure(sensor, movement, combat, renderer);
            enemyHealth.Configure(config, renderer, new Behaviour[] { brain, movement, combat, sensor });
            enemy.SetActive(true);
            Physics2D.SyncTransforms();
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(enemy); Object.Destroy(player); Object.Destroy(left); Object.Destroy(right); Object.Destroy(config);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Health_DamagesAndRaisesDeathExactlyOnce()
        {
            int deaths = 0; enemyHealth.Died += () => deaths++;
            enemyHealth.TakeDamage(1);
            Assert.That(enemyHealth.CurrentHealth, Is.EqualTo(2));
            enemyHealth.TakeDamage(10);
            enemyHealth.TakeDamage(10);
            Assert.That(enemyHealth.IsDead, Is.True);
            Assert.That(deaths, Is.EqualTo(1));
            yield return null;
        }

        [UnityTest]
        public IEnumerator Sensor_DetectsAndLosesPlayerUsingLayerMask()
        {
            player.transform.position = new Vector3(4f, 0f); Physics2D.SyncTransforms(); sensor.Scan();
            Assert.That(sensor.CurrentTarget, Is.EqualTo(player.transform));
            player.transform.position = new Vector3(10f, 0f); Physics2D.SyncTransforms(); sensor.Scan();
            Assert.That(sensor.HasTarget, Is.False);
            yield return null;
        }

        [UnityTest]
        public IEnumerator Brain_CentralizesAllRangeTransitions()
        {
            Assert.That(brain.CurrentState, Is.EqualTo(EnemyState.Patrol));
            player.transform.position = new Vector3(4f, 0f); Physics2D.SyncTransforms(); yield return null;
            Assert.That(brain.CurrentState, Is.EqualTo(EnemyState.Chase));
            player.transform.position = new Vector3(1f, 0f); Physics2D.SyncTransforms(); yield return null;
            Assert.That(brain.CurrentState, Is.EqualTo(EnemyState.Attack));
            player.transform.position = new Vector3(3f, 0f); Physics2D.SyncTransforms(); yield return null;
            Assert.That(brain.CurrentState, Is.EqualTo(EnemyState.Chase));
            player.transform.position = new Vector3(10f, 0f); Physics2D.SyncTransforms(); yield return null;
            Assert.That(brain.CurrentState, Is.EqualTo(EnemyState.Patrol));
        }

        [UnityTest]
        public IEnumerator Combat_RevalidatesImpactAndPreventsDuplicateOrCooldownHits()
        {
            brain.enabled = false;
            player.transform.position = new Vector3(0.5f, 0f); Physics2D.SyncTransforms(); sensor.Scan();
            Assert.That(combat.TryBeginAttack(), Is.True);
            player.transform.position = new Vector3(3f, 0f); Physics2D.SyncTransforms(); sensor.Scan();
            yield return new WaitForSeconds(0.2f); combat.Tick();
            Assert.That(playerHealth.CurrentHealth, Is.EqualTo(playerHealth.MaxHealth));

            player.transform.position = new Vector3(0.5f, 0f); Physics2D.SyncTransforms(); sensor.Scan();
            yield return new WaitForSeconds(0.45f);
            Assert.That(combat.TryBeginAttack(), Is.True);
            yield return new WaitForSeconds(0.2f); combat.Tick(); combat.Tick();
            Assert.That(playerHealth.CurrentHealth, Is.EqualTo(playerHealth.MaxHealth - 1));
            Assert.That(combat.TryBeginAttack(), Is.False, "Cooldown must block an immediate repeat attack.");
        }
    }
}
