using System.Collections;
using HUST.EnemyAILab.Prototype;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace HUST.EnemyAILab.Tests.PlayMode
{
    public sealed class PrototypeSlimeTests
    {
        [UnityTest]
        public IEnumerator Patrol_ReversesAtBothBoundariesAndFlipsVisual()
        {
            var leftPoint = new GameObject("LeftPoint");
            var rightPoint = new GameObject("RightPoint");
            var slimeObject = new GameObject("PrototypeSlimeTest");
            slimeObject.SetActive(false);

            leftPoint.transform.position = new Vector3(-0.25f, 0f, 0f);
            rightPoint.transform.position = new Vector3(0.25f, 0f, 0f);

            var body = slimeObject.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            var slime = slimeObject.AddComponent<PrototypeSlime>();
            slime.ConfigurePatrol(leftPoint.transform, rightPoint.transform, 2f);
            slimeObject.SetActive(true);

            bool movedRight = false;
            bool reversedLeft = false;
            bool reversedRightAgain = false;
            bool flippedWhileMovingLeft = false;

            for (int frame = 0; frame < 120 && !reversedRightAgain; frame++)
            {
                yield return new WaitForFixedUpdate();

                if (body.linearVelocity.x > 0f)
                {
                    if (reversedLeft)
                    {
                        reversedRightAgain = true;
                    }

                    movedRight = true;
                }
                else if (body.linearVelocity.x < 0f && movedRight)
                {
                    reversedLeft = true;
                    flippedWhileMovingLeft |= slimeObject.GetComponent<SpriteRenderer>().flipX;
                }
            }

            Assert.That(movedRight, Is.True, "The slime never moved toward the right patrol point.");
            Assert.That(reversedLeft, Is.True, "The slime did not reverse at the right patrol point.");
            Assert.That(reversedRightAgain, Is.True, "The slime did not reverse at the left patrol point.");
            Assert.That(flippedWhileMovingLeft, Is.True, "The visual did not flip while moving left.");

            Object.Destroy(slimeObject);
            Object.Destroy(leftPoint);
            Object.Destroy(rightPoint);
        }
    }
}
