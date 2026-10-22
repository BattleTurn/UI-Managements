using System.IO;
using UnityEditor;
using UnityEngine;
using BattleTurn.UIManagements.Runtime;
using Path = BattleTurn.UIManagements.Runtime.Path;

namespace BattleTurn.UIManagements.Editor.Auto
{
    [InitializeOnLoad]
    internal sealed class AutoGenerateUILayerFile
    {
        static AutoGenerateUILayerFile()
        {
            EditorApplication.delayCall += Initialize;
        }

        private static void Initialize()
        {
            // Wait for UILayerSO to be created first
            EditorApplication.delayCall += GenerateUILayerFileIfNeeded;
        }

        private static void GenerateUILayerFileIfNeeded()
        {
            // Check if UILayer.cs file already exists
            if (File.Exists(Path.LAYER_NAME_FILE_PATH))
            {
                return;
            }

            // Find UILayerSO in the project
            string[] guids = AssetDatabase.FindAssets("t:UILayerSO");

            if (guids.Length > 0)
            {
                // Get the first UILayerSO found
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                UILayerSO layerSO = AssetDatabase.LoadAssetAtPath<UILayerSO>(assetPath);

                if (layerSO != null)
                {
                    GenerateUILayerFile(layerSO);
                }
            }
        }

        private static void GenerateUILayerFile(UILayerSO layerSO)
        {
            // Ensure directory exists
            string directoryPath = System.IO.Path.GetDirectoryName(Path.LAYER_NAME_FILE_PATH);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Generate the static class content
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("// Auto-generated file. Do not modify manually.");
            sb.AppendLine();
            sb.AppendLine($"namespace {nameof(BattleTurn.UIManagements.Runtime)}");
            sb.AppendLine("{");
            sb.AppendLine("    public static class UILayer");
            sb.AppendLine("    {");

            foreach (string layerName in layerSO.LayerNames)
            {
                if (string.IsNullOrWhiteSpace(layerName))
                    continue;

                // Convert to constant name: uppercase and replace spaces with underscores
                string constantName = layerName.ToUpper().Replace(' ', '_');

                sb.AppendLine($"        public const string {constantName} = \"{layerName}\";");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            // Write to file
            File.WriteAllText(Path.LAYER_NAME_FILE_PATH, sb.ToString());

            // Refresh AssetDatabase
            AssetDatabase.Refresh();

            Debug.Log($"[AutoGenerateUILayerFile] Generated UILayer class at: {Path.LAYER_NAME_FILE_PATH}");
        }
    }
}
