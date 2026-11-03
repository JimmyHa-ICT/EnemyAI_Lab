using System.IO;
using HUST.EnemyAILab.CameraSystem;
using HUST.EnemyAILab.Prototype;
using HUST.EnemyAILab.Shared.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUST.EnemyAILab.Editor
{
    public static class PrototypeSceneBuilder
    {
        private const string Root = "Assets/HUSTEnemyAILab";
        private const string FoundationScenePath = Root + "/Scenes/00_Foundation.unity";
        private const string PrototypeScenePath = Root + "/Scenes/01_Prototype.unity";
        private const string ArenaPrefabPath = Root + "/Prefabs/Environment/DemoArena.prefab";
        private const string PlayerPrefabPath = Root + "/Prefabs/Player/Player.prefab";
        private const string SlimePrefabPath = Root + "/Prefabs/Enemies/Prototype/PrototypeSlime.prefab";
        private const string SlimeSpritePath = Root + "/Art/Sprites/PrototypeSlimePlaceholder.png";
        private const string PatrolMarkerSpritePath = Root + "/Art/Sprites/PatrolMarkerPlaceholder.png";

        private const int EnemyLayer = 10;

        [MenuItem("HUST Enemy AI Lab/Build Milestone 2 Prototype")]
        public static void BuildMilestoneTwo()
        {
            EnsureFolders();
            CreatePrototypeSprites();

            Sprite slimeSprite = AssetDatabase.LoadAssetAtPath<Sprite>(SlimeSpritePath);
            Sprite markerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PatrolMarkerSpritePath);
            GameObject arenaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArenaPrefabPath);
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);

            if (slimeSprite == null || markerSprite == null || arenaPrefab == null || playerPrefab == null)
            {
                throw new InvalidDataException(
                    "Milestone 2 requires the generated prototype sprites and Milestone 1 shared prefabs.");
            }

            CreateSlimePrefab(slimeSprite);
            CreatePrototypeScene(arenaPrefab, playerPrefab, markerSprite);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Milestone 2 prototype generated at {PrototypeScenePath}");
        }

        private static void EnsureFolders()
        {
            EnsureFolder(Root + "/Prefabs", "Enemies");
            EnsureFolder(Root + "/Prefabs/Enemies", "Prototype");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static void CreatePrototypeSprites()
        {
            CreateSlimeTexture();
            CreatePatrolMarkerTexture();
            ImportSprite(SlimeSpritePath, 16f);
            ImportSprite(PatrolMarkerSpritePath, 16f);
        }

        private static void CreateSlimeTexture()
        {
            var texture = new Texture2D(20, 12, TextureFormat.RGBA32, false);
            Color clear = new Color(0f, 0f, 0f, 0f);
            Color outline = new Color32(31, 91, 58, 255);
            Color body = new Color32(89, 205, 105, 255);
            Color highlight = new Color32(145, 238, 137, 255);
            Color eye = new Color32(246, 252, 238, 255);
            Color pupil = new Color32(20, 39, 30, 255);

            Fill(texture, clear);
            for (int y = 1; y < 10; y++)
            {
                int inset = y >= 8 ? 3 : (y >= 6 ? 2 : 1);
                for (int x = inset; x < texture.width - inset; x++)
                {
                    bool border = x == inset || x == texture.width - inset - 1 || y == 1 || y == 9;
                    texture.SetPixel(x, y, border ? outline : body);
                }
            }

            for (int x = 5; x < 14; x++)
            {
                texture.SetPixel(x, 8, highlight);
            }

            texture.SetPixel(6, 6, eye);
            texture.SetPixel(7, 6, pupil);
            texture.SetPixel(13, 6, eye);
            texture.SetPixel(14, 6, pupil);
            texture.SetPixel(6, 7, eye);
            texture.SetPixel(7, 7, eye);
            texture.SetPixel(13, 7, eye);
            texture.SetPixel(14, 7, eye);
            texture.Apply();
            WriteTexture(texture, SlimeSpritePath);
        }

        private static void CreatePatrolMarkerTexture()
        {
            var texture = new Texture2D(8, 16, TextureFormat.RGBA32, false);
            Color clear = new Color(0f, 0f, 0f, 0f);
            Color pole = new Color32(92, 67, 31, 255);
            Color flag = new Color32(255, 210, 45, 255);
            Color flagLight = new Color32(255, 238, 117, 255);

            Fill(texture, clear);
            for (int y = 1; y < 15; y++)
            {
                texture.SetPixel(1, y, pole);
            }

            for (int y = 9; y < 15; y++)
            {
                int width = 7 - (14 - y) / 2;
                for (int x = 2; x < width; x++)
                {
                    texture.SetPixel(x, y, y == 14 ? flagLight : flag);
                }
            }

            texture.Apply();
            WriteTexture(texture, PatrolMarkerSpritePath);
        }

        private static void Fill(Texture2D texture, Color color)
        {
            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }

        private static void WriteTexture(Texture2D texture, string assetPath)
        {
            File.WriteAllBytes(Path.GetFullPath(assetPath), texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        private static void ImportSprite(string assetPath, float pixelsPerUnit)
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();
        }

        private static void CreateSlimePrefab(Sprite slimeSprite)
        {
            var slime = new GameObject("PrototypeSlime");
            slime.layer = EnemyLayer;

            var renderer = slime.AddComponent<SpriteRenderer>();
            renderer.sprite = slimeSprite;
            renderer.sortingOrder = 10;

            var body = slime.AddComponent<Rigidbody2D>();
            body.gravityScale = 3f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = slime.AddComponent<CapsuleCollider2D>();
            collider.size = new Vector2(1.05f, 0.65f);

            var patrol = slime.AddComponent<PrototypeSlime>();
            var patrolData = new SerializedObject(patrol);
            patrolData.FindProperty("patrolSpeed").floatValue = 2f;
            patrolData.FindProperty("arrivalDistance").floatValue = 0.08f;
            patrolData.FindProperty("visual").objectReferenceValue = renderer;
            patrolData.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(slime, SlimePrefabPath);
            Object.DestroyImmediate(slime);
        }

        private static void CreatePrototypeScene(
            GameObject arenaPrefab,
            GameObject playerPrefab,
            Sprite markerSprite)
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "01_Prototype";

            var environmentRoot = new GameObject("Environment");
            var arena = (GameObject)PrefabUtility.InstantiatePrefab(arenaPrefab, environmentRoot.transform);
            arena.name = "DemoArena";

            var playerRoot = new GameObject("Player Setup");
            var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, playerRoot.transform);
            player.transform.position = new Vector3(-10f, -1.7f, 0f);

            CreateEnemySetup(markerSprite);
            CreateCamera(player.transform);
            CreateUserInterface();
            new GameObject("Demo Controls").AddComponent<DemoSceneReset>();

            EditorSceneManager.SaveScene(scene, PrototypeScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(FoundationScenePath, true),
                new EditorBuildSettingsScene(PrototypeScenePath, true)
            };
        }

        private static void CreateEnemySetup(Sprite markerSprite)
        {
            var enemyRoot = new GameObject("Enemy Setup");
            var patrolPoints = new GameObject("Patrol Points");
            patrolPoints.transform.SetParent(enemyRoot.transform);

            Transform leftPoint = CreatePatrolPoint(
                patrolPoints.transform, "PatrolPoint_Left", markerSprite, new Vector3(-4f, -2f, 0f));
            Transform rightPoint = CreatePatrolPoint(
                patrolPoints.transform, "PatrolPoint_Right", markerSprite, new Vector3(4f, -2f, 0f));

            GameObject slimePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(SlimePrefabPath);
            var slime = (GameObject)PrefabUtility.InstantiatePrefab(slimePrefab, enemyRoot.transform);
            slime.transform.position = new Vector3(-1f, -2.175f, 0f);

            var patrol = slime.GetComponent<PrototypeSlime>();
            var patrolData = new SerializedObject(patrol);
            patrolData.FindProperty("leftPatrolPoint").objectReferenceValue = leftPoint;
            patrolData.FindProperty("rightPatrolPoint").objectReferenceValue = rightPoint;
            patrolData.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Transform CreatePatrolPoint(
            Transform parent,
            string name,
            Sprite markerSprite,
            Vector3 position)
        {
            var marker = new GameObject(name);
            marker.transform.SetParent(parent);
            marker.transform.position = position;
            var renderer = marker.AddComponent<SpriteRenderer>();
            renderer.sprite = markerSprite;
            renderer.sortingOrder = 5;
            return marker.transform;
        }

        private static void CreateCamera(Transform player)
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(-7f, 0f, -10f);

            var sceneCamera = cameraObject.AddComponent<UnityEngine.Camera>();
            sceneCamera.orthographic = true;
            sceneCamera.orthographicSize = 5.625f;
            sceneCamera.backgroundColor = new Color32(120, 195, 230, 255);
            sceneCamera.clearFlags = CameraClearFlags.SolidColor;

            var cameraFollow = cameraObject.AddComponent<SideScrollingCamera>();
            var cameraData = new SerializedObject(cameraFollow);
            cameraData.FindProperty("target").objectReferenceValue = player;
            cameraData.FindProperty("fixedY").floatValue = 0f;
            cameraData.FindProperty("minimumX").floatValue = -7f;
            cameraData.FindProperty("maximumX").floatValue = 7f;
            cameraData.FindProperty("smoothTime").floatValue = 0.12f;
            cameraData.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateUserInterface()
        {
            var uiRoot = new GameObject("UI");
            var canvas = uiRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = uiRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            uiRoot.AddComponent<GraphicRaycaster>();

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            CreateText(
                uiRoot.transform,
                "Title",
                "SCENE 01  |  PROTOTYPE: SIMPLE SLIME PATROL",
                font,
                24,
                TextAnchor.UpperLeft,
                new Vector2(18f, -16f),
                new Vector2(760f, 44f));

            CreateText(
                uiRoot.transform,
                "Controls",
                "Move: A / D or Arrow Keys     Jump: Space     Reset: R\nOne MonoBehaviour. Two patrol points. No State Machine.",
                font,
                19,
                TextAnchor.LowerLeft,
                new Vector2(18f, 16f),
                new Vector2(760f, 70f),
                anchorAtBottom: true);
        }

        private static void CreateText(
            Transform parent,
            string name,
            string content,
            Font font,
            int size,
            TextAnchor alignment,
            Vector2 position,
            Vector2 dimensions,
            bool anchorAtBottom = false)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            var text = textObject.AddComponent<Text>();
            text.text = content;
            text.font = font;
            text.fontSize = size;
            text.alignment = alignment;
            text.color = Color.white;

            var outline = textObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.75f);
            outline.effectDistance = new Vector2(1.5f, -1.5f);

            RectTransform rect = text.rectTransform;
            if (anchorAtBottom)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.zero;
                rect.pivot = Vector2.zero;
            }
            else
            {
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
            }

            rect.anchoredPosition = position;
            rect.sizeDelta = dimensions;
        }
    }
}
