using System.IO;
using HUST.EnemyAILab.Production;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUST.EnemyAILab.Editor
{
    public static class BugDemoSceneBuilder
    {
        private const string Root = "Assets/HUSTEnemyAILab";
        private const string SourceScene = Root + "/Scenes/03_ModularAI.unity";
        private const string TargetScene = Root + "/Scenes/04_BugDemo.unity";

        [MenuItem("HUST Enemy AI Lab/Build Milestone 5 Bug Demo")]
        public static void Build()
        {
            if (!File.Exists(Path.GetFullPath(SourceScene)))
                throw new InvalidDataException("Milestone 5 requires the accepted Scene 03 modular AI demonstration.");

            Scene scene = EditorSceneManager.OpenScene(SourceScene, OpenSceneMode.Single);
            EnemyCombat combat = Object.FindFirstObjectByType<EnemyCombat>();
            if (combat == null) throw new InvalidDataException("Scene 03 has no production EnemyCombat component.");

            combat.SetValidationMode(AttackValidationMode.UnsafeTeachingDemo);
            EditorUtility.SetDirty(combat);

            Text title = GameObject.Find("UI/Title")?.GetComponent<Text>();
            if (title != null) title.text = "SCENE 04  |  PRODUCTION BUG DEMO: STALE ATTACK RANGE";
            Text controls = GameObject.Find("UI/Controls")?.GetComponent<Text>();
            if (controls != null)
                controls.text = "1) Enter red Attack range  2) Move away during wind-up  3) Observe damage  4) R resets\nEnemyCombat > Validation Mode: Safe or UnsafeTeachingDemo (intentionally incorrect)";

            EditorSceneManager.SaveScene(scene, TargetScene);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(Root + "/Scenes/00_Foundation.unity", true),
                new EditorBuildSettingsScene(Root + "/Scenes/01_Prototype.unity", true),
                new EditorBuildSettingsScene(Root + "/Scenes/02_EnumFSM.unity", true),
                new EditorBuildSettingsScene(SourceScene, true),
                new EditorBuildSettingsScene(TargetScene, true)
            };
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Milestone 5 bug demonstration generated at {TargetScene}");
        }
    }
}
