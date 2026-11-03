using UnityEngine;

namespace HUST.EnemyAILab.Prototype
{
    /// <summary>
    /// First-pass enemy implementation for a single, limited requirement:
    /// move horizontally between two patrol points.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public sealed class PrototypeSlime : MonoBehaviour
    {
        [Header("Patrol")]
        [SerializeField] private Transform leftPatrolPoint;
        [SerializeField] private Transform rightPatrolPoint;
        [SerializeField, Min(0f)] private float patrolSpeed = 2f;
        [SerializeField, Min(0.01f)] private float arrivalDistance = 0.05f;
        [SerializeField] private bool startMovingRight = true;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer visual;

        private Rigidbody2D body;
        private bool movingRight;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            visual ??= GetComponent<SpriteRenderer>();
            movingRight = startMovingRight;
        }

        private void FixedUpdate()
        {
            if (leftPatrolPoint == null || rightPatrolPoint == null)
            {
                body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
                return;
            }

            float leftBoundary = Mathf.Min(leftPatrolPoint.position.x, rightPatrolPoint.position.x);
            float rightBoundary = Mathf.Max(leftPatrolPoint.position.x, rightPatrolPoint.position.x);

            if (movingRight && body.position.x >= rightBoundary - arrivalDistance)
            {
                movingRight = false;
            }
            else if (!movingRight && body.position.x <= leftBoundary + arrivalDistance)
            {
                movingRight = true;
            }

            float direction = movingRight ? 1f : -1f;
            body.linearVelocity = new Vector2(direction * patrolSpeed, body.linearVelocity.y);
            visual.flipX = direction < 0f;
        }

        private void OnDisable()
        {
            if (body != null)
            {
                body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
            }
        }

        /// <summary>
        /// Supports straightforward runtime spawning and focused tests without a framework.
        /// Inspector references remain the normal scene-authoring workflow.
        /// </summary>
        public void ConfigurePatrol(Transform leftPoint, Transform rightPoint, float speed)
        {
            leftPatrolPoint = leftPoint;
            rightPatrolPoint = rightPoint;
            patrolSpeed = Mathf.Max(0f, speed);
        }

        private void OnDrawGizmosSelected()
        {
            if (leftPatrolPoint == null || rightPatrolPoint == null)
            {
                return;
            }

            Gizmos.color = new Color(1f, 0.85f, 0.1f);
            Gizmos.DrawLine(leftPatrolPoint.position, rightPatrolPoint.position);
            Gizmos.DrawWireSphere(leftPatrolPoint.position, 0.15f);
            Gizmos.DrawWireSphere(rightPatrolPoint.position, 0.15f);
        }
    }
}
