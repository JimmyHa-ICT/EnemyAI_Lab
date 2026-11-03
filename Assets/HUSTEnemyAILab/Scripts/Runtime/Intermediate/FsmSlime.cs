using HUST.EnemyAILab.Shared.Combat;
using UnityEngine;

namespace HUST.EnemyAILab.Intermediate
{
    public enum EnemyState { Patrol, Chase, Attack }

    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public sealed class FsmSlime : MonoBehaviour
    {
        [Header("Patrol")]
        [SerializeField] private Transform leftPatrolPoint;
        [SerializeField] private Transform rightPatrolPoint;
        [SerializeField, Min(0f)] private float patrolSpeed = 2f;
        [SerializeField, Min(0.01f)] private float arrivalDistance = 0.08f;
        [SerializeField] private bool startMovingRight = true;

        [Header("Detection and Chase")]
        [SerializeField] private Transform playerTarget;
        [SerializeField, Min(0f)] private float detectionRange = 5f;
        [SerializeField, Min(0f)] private float chaseVerticalTolerance = 1.25f;
        [SerializeField, Min(0f)] private float attackRange = 1.25f;
        [SerializeField, Min(0f)] private float chaseSpeed = 3f;

        [Header("Attack")]
        [SerializeField, Min(0f)] private float attackWindUp = 0.5f;
        [SerializeField, Min(0f)] private float attackCooldown = 1f;
        [SerializeField, Min(1)] private int attackDamage = 1;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer visual;
        [SerializeField] private Color patrolColor = Color.white;
        [SerializeField] private Color chaseColor = new Color(1f, 0.85f, 0.25f);
        [SerializeField] private Color attackColor = new Color(1f, 0.35f, 0.3f);

        private Rigidbody2D body;
        private EnemyState currentState = EnemyState.Patrol;
        private bool movingRight;
        private bool attackWindingUp;
        private float impactTime;
        private float nextAttackTime;
        private float nextPlayerSearchTime;

        public EnemyState CurrentState => currentState;
        public bool IsAttackWindingUp => attackWindingUp;

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            visual ??= GetComponent<SpriteRenderer>();
            movingRight = startMovingRight;
            ApplyStateVisual();
        }

        private void Update()
        {
            FindPlayerIfNeeded();
            switch (currentState)
            {
                case EnemyState.Patrol: UpdatePatrol(); break;
                case EnemyState.Chase: UpdateChase(); break;
                case EnemyState.Attack: UpdateAttack(); break;
            }
        }

        private void FixedUpdate()
        {
            switch (currentState)
            {
                case EnemyState.Patrol: MovePatrol(); break;
                case EnemyState.Chase: MoveTowardPlayer(); break;
                case EnemyState.Attack: StopHorizontalMovement(); break;
            }
        }

        private void UpdatePatrol()
        {
            if (CanSeePlayer()) ChangeState(EnemyState.Chase);
        }

        private void UpdateChase()
        {
            if (!CanSeePlayer())
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (IsPlayerInAttackRange()) ChangeState(EnemyState.Attack);
        }

        private void UpdateAttack()
        {
            if (!CanSeePlayer())
            {
                ChangeState(EnemyState.Patrol);
                return;
            }

            if (!IsPlayerInAttackRange())
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            if (attackWindingUp)
            {
                if (Time.time >= impactTime)
                {
                    attackWindingUp = false;
                    ApplyAttackImpact();
                    nextAttackTime = Time.time + attackCooldown;
                }
                return;
            }

            if (Time.time >= nextAttackTime)
            {
                attackWindingUp = true;
                impactTime = Time.time + attackWindUp;
            }
        }

        private void ChangeState(EnemyState newState)
        {
            if (currentState == newState) return;
            if (currentState == EnemyState.Attack) attackWindingUp = false;
            currentState = newState;
            if (newState == EnemyState.Attack) StopHorizontalMovement();
            ApplyStateVisual();
        }

        private void MovePatrol()
        {
            if (leftPatrolPoint == null || rightPatrolPoint == null)
            {
                StopHorizontalMovement();
                return;
            }

            float left = Mathf.Min(leftPatrolPoint.position.x, rightPatrolPoint.position.x);
            float right = Mathf.Max(leftPatrolPoint.position.x, rightPatrolPoint.position.x);
            if (movingRight && body.position.x >= right - arrivalDistance) movingRight = false;
            else if (!movingRight && body.position.x <= left + arrivalDistance) movingRight = true;
            MoveHorizontally(movingRight ? 1f : -1f, patrolSpeed);
        }

        private void MoveTowardPlayer()
        {
            if (playerTarget == null)
            {
                StopHorizontalMovement();
                return;
            }
            MoveHorizontally(Mathf.Sign(playerTarget.position.x - body.position.x), chaseSpeed);
        }

        private void MoveHorizontally(float direction, float speed)
        {
            body.linearVelocity = new Vector2(direction * speed, body.linearVelocity.y);
            if (!Mathf.Approximately(direction, 0f) && visual != null) visual.flipX = direction < 0f;
        }

        private void StopHorizontalMovement()
        {
            if (body != null) body.linearVelocity = new Vector2(0f, body.linearVelocity.y);
        }

        private bool CanSeePlayer() => playerTarget != null &&
            Mathf.Abs(playerTarget.position.x - transform.position.x) <= detectionRange &&
            Mathf.Abs(playerTarget.position.y - transform.position.y) <= chaseVerticalTolerance;

        private bool IsPlayerInAttackRange() => playerTarget != null &&
            Vector2.Distance(transform.position, playerTarget.position) <= attackRange;

        private void ApplyAttackImpact()
        {
            if (!IsPlayerInAttackRange()) return;
            playerTarget.GetComponent<IDamageable>()?.TakeDamage(attackDamage);
        }

        private void FindPlayerIfNeeded()
        {
            if (playerTarget != null || Time.time < nextPlayerSearchTime) return;
            nextPlayerSearchTime = Time.time + 0.5f;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            playerTarget = player != null ? player.transform : null;
        }

        private void ApplyStateVisual()
        {
            if (visual == null) return;
            visual.color = currentState switch
            {
                EnemyState.Chase => chaseColor,
                EnemyState.Attack => attackColor,
                _ => patrolColor
            };
        }

        private void OnDisable()
        {
            attackWindingUp = false;
            StopHorizontalMovement();
        }

        public void Configure(Transform leftPoint, Transform rightPoint, Transform target)
        {
            leftPatrolPoint = leftPoint;
            rightPatrolPoint = rightPoint;
            playerTarget = target;
        }

        public void ConfigureTuning(float newDetectionRange, float newAttackRange,
            float newWindUp, float newCooldown, int newDamage, float newChaseVerticalTolerance = 1.25f)
        {
            detectionRange = Mathf.Max(0f, newDetectionRange);
            chaseVerticalTolerance = Mathf.Max(0f, newChaseVerticalTolerance);
            attackRange = Mathf.Clamp(newAttackRange, 0f, detectionRange);
            attackWindUp = Mathf.Max(0f, newWindUp);
            attackCooldown = Mathf.Max(0f, newCooldown);
            attackDamage = Mathf.Max(1, newDamage);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.85f, 0.1f);
            Gizmos.DrawWireCube(transform.position,
                new Vector3(detectionRange * 2f, chaseVerticalTolerance * 2f, 0f));
            Gizmos.color = new Color(1f, 0.25f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, attackRange);
            if (leftPatrolPoint == null || rightPatrolPoint == null) return;
            Gizmos.color = Color.white;
            Gizmos.DrawLine(leftPatrolPoint.position, rightPatrolPoint.position);
        }
    }
}
