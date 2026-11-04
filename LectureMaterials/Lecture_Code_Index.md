# Lecture Code Extraction Plan

Status: **plan prepared for review; extraction and PNG rendering not yet run**.

This index maps proposed lecture excerpts to the actual Unity source. Runtime files under
`Assets/HUSTEnemyAILab/Scripts/Runtime` are read-only inputs to this workflow. Extracted files will
be written under `LectureMaterials/CodeExtracts`, and rendered images under
`LectureMaterials/CodeImages`.

## Source integrity rules

- Extract only complete, contiguous source-line ranges; do not rewrite or shorten code inside a range.
- Preserve indentation, blank lines, identifiers, comments, and punctuation exactly.
- Store source path and inclusive line range with every extract.
- Validate every extract by comparing it to the same range read again from the source file.
- Treat excerpts as partial source views, not independently compilable units, unless explicitly verified.
- If a teaching topic is too large, split it across slides at method boundaries rather than editing it.

## Recommended slide excerpts

| ID | Stage | Source file | Exact lines | Teaching purpose | Engineering concept | Slide note |
|---|---|---|---:|---|---|---|
| P1 | Prototype | `Prototype/PrototypeSlime.cs` | 32-55 | Show the complete fixed-step patrol loop. | Rigidbody movement, boundary normalization, direction state, sprite orientation. | One slide at 24 lines; highlight 40-41, then 52-54. |
| P2 | Prototype | `Prototype/PrototypeSlime.cs` | 40-54 | Focus on direction reversal at both patrol boundaries. | Threshold checks, mutually exclusive reversal, direction-to-velocity mapping. | One focused slide; overlaps P1 intentionally as an alternate teaching view. |
| I1 | Intermediate | `Intermediate/FsmSlime.cs` | 6-6 | Introduce the state vocabulary. | Enum-based finite state representation. | Pair visually with I2; this one-line excerpt is not independently compilable. |
| I2 | Intermediate | `Intermediate/FsmSlime.cs` | 55-74 | Show frame update and physics update dispatch. | State dispatch; separation of decision timing from physics movement. | One slide at 20 lines; highlight the two switches separately. |
| I3 | Intermediate | `Intermediate/FsmSlime.cs` | 76-90 | Show Patrol-to-Chase and Chase-to-Patrol/Attack decisions. | Guard conditions and directed FSM transitions. | One slide. |
| I4 | Intermediate | `Intermediate/FsmSlime.cs` | 92-122 | Show Attack exit conditions and timed wind-up/impact/cooldown behavior. | Temporal state within a state; early returns; attack lifecycle. | 31 lines: split into two slides using the same exact range, first highlighting 94-104 and second 106-121, or use I4a/I4b below. |
| I4a | Intermediate | `Intermediate/FsmSlime.cs` | 92-104 | Explain exits from Attack. | Transition priority and guard clauses. | Preferred first half of I4. |
| I4b | Intermediate | `Intermediate/FsmSlime.cs` | 106-122 | Explain attack wind-up, impact, and cooldown. | Time-driven lifecycle and deferred effects. | Preferred second half of I4. |
| I5 | Intermediate | `Intermediate/FsmSlime.cs` | 124-131 | Centralize transition side effects. | Transition gateway, cancellation, entry action, presentation feedback. | One slide. |
| I6 | Intermediate | `Intermediate/FsmSlime.cs` | 133-146 | Show patrol behavior inside the FSM version. | Reusable movement behavior under state control. | One slide. |
| I7 | Intermediate | `Intermediate/FsmSlime.cs` | 148-167 | Show Chase movement and shared movement helpers. | Target steering, helper extraction, defensive stopping. | One slide. |
| I8 | Intermediate | `Intermediate/FsmSlime.cs` | 169-180 | Show detection, attack-range checks, and damage application. | Boolean queries, spatial tests, interface-based damage. | One slide. |
| R1 | Production | `Production/EnemyBrain.cs` | 6-12 | Introduce the production coordinator and its collaborators. | Composition and orchestration. | Pair with R2. |
| R2 | Production | `Production/EnemyBrain.cs` | 15-41 | Show brain coordination across sensing, combat, decisions, and movement. | Thin coordinator, state dispatch, Update/FixedUpdate responsibility split. | 27 lines: use two slides, 15-34 then 36-41, without modifying either range. |
| R2a | Production | `Production/EnemyBrain.cs` | 15-34 | Show sensing/combat ticks and state decisions. | Orchestration and collaborator delegation. | Preferred first half of R2. |
| R2b | Production | `Production/EnemyBrain.cs` | 36-41 | Show state-to-movement dispatch. | Physics-loop delegation. | Preferred second half of R2. |
| R3 | Production | `Production/EnemyBrain.cs` | 43-50 | Show transition cleanup and entry behavior. | Centralized transition side effects and attack cancellation. | One slide. |
| R4 | Production | `Production/EnemySensor.cs` | 5-23 | Show sensor ownership of target acquisition and range queries. | Single responsibility, physics query, encapsulated perception API. | One slide at 19 lines. |
| R5 | Production | `Production/EnemyMovement.cs` | 18-39 | Show movement ownership for patrol, chase, stop, and facing. | Single responsibility, target steering, shared movement primitive. | One slide at 22 lines. |
| R6 | Production | `Production/EnemyCombat.cs` | 6-10 | Introduce safe versus intentionally unsafe validation modes. | Explicit policy selection for a teaching experiment. | Pair with R7/R8; not independently compilable. |
| R7 | Production | `Production/EnemyCombat.cs` | 24-31 | Show attack admission and wind-up setup. | Preconditions, target snapshot, deferred impact. | One slide. |
| R8 | Production | `Production/EnemyCombat.cs` | 33-45 | Show impact-time revalidation, damage, and cooldown. | Attack lifecycle, stale-target defense, interface-based damage. | One slide; highlight 39-44. |
| R9 | Production | `Production/EnemyCombat.cs` | 47-52 | Show safe attack cancellation. | Lifecycle cleanup and policy-dependent behavior. | Pair with R8 if space permits. |
| R10 | Production | `Production/EnemyHealth.cs` | 8-27 | Show health state, damage handling, and death dispatch. | Interface implementation, invariant clamping, event-driven lifecycle. | Optional production-responsibility slide; 20 lines. |
| R11 | Production | `Production/EnemyHealth.cs` | 37-44 | Show death side effects. | Idempotence, component shutdown, event notification. | Optional focused slide. |
| R12 | Production | `Production/EnemyConfig.cs` | 5-16 | Show the ScriptableObject and serialized tuning surface. | Data-driven configuration, reusable assets, constrained inspector values. | Primary configuration slide. |
| R13 | Production | `Production/EnemyConfig.cs` | 18-30 | Show validated runtime/test configuration. | Boundary validation and configuration invariants. | Optional follow-up; 13 lines. |

