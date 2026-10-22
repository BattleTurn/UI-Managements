using System.IO;
using System.Text;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using BattleTurn.UIManagements.Runtime;

namespace BattleTurn.UIManagements.Editor
{
    [CustomEditor(typeof(UILayerSO))]
    internal sealed class UILayerSOEditor : UnityEditor.Editor
    {
        private const string DEFAULT_LAYER = "Default";
        private const string OVERLAY_LAYER = "Overlay";
        private const int DEFAULT_LAYER_INDEX = 0;

        private ReorderableList _reorderableList;
        private SerializedProperty _layerNamesProperty;

        private bool _hasChanges;

        private void OnEnable()
        {
            _layerNamesProperty =
                serializedObject.FindProperty("_layerNames");

            SetupReorderableList();
        }

        private void SetupReorderableList()
        {
            _reorderableList = new ReorderableList(
                serializedObject,
                _layerNamesProperty,
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true
            );

            _reorderableList.drawHeaderCallback = DrawHeader;
            _reorderableList.drawElementCallback = DrawElement;

            _reorderableList.onCanRemoveCallback = CanRemove;
            _reorderableList.onRemoveCallback = RemoveElement;
            _reorderableList.onAddCallback = AddElement;

            _reorderableList.onReorderCallbackWithDetails =
                OnReorder;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space(10);

            EditorGUI.BeginChangeCheck();

            _reorderableList.DoLayoutList();

            bool changed = EditorGUI.EndChangeCheck();

            // Always guarantee the reserved layer invariant.
            EnsureDefaultLayer();

            if (changed || serializedObject.hasModifiedProperties)
            {
                _hasChanges = true;

                // Inspector changes must always be committed.
                serializedObject.ApplyModifiedProperties();

                EditorUtility.SetDirty(target);
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "Default and Overlay are reserved layers. " +
                "They cannot be renamed, removed, or reordered.",
                MessageType.Info
            );

            EditorGUILayout.Space(10);

            using (new EditorGUI.DisabledScope(!_hasChanges))
            {
                if (GUILayout.Button(
                    "Apply Changes",
                    GUILayout.Height(30)))
                {
                    // Make absolutely sure the latest text field
                    // value is committed before generating.
                    serializedObject.ApplyModifiedProperties();

                    GenerateLayerNameFile();

                    _hasChanges = false;

                    EditorUtility.SetDirty(target);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Layer Names");
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _layerNamesProperty.GetArrayElementAtIndex(index);

            rect.y += 2f;

            bool isDefault = element.stringValue == DEFAULT_LAYER;
            bool isOverlay = element.stringValue == OVERLAY_LAYER;

            // Default and Overlay cannot be renamed.
            bool isReadOnly = isDefault || isOverlay;

            Rect fieldRect = rect;

            if (isReadOnly)
                fieldRect.width -= 20f;

            using (new EditorGUI.DisabledScope(isReadOnly))
            {
                EditorGUI.PropertyField(new Rect(fieldRect.x, fieldRect.y, fieldRect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            }

            if (!isReadOnly)
                return;

            Rect lockRect = new Rect(rect.xMax - 16f, rect.y, 16f, EditorGUIUtility.singleLineHeight);

            GUI.Label(lockRect, EditorGUIUtility.IconContent("AssemblyLock"));
        }

        private bool CanRemove(ReorderableList list)
        {
            if (list.index < 0 || list.index >= _layerNamesProperty.arraySize)
                return false;

            string layerName = _layerNamesProperty.GetArrayElementAtIndex(list.index).stringValue;

            return layerName != DEFAULT_LAYER &&
                   layerName != OVERLAY_LAYER;
        }

        private void RemoveElement(ReorderableList list)
        {
            if (!CanRemove(list))
                return;

            ReorderableList.defaultBehaviours.DoRemoveButton(list);

            _hasChanges = true;
        }

        private void AddElement(ReorderableList list)
        {
            SerializedProperty property = list.serializedProperty;

            int newIndex = property.arraySize;
            property.InsertArrayElementAtIndex(newIndex);

            SerializedProperty element = property.GetArrayElementAtIndex(newIndex);
            element.stringValue = GetUniqueLayerName();

            list.index = newIndex;

            _hasChanges = true;
        }

        private void OnReorder(ReorderableList list, int oldIndex, int newIndex)
        {
            // Default itself cannot be moved.
            if (oldIndex == DEFAULT_LAYER_INDEX)
            {
                list.serializedProperty.MoveArrayElement(
                    newIndex,
                    DEFAULT_LAYER_INDEX
                );

                list.index = DEFAULT_LAYER_INDEX;
                return;
            }

            // No other layer can be moved before Default.
            if (newIndex == DEFAULT_LAYER_INDEX)
            {
                list.serializedProperty.MoveArrayElement(
                    newIndex,
                    oldIndex
                );

                list.index = oldIndex;
                return;
            }

            _hasChanges = true;
        }

        private void EnsureDefaultLayer()
        {
            int defaultIndex = FindLayerIndex(DEFAULT_LAYER);

            // Default was somehow removed.
            if (defaultIndex < 0)
            {
                _layerNamesProperty.InsertArrayElementAtIndex(0);

                _layerNamesProperty
                    .GetArrayElementAtIndex(0)
                    .stringValue = DEFAULT_LAYER;

                return;
            }

            // Default must always stay at index 0.
            if (defaultIndex != DEFAULT_LAYER_INDEX)
            {
                _layerNamesProperty.MoveArrayElement(
                    defaultIndex,
                    DEFAULT_LAYER_INDEX
                );
            }
        }

        private int FindLayerIndex(string layerName)
        {
            for (int i = 0; i < _layerNamesProperty.arraySize; i++)
            {
                SerializedProperty element =
                    _layerNamesProperty.GetArrayElementAtIndex(i);

                if (element.stringValue == layerName)
                    return i;
            }

            return -1;
        }

        private string GetUniqueLayerName()
        {
            const string baseName = "New Layer";

            if (!ContainsLayer(baseName))
                return baseName;

            int index = 1;

            while (ContainsLayer($"{baseName} {index}"))
                index++;

            return $"{baseName} {index}";
        }

        private bool ContainsLayer(string layerName)
        {
            for (int i = 0; i < _layerNamesProperty.arraySize; i++)
            {
                if (_layerNamesProperty
                    .GetArrayElementAtIndex(i)
                    .stringValue == layerName)
                {
                    return true;
                }
            }

            return false;
        }

        private void GenerateLayerNameFile()
        {
            UILayerSO layerSO = (UILayerSO)target;

            string directoryPath =
                System.IO.Path.GetDirectoryName(
                    Runtime.Path.LAYER_NAME_FILE_PATH
                );

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            StringBuilder sb = new StringBuilder();

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

                string constantName = layerName
                    .ToUpperInvariant()
                    .Replace(' ', '_');

                sb.AppendLine(
                    $"        public const string {constantName} = \"{layerName}\";"
                );
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            File.WriteAllText(
                Runtime.Path.LAYER_NAME_FILE_PATH,
                sb.ToString()
            );

            AssetDatabase.Refresh();

            Debug.Log(
                $"[{nameof(UILayerSO)}] Generated UILayer class at: " +
                Runtime.Path.LAYER_NAME_FILE_PATH
            );
        }
    }
}