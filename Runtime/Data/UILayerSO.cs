using UnityEngine;
using System.Collections.Generic;

namespace BattleTurn.UIManagements.Runtime
{
    [CreateAssetMenu(fileName = "UILayerSO", menuName = "UI/UILayerSO")]
    public sealed class UILayerSO : ScriptableObject
    {
        [SerializeField] private string[] _layerNames = { "Default", "Overlay"};

        public IReadOnlyList<string> LayerNames => _layerNames;
    }
}