All paths in the table are relative to `Assets/HUSTEnemyAILab/Scripts/Runtime`.

## Planned extract format

Each approved excerpt will be copied verbatim to a `.cs` file named with its ID, source basename,
and line range, for example `P1_PrototypeSlime_L032-L055.cs`. A sidecar metadata record in this index
will contain the source path, inclusive range, teaching purpose, concept, highlight lines, and a
validation digest. The extract body will contain only original source text so metadata cannot be
mistaken for project code.

## Rendering method

Use a local, dependency-free HTML-to-PNG pipeline:

1. Read the approved inclusive line range directly from the UTF-8 source file.
2. HTML-escape the text without normalizing or rewriting it.
3. Apply deterministic C# token spans for presentation only; retain the underlying characters exactly.
4. Render a 16:9 card at 2560 x 1440 with a dark background, Cascadia Mono, visible source filename,
   optional original line numbers, and CSS classes for selected-line highlights.
5. Capture the card as PNG using the already-installed Node Playwright runtime and local Chrome or Edge.
6. Compare extracted plain text reconstructed from the render input with the source range before capture.

Locally verified capabilities:

- Node runtime and Playwright are installed.
- Google Chrome and Microsoft Edge executables are installed.
- Cascadia Mono and Cascadia Code fonts are installed.
- Pillow and `System.Drawing` are available as fallbacks for image inspection.
- No standalone Pygments, Shiki, Highlight.js, or Prism package is installed; no package installation is
  required for the proposed method.

