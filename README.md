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

Enemy AI is intentionally not implemented yet.

## Open the demo

1. Open the project in Unity `6000.3.17f1`.
2. Open `Assets/HUSTEnemyAILab/Scenes/00_Foundation.unity`.
3. Enter Play Mode.
4. Use **A/D** or **Arrow Keys** to move and **Space** to jump.

See [SceneGuide](Assets/HUSTEnemyAILab/Documentation/SceneGuide.md) for scene details.

## Testing

Open **Window > General > Test Runner** and run both EditMode and PlayMode tests.

Automated compilation and tests do not replace manual Play Mode verification of input, physics, camera framing, and presentation.
