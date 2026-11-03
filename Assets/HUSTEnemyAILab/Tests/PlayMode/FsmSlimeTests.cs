using System.Collections;
using HUST.EnemyAILab.Intermediate;
using HUST.EnemyAILab.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HUST.EnemyAILab.Tests.PlayMode
{
    public sealed class FsmSlimeTests
    {
        private GameObject leftPoint, rightPoint, player, slimeObject;
        private FsmSlime slime;
        private PlayerHealth health;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            leftPoint = new GameObject("LeftPoint");
            rightPoint = new GameObject("RightPoint");
            player = new GameObject("Player");
            slimeObject = new GameObject("FsmSlimeTest");
            slimeObject.SetActive(false);
            leftPoint.transform.position = new Vector3(-4f, 0f);
            rightPoint.transform.position = new Vector3(4f, 0f);
            player.transform.position = new Vector3(10f, 0f);
            health = player.AddComponent<PlayerHealth>();
            slimeObject.AddComponent<Rigidbody2D>().gravityScale = 0f;
            slime = slimeObject.AddComponent<FsmSlime>();
            slime.Configure(leftPoint.transform, rightPoint.transform, player.transform);
            slime.ConfigureTuning(5f, 1.25f, 0.2f, 0.7f, 1);
            slimeObject.SetActive(true);
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            Object.Destroy(slimeObject);
            Object.Destroy(player);
            Object.Destroy(leftPoint);
            Object.Destroy(rightPoint);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ExplicitRangeConditions_DriveAllStateTransitions()
        {
            Assert.That(slime.CurrentState, Is.EqualTo(EnemyState.Patrol));
            player.transform.position = new Vector3(4f, 3f);
            yield return null;
            Assert.That(slime.CurrentState, Is.EqualTo(EnemyState.Patrol),
                "A player on another vertical level must not trigger Chase.");
            player.transform.position = new Vector3(4f, 0f);
            yield return null;
            Assert.That(slime.CurrentState, Is.EqualTo(EnemyState.Chase));
            player.transform.position = new Vector3(1f, 0f);
            yield return null;
            Assert.That(slime.CurrentState, Is.EqualTo(EnemyState.Attack));
            player.transform.position = new Vector3(3f, 0f);
            yield return null;
            Assert.That(slime.CurrentState, Is.EqualTo(EnemyState.Chase));
            player.transform.position = new Vector3(10f, 0f);
            yield return null;
            Assert.That(slime.CurrentState, Is.EqualTo(EnemyState.Patrol));
        }

        [UnityTest]
        public IEnumerator Patrol_ReversesAtBothBoundariesAndFlipsVisual()
        {
            leftPoint.transform.position = new Vector3(-0.25f, 0f);
            rightPoint.transform.position = new Vector3(0.25f, 0f);
            var body = slimeObject.GetComponent<Rigidbody2D>();
            var visual = slimeObject.GetComponent<SpriteRenderer>();
            bool movedRight = false;
            bool reversedLeft = false;
            bool reversedRightAgain = false;
            bool flippedWhileMovingLeft = false;

            for (int frame = 0; frame < 120 && !reversedRightAgain; frame++)
            {
                yield return new WaitForFixedUpdate();
                if (body.linearVelocity.x > 0f)
                {
                    if (reversedLeft) reversedRightAgain = true;
                    movedRight = true;
                }
                else if (body.linearVelocity.x < 0f && movedRight)
                {
                    reversedLeft = true;
                    flippedWhileMovingLeft |= visual.flipX;
                }
            }

            Assert.That(movedRight, Is.True);
            Assert.That(reversedLeft, Is.True);
            Assert.That(reversedRightAgain, Is.True);
            Assert.That(flippedWhileMovingLeft, Is.True);
        }

        [UnityTest]
        public IEnumerator LeavingAttackRangeDuringWindUp_DoesNotDealDamage()
        {
            player.transform.position = new Vector3(1f, 0f);
            yield return null;
            yield return null;
            yield return null;
            Assert.That(slime.IsAttackWindingUp, Is.True);
            player.transform.position = new Vector3(3f, 0f);
            yield return new WaitForSeconds(0.3f);
            Assert.That(health.CurrentHealth, Is.EqualTo(health.MaxHealth));
            Assert.That(slime.CurrentState, Is.EqualTo(EnemyState.Chase));
        }

        [UnityTest]
        public IEnumerator AttackCooldown_PreventsAnImmediateSecondHit()
        {
            player.transform.position = new Vector3(0.5f, 0f);
            yield return WaitForHealth(health.MaxHealth - 1, 1f);
            Assert.That(health.CurrentHealth, Is.EqualTo(health.MaxHealth - 1));
            yield return new WaitForSeconds(0.55f);
            Assert.That(health.CurrentHealth, Is.EqualTo(health.MaxHealth - 1));
            yield return WaitForHealth(health.MaxHealth - 2, 1f);
            Assert.That(health.CurrentHealth, Is.EqualTo(health.MaxHealth - 2));
        }

        private IEnumerator WaitForHealth(int expectedHealth, float timeout)
        {
            float deadline = Time.time + timeout;
            while (health.CurrentHealth > expectedHealth && Time.time < deadline) yield return null;
        }
    }
}
