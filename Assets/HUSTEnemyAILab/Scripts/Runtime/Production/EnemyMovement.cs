using UnityEngine;

namespace HUST.EnemyAILab.Production
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private EnemyConfig config;
        [SerializeField] private Transform leftPatrolPoint;
        [SerializeField] private Transform rightPatrolPoint;
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private float arrivalDistance = 0.08f;
        private Rigidbody2D body;
        private bool movingRight = true;

        private void Awake() { body = GetComponent<Rigidbody2D>(); visual ??= GetComponent<SpriteRenderer>(); }

        public void Patrol()
        {
            if (config == null || leftPatrolPoint == null || rightPatrolPoint == null) { Stop(); return; }
            float left = Mathf.Min(leftPatrolPoint.position.x, rightPatrolPoint.position.x);
            float right = Mathf.Max(leftPatrolPoint.position.x, rightPatrolPoint.position.x);
            if (movingRight && body.position.x >= right - arrivalDistance) movingRight = false;
            else if (!movingRight && body.position.x <= left + arrivalDistance) movingRight = true;
            Move(movingRight ? 1f : -1f, config.MoveSpeed);
        }

        public void Chase(Transform target)
        {
            if (config == null || target == null) { Stop(); return; }
            Move(Mathf.Sign(target.position.x - body.position.x), config.ChaseSpeed);
        }

        public void Stop() { if (body != null) body.linearVelocity = new Vector2(0f, body.linearVelocity.y); }
        private void Move(float direction, float speed)
        {
            body.linearVelocity = new Vector2(direction * speed, body.linearVelocity.y);
            if (visual != null && !Mathf.Approximately(direction, 0f)) visual.flipX = direction < 0f;
        }

        public void Configure(EnemyConfig value, Transform left, Transform right, SpriteRenderer renderer)
        { config = value; leftPatrolPoint = left; rightPatrolPoint = right; visual = renderer; }
    }
}
