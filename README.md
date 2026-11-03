# HUST Enemy AI Lab

Educational Unity project for the guest lecture **From OOP to Production Code: What Changes When Developing Professional Games?**

## Environment

- Unity `6000.3.17f1`
- Universal Render Pipeline `17.3.0`
- Input System `1.19.0`

Do not migrate the render pipeline or open the project in an older Unity version.

## Current implementation

Milestone 1 provides the shared 2D platformer foundation:

- Rigidbody2D player movement and jumping
- Ground detection
- Horizontally bounded side-scrolling camera
- Shared health and `IDamageable` contract
- Small arena with ground and platforms
- Placeholder sprites and simple on-screen instructions
- EditMode and PlayMode test assemblies

Milestone 2 adds the first deliberately simple Enemy AI implementation:

- One `PrototypeSlime` MonoBehaviour
- Horizontal patrol between two visible points
- Configurable patrol speed
- Direction reversal and visual flipping
- No State Machine or generic Enemy framework

Milestone 3 adds an intermediate enum-based FSM implementation:

- One `FsmSlime` MonoBehaviour containing Patrol, Chase, and Attack behavior
- Explicit range-based state transitions with state-specific color feedback
- Attack wind-up, impact-time target/range validation, and cooldown
- A separate scene and prefab that leave the prototype demonstration intact

Milestone 4 preserves that behavior while separating production-oriented responsibilities:

- `EnemyBrain` owns the enum state and all transitions.
- `EnemySensor`, `EnemyMovement`, and `EnemyCombat` own queries, motion, and attack timing respectively.
- `EnemyHealth` owns damage/death, while `EnemyConfig` stores shared tuning without runtime state.
- A minimal **F** key player attack demonstrates hit feedback and enemy death.

Milestone 5 adds `04_BugDemo`, a deterministic comparison of the production-safe attack lifecycle and an explicitly incorrect `UnsafeTeachingDemo` mode. The unsafe mode captures a target at wind-up and deliberately omits range revalidation at impact. `Safe` remains the production prefab and Scene 03 default. See `Assets/HUSTEnemyAILab/Documentation/BugDemo.md` for reproduction steps and root-cause analysis.

## Open the demo

1. Open the project in Unity `6000.3.17f1`.
2. Open scenes `00_Foundation` through `04_BugDemo` under `Assets/HUSTEnemyAILab/Scenes`.
3. Enter Play Mode.
4. Use **A/D** or **Arrow Keys** to move and **Space** to jump.
5. In either Enemy AI scene, press **R** to reset the demonstration.

See [SceneGuide](Assets/HUSTEnemyAILab/Documentation/SceneGuide.md) for scene details.

## Testing

Open **Window > General > Test Runner** and run both EditMode and PlayMode tests.

Automated compilation and tests do not replace manual Play Mode verification of input, physics, camera framing, and presentation.
