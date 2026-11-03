using System.Collections;
using HUST.EnemyAILab.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HUST.EnemyAILab.Tests.PlayMode
{
    public sealed class PlayerFoundationTests
    {
        [UnityTest]
        public IEnumerator PlayerController_RequiresRigidbody2D()
        {
            var player = new GameObject("PlayerControllerTest");
            player.SetActive(false);
            player.AddComponent<PlayerController>();

            Assert.That(player.GetComponent<Rigidbody2D>(), Is.Not.Null);

            Object.Destroy(player);
            yield return null;
        }
    }
}