## Technical limitations

- Excerpts beginning inside a class or method are intentionally partial and normally will not compile alone.
- Syntax coloring will be presentation markup, not compiler-semantic classification; source characters remain unchanged.
- Long source lines may require a smaller font or a wider crop. They will not be wrapped if wrapping would make
  line-number correspondence ambiguous.
- P1/P2 and I4/I4a/I4b are alternate views. The final deck should choose the broad view or split views to avoid duplication.
- Production `EnemyBrain` reuses `EnemyState` declared in the Intermediate namespace; R1 alone does not show that declaration.
- `EnemyHealth` was inspected because it is a requested production source, but it is supplementary to the five specifically
  requested production teaching themes.

## Approval gate

No extracts or PNGs will be generated until the excerpt selection and rendering approach are approved.

## Generated asset manifest

Generated from the approved ranges. SHA-256 values are recorded in hexadecimal.

### Source file hashes

| Source file | SHA-256 |
|---|---|
| `Prototype/PrototypeSlime.cs` | `d2101e57680c65e2db375c6d3533ca4c217526344487fb67c097c4080b7450de` |
| `Intermediate/FsmSlime.cs` | `5a62349c315cbecc08dbe91ec7f51ab2e3e33d9de70cdbb8ea7cad4d11386740` |
| `Production/EnemyBrain.cs` | `91d84868926aadebd16de22b22d768f12ee79a2dc90a385f18c1d83244584159` |
| `Production/EnemySensor.cs` | `144e8859711640656805f4bfbf0ca0480f61dad1eab839df12136a8fc5ffa2f7` |
| `Production/EnemyMovement.cs` | `c0869629ce586f0afa68e770f8f8a3412676b7aa6acba15fc42d0d6fbed3d169` |
| `Production/EnemyCombat.cs` | `22859627bad8957181ce5af283c4ff14348b1dcfd2f6e3561b9c1a5746ab0da8` |
| `Production/EnemyHealth.cs` | `da6dcabab56eebc79d96641dc12102ad56cd2180fbebd3f9da7cdbc2204c6249` |
| `Production/EnemyConfig.cs` | `f1ecfaea03849015aa00d85ed67dbf123b144cbacf496672f71e98c07d1e4060` |

### Extract and image validation

