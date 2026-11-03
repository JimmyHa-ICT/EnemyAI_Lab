using System.IO;
using HUST.EnemyAILab.CameraSystem;
using HUST.EnemyAILab.Intermediate;
using HUST.EnemyAILab.Shared.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUST.EnemyAILab.Editor
{
    public static class FsmSceneBuilder
    {
        private const string Root = "Assets/HUSTEnemyAILab";
        private const string FoundationScene = Root + "/Scenes/00_Foundation.unity";
        private const string PrototypeScene = Root + "/Scenes/01_Prototype.unity";
        private const string FsmScene = Root + "/Scenes/02_EnumFSM.unity";
        private const string ArenaPrefab = Root + "/Prefabs/Environment/DemoArena.prefab";
        private const string PlayerPrefab = Root + "/Prefabs/Player/Player.prefab";
        private const string SlimePrefab = Root + "/Prefabs/Enemies/Intermediate/FsmSlime.prefab";
        private const string SlimeSprite = Root + "/Art/Sprites/PrototypeSlimePlaceholder.png";
        private const string MarkerSprite = Root + "/Art/Sprites/PatrolMarkerPlaceholder.png";

        [MenuItem("HUST Enemy AI Lab/Build Milestone 3 Enum FSM")]
        public static void BuildMilestoneThree()
        {
            EnsureFolder(Root + "/Prefabs/Enemies", "Intermediate");
            Sprite slimeSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SlimeSprite);
            Sprite markerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(MarkerSprite);
            GameObject arenaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArenaPrefab);
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefab);
            if (slimeSprite == null || markerSprite == null || arenaPrefab == null || playerPrefab == null)
                throw new InvalidDataException("Milestone 3 requires the accepted Milestone 1 and 2 assets.");

            CreateSlimePrefab(slimeSprite);
            CreateScene(arenaPrefab, playerPrefab, markerSprite);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Milestone 3 enum FSM generated at {FsmScene}");
        }

        private static void EnsureFolder(string parent, string child)
        {
            if (!AssetDatabase.IsValidFolder(parent + "/" + child)) AssetDatabase.CreateFolder(parent, child);
        }

        private static void CreateSlimePrefab(Sprite sprite)
        {
            var slime = new GameObject("FsmSlime");
            slime.layer = 10;
            var renderer = slime.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 10;
            var body = slime.AddComponent<Rigidbody2D>();
            body.gravityScale = 3f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            slime.AddComponent<CapsuleCollider2D>().size = new Vector2(1.05f, 0.65f);
            var fsm = slime.AddComponent<FsmSlime>();
            var data = new SerializedObject(fsm);
            data.FindProperty("visual").objectReferenceValue = renderer;
            data.FindProperty("patrolSpeed").floatValue = 2f;
            data.FindProperty("chaseSpeed").floatValue = 3f;
            data.FindProperty("detectionRange").floatValue = 5f;
            data.FindProperty("attackRange").floatValue = 1.25f;
            data.FindProperty("attackWindUp").floatValue = 0.5f;
            data.FindProperty("attackCooldown").floatValue = 1f;
            data.ApplyModifiedPropertiesWithoutUndo();
            PrefabUtility.SaveAsPrefabAsset(slime, SlimePrefab);
            Object.DestroyImmediate(slime);
        }

        private static void CreateScene(GameObject arenaPrefab, GameObject playerPrefab, Sprite markerSprite)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "02_EnumFSM";
            var environment = new GameObject("Environment");
            ((GameObject)PrefabUtility.InstantiatePrefab(arenaPrefab, environment.transform)).name = "DemoArena";
            var playerRoot = new GameObject("Player Setup");
            var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, playerRoot.transform);
            player.transform.position = new Vector3(-10f, -1.7f, 0f);
            CreateEnemy(player.transform, markerSprite);
            CreateCamera(player.transform);
            CreateUi();
            new GameObject("Demo Controls").AddComponent<DemoSceneReset>();
            EditorSceneManager.SaveScene(scene, FsmScene);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(FoundationScene, true),
                new EditorBuildSettingsScene(PrototypeScene, true),
                new EditorBuildSettingsScene(FsmScene, true)
            };
        }

        private static void CreateEnemy(Transform player, Sprite markerSprite)
        {
            var root = new GameObject("Enemy Setup");
            var points = new GameObject("Patrol Points");
            points.transform.SetParent(root.transform);
            Transform left = CreateMarker(points.transform, "PatrolPoint_Left", markerSprite, new Vector3(-4f, -2f));
            Transform right = CreateMarker(points.transform, "PatrolPoint_Right", markerSprite, new Vector3(4f, -2f));
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(SlimePrefab);
            var slime = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root.transform);
            slime.transform.position = new Vector3(-1f, -2.175f, 0f);
            var data = new SerializedObject(slime.GetComponent<FsmSlime>());
            data.FindProperty("leftPatrolPoint").objectReferenceValue = left;
            data.FindProperty("rightPatrolPoint").objectReferenceValue = right;
            data.FindProperty("playerTarget").objectReferenceValue = player;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Transform CreateMarker(Transform parent, string name, Sprite sprite, Vector3 position)
        {
            var marker = new GameObject(name);
            marker.transform.SetParent(parent);
            marker.transform.position = position;
            var renderer = marker.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 5;
            return marker.transform;
        }

        private static void CreateCamera(Transform player)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(-7f, 0f, -10f);
            var camera = cameraObject.AddComponent<UnityEngine.Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5.625f;
            camera.backgroundColor = new Color32(120, 195, 230, 255);
            camera.clearFlags = CameraClearFlags.SolidColor;
            var follow = cameraObject.AddComponent<SideScrollingCamera>();
            var data = new SerializedObject(follow);
            data.FindProperty("target").objectReferenceValue = player;
            data.FindProperty("fixedY").floatValue = 0f;
            data.FindProperty("minimumX").floatValue = -7f;
            data.FindProperty("maximumX").floatValue = 7f;
            data.FindProperty("smoothTime").floatValue = 0.12f;
            data.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateUi()
        {
            var root = new GameObject("UI");
            root.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            root.AddComponent<GraphicRaycaster>();
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            CreateText(root.transform, "Title", "SCENE 02  |  ENUM FSM: PATROL / CHASE / ATTACK", font,
                24, new Vector2(18f, -16f), new Vector2(800f, 44f), false);
            CreateText(root.transform, "Controls",
                "Move: A / D or Arrow Keys     Jump: Space     Reset: R\nWhite: Patrol     Yellow: Chase     Red: Attack",
                font, 19, new Vector2(18f, 16f), new Vector2(800f, 70f), true);
        }

        private static void CreateText(Transform parent, string name, string content, Font font,
            int size, Vector2 position, Vector2 dimensions, bool bottom)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            var text = obj.AddComponent<Text>();
            text.text = content;
            text.font = font;
            text.fontSize = size;
            text.alignment = bottom ? TextAnchor.LowerLeft : TextAnchor.UpperLeft;
            text.color = Color.white;
            var outline = obj.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.75f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);
            RectTransform rect = text.rectTransform;
            rect.anchorMin = bottom ? Vector2.zero : new Vector2(0f, 1f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = rect.anchorMin;
            rect.anchoredPosition = position;
            rect.sizeDelta = dimensions;
        }
    }
}
