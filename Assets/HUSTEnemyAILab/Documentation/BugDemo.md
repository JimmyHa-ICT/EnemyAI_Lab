# Production Bug Demonstration

`04_BugDemo` demonstrates one deliberately narrow attack-lifecycle defect:

> The enemy captures a valid target when wind-up starts, the player leaves attack range before impact, and the player still receives damage.

## Reproduce the bug

1. Open `04_BugDemo` and confirm the Slime's `EnemyCombat > Validation Mode` is `UnsafeTeachingDemo`.
2. Enter the Slime's attack range until it turns red and begins its wind-up.
3. Move outside attack range before the impact time.
4. Observe that the player still takes damage. Press **R** to repeat.

To compare the fix, stop Play Mode, select the Slime, change Validation Mode to `Safe`, and repeat the same steps. The attack is cancelled and no damage is applied.

## Root cause

The unsafe mode captures the target at attack start, deliberately ignores cancellation when the brain leaves Attack state, and applies damage at impact without checking current range. The target is still alive and damageable; this is specifically stale range validation, not a null-reference defect.

## Corrected behavior

`Safe` is the production default. Leaving range cancels the pending attack. Impact also revalidates that the captured target still exists, remains the sensor's current target, and is inside attack range. The impact flag is consumed before damage, preventing duplicate execution, and cooldown begins at impact.

| Mode | Cancellation on leaving range | Impact-time range check | Intended use |
|---|---|---|---|
| Safe | Yes | Yes | Production |
| UnsafeTeachingDemo | Intentionally ignored | Intentionally omitted | Classroom bug reproduction only |

`UnsafeTeachingDemo` is intentionally incorrect and must never be the production prefab or Scene 03 default.

## Production concern

Attack wind-up separates intent from impact in time. Targets can move, die, despawn, or be replaced during that interval. Treating attack-start facts as permanently valid produces unfair damage and makes networking, animation timing, and gameplay debugging unreliable.
