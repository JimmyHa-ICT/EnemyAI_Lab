using HUST.EnemyAILab.Intermediate;
using UnityEngine;

namespace HUST.EnemyAILab.Production
{
    public sealed class EnemyBrain : MonoBehaviour
    {
        [SerializeField] private EnemySensor sensor;
        [SerializeField] private EnemyMovement movement;
        [SerializeField] private EnemyCombat combat;
        [SerializeField] private SpriteRenderer visual;
        public EnemyState CurrentState { get; private set; } = EnemyState.Patrol;

        private void Awake() { visual ??= GetComponent<SpriteRenderer>(); ApplyColor(); }
        private void Update()
        {
            sensor.Scan();
            switch (CurrentState)
            {
                case EnemyState.Patrol:
                    if (sensor.HasTarget) ChangeState(EnemyState.Chase);
                    break;
                case EnemyState.Chase:
                    if (!sensor.HasTarget) ChangeState(EnemyState.Patrol);
                    else if (sensor.IsTargetInAttackRange()) ChangeState(EnemyState.Attack);
                    break;
                case EnemyState.Attack:
                    if (!sensor.HasTarget) ChangeState(EnemyState.Patrol);
                    else if (!sensor.IsTargetInAttackRange()) ChangeState(EnemyState.Chase);
                    else { combat.TryBeginAttack(); combat.Tick(); }
                    break;
            }
        }

        private void FixedUpdate()
        {
            if (CurrentState == EnemyState.Patrol) movement.Patrol();
            else if (CurrentState == EnemyState.Chase) movement.Chase(sensor.CurrentTarget);
            else movement.Stop();
        }

        private void ChangeState(EnemyState state)
        {
            if (state == CurrentState) return;
            if (CurrentState == EnemyState.Attack) combat.CancelAttack();
            CurrentState = state;
            if (state == EnemyState.Attack) movement.Stop();
            ApplyColor();
        }

        private void ApplyColor()
        {
            if (visual == null) return;
            visual.color = CurrentState == EnemyState.Patrol ? new Color(0.35f, 0.9f, 0.4f) :
                CurrentState == EnemyState.Chase ? new Color(1f, 0.85f, 0.25f) : new Color(1f, 0.3f, 0.25f);
        }

        public void Configure(EnemySensor valueSensor, EnemyMovement valueMovement, EnemyCombat valueCombat, SpriteRenderer renderer)
        { sensor = valueSensor; movement = valueMovement; combat = valueCombat; visual = renderer; ApplyColor(); }
    }
}
