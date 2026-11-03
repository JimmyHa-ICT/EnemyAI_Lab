using System.IO;
using HUST.EnemyAILab.Player;
using HUST.EnemyAILab.Production;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUST.EnemyAILab.Editor
{
    public static class ModularSceneBuilder
    {
        private const string Root = "Assets/HUSTEnemyAILab";
        private const string SourceScene = Root + "/Scenes/02_EnumFSM.unity";
        private const string TargetScene = Root + "/Scenes/03_ModularAI.unity";
        private const string PrefabPath = Root + "/Prefabs/Enemies/Production/ProductionSlime.prefab";
        private const string ConfigPath = Root + "/Configuration/ProductionSlimeConfig.asset";
        private const string SlimeSprite = Root + "/Art/Sprites/PrototypeSlimePlaceholder.png";
        private const string MarkerSprite = Root + "/Art/Sprites/PatrolMarkerPlaceholder.png";

        [MenuItem("HUST Enemy AI Lab/Build Milestone 4 Modular AI")]
        public static void Build()
        {
            EnsureFolder(Root + "/Prefabs/Enemies", "Production");
            EnemyConfig config = CreateConfig();
            Sprite slime = AssetDatabase.LoadAssetAtPath<Sprite>(SlimeSprite);
            Sprite marker = AssetDatabase.LoadAssetAtPath<Sprite>(MarkerSprite);
            if (slime == null || marker == null || !File.Exists(Path.GetFullPath(SourceScene)))
                throw new InvalidDataException("Milestone 4 requires the accepted Milestone 3 scene and visuals.");
            CreatePrefab(slime, config);
            CreateScene(config, marker);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Milestone 4 modular AI generated at {TargetScene}");
        }

        private static void EnsureFolder(string parent, string child)
        {
            if (!AssetDatabase.IsValidFolder(parent + "/" + child)) AssetDatabase.CreateFolder(parent, child);
        }

        private static EnemyConfig CreateConfig()
        {
            EnemyConfig config = AssetDatabase.LoadAssetAtPath<EnemyConfig>(ConfigPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<EnemyConfig>();
                AssetDatabase.CreateAsset(config, ConfigPath);
            }
            config.Configure(2f, 3f, 5f, 1.25f, 1f, 0.5f, 1, 3);
            EditorUtility.SetDirty(config);
            return config;
        }

        private static void CreatePrefab(Sprite sprite, EnemyConfig config)
        {
            var root = new GameObject("ProductionSlime");
            root.layer = LayerMask.NameToLayer("Enemy");
            var renderer = root.AddComponent<SpriteRenderer>(); renderer.sprite = sprite; renderer.sortingOrder = 10;
            var body = root.AddComponent<Rigidbody2D>(); body.gravityScale = 3f; body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate; body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            root.AddComponent<CapsuleCollider2D>().size = new Vector2(1.05f, 0.65f);
            var sensor = root.AddComponent<EnemySensor>();
            var movement = root.AddComponent<EnemyMovement>();
            var combat = root.AddComponent<EnemyCombat>();
            var brain = root.AddComponent<EnemyBrain>();
            var health = root.AddComponent<EnemyHealth>();
            sensor.Configure(config, LayerMask.GetMask("Player"));
            movement.Configure(config, null, null, renderer);
            combat.Configure(config, sensor);
            brain.Configure(sensor, movement, combat, renderer);
            health.Configure(config, renderer, new Behaviour[] { brain, movement, combat, sensor });
            PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);
        }

        private static void CreateScene(EnemyConfig config, Sprite markerSprite)
        {
            Scene scene = EditorSceneManager.OpenScene(SourceScene, OpenSceneMode.Single);
            GameObject oldEnemy = GameObject.Find("Enemy Setup");
            if (oldEnemy != null) Object.DestroyImmediate(oldEnemy);

            GameObject player = GameObject.FindWithTag("Player");
            if (player == null) throw new InvalidDataException("The accepted scene has no Player.");
            var playerCombat = player.GetComponent<PlayerCombat>() ?? player.AddComponent<PlayerCombat>();
            playerCombat.Configure(LayerMask.GetMask("Enemy"));

            var root = new GameObject("Enemy Setup");
            var points = new GameObject("Patrol Points"); points.transform.SetParent(root.transform);
            Transform left = Marker(points.transform, "PatrolPoint_Left", markerSprite, new Vector3(-4f, -2f));
            Transform right = Marker(points.transform, "PatrolPoint_Right", markerSprite, new Vector3(4f, -2f));
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            var enemy = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform);
            enemy.transform.position = new Vector3(-1f, -2.175f, 0f);
            enemy.GetComponent<EnemyMovement>().Configure(config, left, right, enemy.GetComponent<SpriteRenderer>());
            EditorUtility.SetDirty(enemy.GetComponent<EnemyMovement>());

            Text title = GameObject.Find("UI/Title")?.GetComponent<Text>();
            if (title != null) title.text = "SCENE 03  |  MODULAR AI: PATROL / CHASE / ATTACK";
            Text controls = GameObject.Find("UI/Controls")?.GetComponent<Text>();
            if (controls != null) controls.text = "Move: A / D or Arrow Keys     Jump: Space     Attack: F     Reset: R\nGreen: Patrol     Yellow: Chase     Red: Attack     White: Hit";

            EditorSceneManager.SaveScene(scene, TargetScene);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(Root + "/Scenes/00_Foundation.unity", true),
                new EditorBuildSettingsScene(Root + "/Scenes/01_Prototype.unity", true),
                new EditorBuildSettingsScene(SourceScene, true),
                new EditorBuildSettingsScene(TargetScene, true)
            };
        }

        private static Transform Marker(Transform parent, string name, Sprite sprite, Vector3 position)
        {
            var marker = new GameObject(name); marker.transform.SetParent(parent); marker.transform.position = position;
            var renderer = marker.AddComponent<SpriteRenderer>(); renderer.sprite = sprite; renderer.sortingOrder = 5;
            return marker.transform;
        }
    }
}