| ID | Stage | Source file | Range | Extracted filename | Rendered image filename | Teaching purpose | Highlighted source lines | Validation | Excerpt SHA-256 |
|---|---|---|---:|---|---|---|---|---|---|
| P1 | Prototype | `Prototype/PrototypeSlime.cs` | 32-55 | `P1_PrototypeSlime_L032-L055.cs` | `P1_PrototypeSlime_L032-L055.png` | Complete fixed-step patrol loop. | 40, 41, 52, 53, 54 | PASS — exact bytes, 24 lines, 2560×1440 | `6c61fff2aabca2be9b4a089e6e3012d88af5dd53741877349220e43dd3e127e0` |
| P2 | Prototype | `Prototype/PrototypeSlime.cs` | 40-54 | `P2_PrototypeSlime_L040-L054.cs` | `P2_PrototypeSlime_L040-L054.png` | Direction reversal at both patrol boundaries. | 43, 44, 45, 47, 48, 49, 52, 53, 54 | PASS — exact bytes, 15 lines, 2560×1440 | `894c621908cafed865abb2cf2c91ee10842b373b8c90cfec86f9159a6612e05a` |
| I1 | Intermediate | `Intermediate/FsmSlime.cs` | 6-6 | `I1_FsmSlime_L006-L006.cs` | `I1_FsmSlime_L006-L006.png` | Introduce the state vocabulary. | 6 | PASS — exact bytes, 1 lines, 2560×1440 | `c2046584415670755dd2ad5080c475ec8a98006ddd1b39ca554d6a65b053e028` |
| I2 | Intermediate | `Intermediate/FsmSlime.cs` | 55-74 | `I2_FsmSlime_L055-L074.cs` | `I2_FsmSlime_L055-L074.png` | Frame update and physics update dispatch. | 58, 59, 60, 61, 62, 68, 69, 70, 71, 72 | PASS — exact bytes, 20 lines, 2560×1440 | `90f9d1d73b6c9b854a33f420a0fe252ec870e98973b3ae75f30e3dfa1615f842` |
| I3 | Intermediate | `Intermediate/FsmSlime.cs` | 76-90 | `I3_FsmSlime_L076-L090.cs` | `I3_FsmSlime_L076-L090.png` | Patrol-to-Chase and Chase-to-Patrol/Attack decisions. | 78, 83, 84, 85, 89 | PASS — exact bytes, 15 lines, 2560×1440 | `4910681d443dc5c57f95c2b8940e629d5de7571e7cc51b42d8cb6955dd9e4886` |
| I4 | Intermediate | `Intermediate/FsmSlime.cs` | 92-122 | `I4_FsmSlime_L092-L122.cs` | `I4_FsmSlime_L092-L122.png` | Attack exits and timed wind-up, impact, and cooldown. | 94, 96, 100, 102, 106, 108, 110, 111, 112, 117, 119, 120 | PASS — exact bytes, 31 lines, 2560×1440 | `354e28d78eaef2ed3fa185bc99bc78ae33d18be8a341761e614483f84cce6660` |
| I4a | Intermediate | `Intermediate/FsmSlime.cs` | 92-104 | `I4a_FsmSlime_L092-L104.cs` | `I4a_FsmSlime_L092-L104.png` | Exit conditions from Attack. | 94, 96, 100, 102 | PASS — exact bytes, 13 lines, 2560×1440 | `b189d87acd8b4dae5dedcb98d96cf03d127cd68c3c80f26abdc97a92f70e1c11` |
| I4b | Intermediate | `Intermediate/FsmSlime.cs` | 106-122 | `I4b_FsmSlime_L106-L122.cs` | `I4b_FsmSlime_L106-L122.png` | Attack wind-up, impact, and cooldown. | 106, 108, 110, 111, 112, 117, 119, 120 | PASS — exact bytes, 17 lines, 2560×1440 | `14aa18b8d4324ba647624ced331365526a3f971077327aed5bb0a29b4a647262` |
| I5 | Intermediate | `Intermediate/FsmSlime.cs` | 124-131 | `I5_FsmSlime_L124-L131.cs` | `I5_FsmSlime_L124-L131.png` | Centralized transition side effects. | 126, 127, 128, 129, 130 | PASS — exact bytes, 8 lines, 2560×1440 | `ae19a94563d677ff0b131735cf13a20de1027b239682a6c2f9010cb295c6ca2d` |
| I6 | Intermediate | `Intermediate/FsmSlime.cs` | 133-146 | `I6_FsmSlime_L133-L146.cs` | `I6_FsmSlime_L133-L146.png` | Patrol behavior inside the FSM version. | 141, 142, 143, 144, 145 | PASS — exact bytes, 14 lines, 2560×1440 | `4e7a91cf2d73ffcd273d5a17fe5d82e1740a2f8cab217df05e60b12539bff0e9` |
| I7 | Intermediate | `Intermediate/FsmSlime.cs` | 148-167 | `I7_FsmSlime_L148-L167.cs` | `I7_FsmSlime_L148-L167.png` | Chase movement and shared movement helpers. | 150, 152, 155, 160, 161, 166 | PASS — exact bytes, 20 lines, 2560×1440 | `4d821f0cbc62f40592527c608a00a6baedf6f9bf0be939c60bc6b153e2f0acc0` |
| I8 | Intermediate | `Intermediate/FsmSlime.cs` | 169-180 | `I8_FsmSlime_L169-L180.cs` | `I8_FsmSlime_L169-L180.png` | Detection, attack-range checks, and damage application. | 169, 170, 171, 173, 174, 178, 179 | PASS — exact bytes, 12 lines, 2560×1440 | `a8b46416d4fe99275dab663c6355b7f9c855f869c1a0d3d874e6ef5ede73d95d` |
| R1 | Production | `Production/EnemyBrain.cs` | 6-12 | `R1_EnemyBrain_L006-L012.cs` | `R1_EnemyBrain_L006-L012.png` | Introduce the production coordinator and collaborators. | 8, 9, 10, 12 | PASS — exact bytes, 7 lines, 2560×1440 | `d4954edfe657b1053bbbc2409f3ded96f495252aea0cc010ad6dd89eaab6a22d` |
| R2 | Production | `Production/EnemyBrain.cs` | 15-41 | `R2_EnemyBrain_L015-L041.cs` | `R2_EnemyBrain_L015-L041.png` | Brain coordination across sensing, combat, decisions, and movement. | 17, 18, 19, 22, 25, 26, 29, 30, 31, 38, 39, 40 | PASS — exact bytes, 27 lines, 2560×1440 | `20f82c825884ed6aad9eed0b7ff2917cc148861100cb5a8afa7cc58bf21d4889` |
| R2a | Production | `Production/EnemyBrain.cs` | 15-34 | `R2a_EnemyBrain_L015-L034.cs` | `R2a_EnemyBrain_L015-L034.png` | Sensing/combat ticks and state decisions. | 17, 18, 19, 22, 25, 26, 29, 30, 31 | PASS — exact bytes, 20 lines, 2560×1440 | `780dfb10cdbbd05975bd6af7c9c0248fba19f8086c5a7d4145e195a7d3c4fe7a` |
| R2b | Production | `Production/EnemyBrain.cs` | 36-41 | `R2b_EnemyBrain_L036-L041.cs` | `R2b_EnemyBrain_L036-L041.png` | State-to-movement dispatch. | 38, 39, 40 | PASS — exact bytes, 6 lines, 2560×1440 | `47454f2b50ddeaede6c285934725ce059f871d1914bc970f5fff80e3b4712d2a` |
| R3 | Production | `Production/EnemyBrain.cs` | 43-50 | `R3_EnemyBrain_L043-L050.cs` | `R3_EnemyBrain_L043-L050.png` | Transition cleanup and entry behavior. | 45, 46, 47, 48, 49 | PASS — exact bytes, 8 lines, 2560×1440 | `39c578ef7f48378cdbe88b1d6bc806e58e32e6839e3dfed0a2302f259172dba8` |
| R4 | Production | `Production/EnemySensor.cs` | 5-23 | `R4_EnemySensor_L005-L023.cs` | `R4_EnemySensor_L005-L023.png` | Sensor ownership of target acquisition and range queries. | 12, 14, 15, 16, 17, 18, 21, 22, 23 | PASS — exact bytes, 19 lines, 2560×1440 | `07d50c0a3dadd6f52b5645f8065d60ad63a222c3aeec01262be4b32be38cb7aa` |
| R5 | Production | `Production/EnemyMovement.cs` | 18-39 | `R5_EnemyMovement_L018-L039.cs` | `R5_EnemyMovement_L018-L039.png` | Movement ownership for patrol, chase, stop, and facing. | 20, 21, 22, 23, 24, 25, 30, 31, 34, 37, 38 | PASS — exact bytes, 22 lines, 2560×1440 | `f4fe56d17eb3e3dd11d3e270cc6a6c2764c214c3204aadff3318c6fa096f536f` |
| R6 | Production | `Production/EnemyCombat.cs` | 6-10 | `R6_EnemyCombat_L006-L010.cs` | `R6_EnemyCombat_L006-L010.png` | Introduce safe versus intentionally unsafe validation modes. | 6, 8, 9 | PASS — exact bytes, 5 lines, 2560×1440 | `1766edccf6ae1b18f776d3dd7445598719dc52e4f738e9710d9b0e3f630f9c1e` |
| R7 | Production | `Production/EnemyCombat.cs` | 24-31 | `R7_EnemyCombat_L024-L031.cs` | `R7_EnemyCombat_L024-L031.png` | Attack admission and wind-up setup. | 26, 27, 28, 29 | PASS — exact bytes, 8 lines, 2560×1440 | `bdd78554004228b2f4a33d4a871ea7aca6d8e91605797b31d2f8cbe024cc7acd` |
| R8 | Production | `Production/EnemyCombat.cs` | 33-45 | `R8_EnemyCombat_L033-L045.cs` | `R8_EnemyCombat_L033-L045.png` | Impact-time revalidation, damage, and cooldown. | 35, 36, 37, 38, 39, 40, 41, 42, 43, 44 | PASS — exact bytes, 13 lines, 2560×1440 | `417c069baf22f7bd722266e6766dd1daff762ad848cea2f634604b598c0437c1` |
| R9 | Production | `Production/EnemyCombat.cs` | 47-52 | `R9_EnemyCombat_L047-L052.cs` | `R9_EnemyCombat_L047-L052.png` | Safe attack cancellation. | 49, 50, 51 | PASS — exact bytes, 6 lines, 2560×1440 | `1611e5279681f9b52795a76ee80b2f9f0ee81acbf99c658257e734834a3d1409` |
| R10 | Production | `Production/EnemyHealth.cs` | 8-27 | `R10_EnemyHealth_L008-L027.cs` | `R10_EnemyHealth_L008-L027.png` | Health state, damage handling, and death dispatch. | 15, 16, 17, 20, 22, 24, 25, 26 | PASS — exact bytes, 20 lines, 2560×1440 | `c7fc25d52db014737bc8108868a8b58d0e84ba10d62d6d4e072d86f373bb915b` |
| R11 | Production | `Production/EnemyHealth.cs` | 37-44 | `R11_EnemyHealth_L037-L044.cs` | `R11_EnemyHealth_L037-L044.png` | Death side effects. | 39, 40, 41, 42, 43 | PASS — exact bytes, 8 lines, 2560×1440 | `654845d56d4fe9d17be178099d6b69b9481c8b3e244cb135944c2a09762491cb` |
| R12 | Production | `Production/EnemyConfig.cs` | 5-16 | `R12_EnemyConfig_L005-L016.cs` | `R12_EnemyConfig_L005-L016.png` | ScriptableObject and serialized tuning surface. | 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 16 | PASS — exact bytes, 12 lines, 2560×1440 | `9629cf174b42303cf63d9d86265a4496fd5711fd7d9baccc427a2055d3ad60c1` |
| R13 | Production | `Production/EnemyConfig.cs` | 18-30 | `R13_EnemyConfig_L018-L030.cs` | `R13_EnemyConfig_L018-L030.png` | Validated runtime/test configuration. | 21, 22, 23, 24, 25, 26, 27, 28, 29 | PASS — exact bytes, 13 lines, 2560×1440 | `75c322a988711c10de1f5e39270429ded237279d9ec98dc9241d827004ee15f3` |

### Final verification

- Expected extracts: 27; present and validated: 27.
- Expected PNG images: 27; present and validated: 27.
- Every image is 2560×1440 pixels.
- Every rendered row was compared with its source line after removing presentation markup.
- No image reported horizontal or vertical clipping.
- I4 and R2 have split alternatives (I4a/I4b and R2a/R2b) for maximum classroom readability; the broad versions are also included.
