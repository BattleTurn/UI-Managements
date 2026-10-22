using UnityEditor;
using UnityEngine;
using BattleTurn.UIManagements.Runtime;

namespace BattleTurn.UIManagements.Editor.Auto
{
    [InitializeOnLoad]
    internal sealed class CreateLayerLayoutSOAutomator
    {
        static CreateLayerLayoutSOAutomator()
        {
            EditorApplication.delayCall += Initialize;
        }

        private static void Initialize()
        {
            // Search for existing LayerLayoutSO assets in the project
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(UILayerSO)}");

            // If no LayerLayoutSO exists, create one
            if (guids.Length == 0)
            {
                CreateLayerLayoutSO();
            }
        }

        private static void CreateLayerLayoutSO()
        {
            // Ensure the directory exists
            string folderPath = Path.GENERATED_FOLDER_PATH;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                CreateFolder(folderPath);
            }

            // Create the LayerLayoutSO asset
            UILayerSO layerLayoutSO = ScriptableObject.CreateInstance<UILayerSO>();
            AssetDatabase.CreateAsset(layerLayoutSO, Path.UI_LAYER_SO_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[{nameof(UILayerSO)}] Created new {nameof(UILayerSO)} at: {Path.UI_LAYER_SO_PATH}");
        }

        private static void CreateFolder(string path)
        {
            string[] folders = path.Split('/');
            string currentPath = folders[0];

            for (int i = 1; i < folders.Length; i++)
            {
                string nextPath = currentPath + "/" + folders[i];
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = nextPath;
            }
        }
    }
}