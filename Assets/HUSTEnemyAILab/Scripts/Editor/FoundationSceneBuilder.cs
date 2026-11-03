using System.IO;
using HUST.EnemyAILab.CameraSystem;
using HUST.EnemyAILab.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUST.EnemyAILab.Editor
{
    public static class FoundationSceneBuilder
    {
        private const string Root = "Assets/HUSTEnemyAILab";
        private const string ScenePath = Root + "/Scenes/00_Foundation.unity";
        private const string PlayerPrefabPath = Root + "/Prefabs/Player/Player.prefab";
        private const string ArenaPrefabPath = Root + "/Prefabs/Environment/DemoArena.prefab";
        private const string PlayerSpritePath = Root + "/Art/Sprites/PlayerPlaceholder.png";
        private const string GroundSpritePath = Root + "/Art/Sprites/GroundPlaceholder.png";
        private const string InputActionsPath = "Assets/InputSystem_Actions.inputactions";

        private const int GroundLayer = 8;
        private const int PlayerLayer = 9;
        private const int EnemyLayer = 10;

        [MenuItem("HUST Enemy AI Lab/Build Milestone 1 Foundation")]
        public static void BuildMilestoneOne()
        {
            EnsureFolders();
            ConfigureLayers();
            CreatePlaceholderSprites();

            Sprite playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>(PlayerSpritePath);
            Sprite groundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(GroundSpritePath);
            InputActionAsset inputActions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(InputActionsPath);

            if (playerSprite == null || groundSprite == null || inputActions == null)
            {
                throw new InvalidDataException(
                    $"Milestone 1 assets could not be loaded after import. " +
                    $"Player sprite: {playerSprite != null}, Ground sprite: {groundSprite != null}, " +
                    $"Input actions: {inputActions != null}.");
            }

            CreateArenaPrefab(groundSprite);
            CreatePlayerPrefab(playerSprite, inputActions);
            CreateFoundationScene();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Milestone 1 foundation generated at {ScenePath}");
        }

        private static void EnsureFolders()
        {
            EnsureFolder("Assets", "HUSTEnemyAILab");
            EnsureFolder(Root, "Art");
            EnsureFolder(Root + "/Art", "Sprites");
            EnsureFolder(Root, "Prefabs");
            EnsureFolder(Root + "/Prefabs", "Environment");
            EnsureFolder(Root + "/Prefabs", "Player");
            EnsureFolder(Root, "Scenes");
        }

        private static void EnsureFolder(string parent, string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }

        private static void ConfigureLayers()
        {
            Object tagManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0];
            var serializedTagManager = new SerializedObject(tagManager);
            SerializedProperty layers = serializedTagManager.FindProperty("layers");

            layers.GetArrayElementAtIndex(GroundLayer).stringValue = "Ground";
            layers.GetArrayElementAtIndex(PlayerLayer).stringValue = "Player";
            layers.GetArrayElementAtIndex(EnemyLayer).stringValue = "Enemy";
            serializedTagManager.ApplyModifiedProperties();
        }

        private static void CreatePlaceholderSprites()
        {
            CreatePlayerTexture();
            CreateGroundTexture();
            ImportSprite(PlayerSpritePath, 16f);
            ImportSprite(GroundSpritePath, 16f);
        }

        private static void CreatePlayerTexture()
        {
            var texture = new Texture2D(16, 24, TextureFormat.RGBA32, false);
            Color clear = new Color(0f, 0f, 0f, 0f);
            Color outline = new Color32(25, 55, 105, 255);
            Color body = new Color32(70, 155, 235, 255);
            Color eye = new Color32(245, 250, 255, 255);
            Color pupil = new Color32(20, 30, 45, 255);

            Fill(texture, clear);
            for (int y = 1; y < 22; y++)
            {
                for (int x = 2; x < 14; x++)
                {
                    bool border = x == 2 || x == 13 || y == 1 || y == 21;
                    texture.SetPixel(x, y, border ? outline : body);
                }
            }

            texture.SetPixel(5, 16, eye);
            texture.SetPixel(6, 16, pupil);
            texture.SetPixel(10, 16, eye);
            texture.SetPixel(11, 16, pupil);
            texture.SetPixel(5, 17, eye);
            texture.SetPixel(6, 17, eye);
            texture.SetPixel(10, 17, eye);
            texture.SetPixel(11, 17, eye);
            texture.Apply();
            WriteTexture(texture, PlayerSpritePath);
        }

        private static void CreateGroundTexture()
        {
            var texture = new Texture2D(16, 16, TextureFormat.RGBA32, false);
            Color soil = new Color32(126, 79, 48, 255);
            Color darkSoil = new Color32(91, 57, 42, 255);
            Color grass = new Color32(85, 176, 87, 255);
            Color grassLight = new Color32(128, 208, 104, 255);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    Color color = y >= 12 ? grass : ((x + y) % 5 == 0 ? darkSoil : soil);
                    if (y == 15 && x % 3 != 0)
                    {
                        color = grassLight;
                    }

                    texture.SetPixel(x, y, color);
                }
            }

            texture.Apply();
            WriteTexture(texture, GroundSpritePath);
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

        private static void CreateArenaPrefab(Sprite groundSprite)
        {
            var arena = new GameObject("DemoArena");

            CreateBlock(arena.transform, "Ground", groundSprite, new Vector2(0f, -3f), new Vector2(30f, 1f));
            CreateBlock(arena.transform, "Platform_Left", groundSprite, new Vector2(-6f, -1f), new Vector2(4f, 0.5f));
            CreateBlock(arena.transform, "Platform_Centre", groundSprite, new Vector2(1f, 0f), new Vector2(4.5f, 0.5f));
            CreateBlock(arena.transform, "Platform_Right", groundSprite, new Vector2(9f, -1.15f), new Vector2(4f, 0.5f));
            CreateBoundary(arena.transform, "LeftBoundary", new Vector2(-15.25f, 0f));
            CreateBoundary(arena.transform, "RightBoundary", new Vector2(15.25f, 0f));

            PrefabUtility.SaveAsPrefabAsset(arena, ArenaPrefabPath);
            Object.DestroyImmediate(arena);
        }

        private static void CreateBlock(
            Transform parent,
            string name,
            Sprite sprite,
            Vector2 position,
            Vector2 scale)
        {
            var block = new GameObject(name);
            block.layer = GroundLayer;
            block.transform.SetParent(parent);
            block.transform.localPosition = position;
            block.transform.localScale = scale;

            var renderer = block.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = 0;
            block.AddComponent<BoxCollider2D>();
        }

        private static void CreateBoundary(Transform parent, string name, Vector2 position)
        {
            var boundary = new GameObject(name);
            boundary.layer = GroundLayer;
            boundary.transform.SetParent(parent);
            boundary.transform.localPosition = position;
            var collider = boundary.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.5f, 10f);
        }

        private static void CreatePlayerPrefab(Sprite playerSprite, InputActionAsset inputActions)
        {
            var player = new GameObject("Player");
            player.layer = PlayerLayer;
            player.tag = "Player";

            var renderer = player.AddComponent<SpriteRenderer>();
            renderer.sprite = playerSprite;
            renderer.sortingOrder = 10;

            var body = player.AddComponent<Rigidbody2D>();
            body.gravityScale = 3f;
            body.freezeRotation = true;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var collider = player.AddComponent<CapsuleCollider2D>();
            collider.size = new Vector2(0.72f, 1.35f);

            var groundCheckObject = new GameObject("GroundCheck");
            groundCheckObject.transform.SetParent(player.transform);
            groundCheckObject.transform.localPosition = new Vector3(0f, -0.72f, 0f);

            var controller = player.AddComponent<PlayerController>();
            var controllerData = new SerializedObject(controller);
            controllerData.FindProperty("inputActions").objectReferenceValue = inputActions;
            controllerData.FindProperty("groundCheck").objectReferenceValue = groundCheckObject.transform;
            controllerData.FindProperty("groundMask").intValue = 1 << GroundLayer;
            controllerData.ApplyModifiedPropertiesWithoutUndo();

            player.AddComponent<PlayerHealth>();
            PrefabUtility.SaveAsPrefabAsset(player, PlayerPrefabPath);
            Object.DestroyImmediate(player);
        }

        private static void CreateFoundationScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "00_Foundation";

            var environmentRoot = new GameObject("Environment");
            GameObject arenaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ArenaPrefabPath);
            var arena = (GameObject)PrefabUtility.InstantiatePrefab(arenaPrefab, environmentRoot.transform);
            arena.name = "DemoArena";

            var playerRoot = new GameObject("Player Setup");
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, playerRoot.transform);
            player.transform.position = new Vector3(-10f, -1.7f, 0f);

            CreateCamera(player.transform);
            CreateUserInterface();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
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
                "HUST ENEMY AI LAB  |  MILESTONE 1: SHARED FOUNDATION",
                font,
                24,
                TextAnchor.UpperLeft,
                new Vector2(18f, -16f),
                new Vector2(820f, 44f));

            CreateText(
                uiRoot.transform,
                "Controls",
                "Move: A / D or Arrow Keys     Jump: Space\nEnemy AI begins in Milestone 2",
                font,
                19,
                TextAnchor.LowerLeft,
                new Vector2(18f, 16f),
                new Vector2(650f, 70f),
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
