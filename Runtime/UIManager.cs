using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;

namespace BattleTurn.UIManagements.Runtime
{
    public sealed class UIManager : MonoBehaviour
    {
        private const string POPUP = "POPUPS";
        private const string SCREEN = "SCREENS";

        [Required("LayerLayoutSO is required to initialize UI layers.")]
        [SerializeField] private UILayerSO _layerLayoutSO = null;

        [BoxGroup(POPUP)]
        [SerializeField] private List<PopupBase> _popupPrefabs = new();

        [BoxGroup(POPUP)]
        [SerializeField, ReadOnly] private SerializedDictionary<string, ViewPool<PopupBase>> _showingGroup = new();

        [BoxGroup(POPUP)]
        [SerializeField, ReadOnly] private SerializedDictionary<string, ViewPool<PopupBase>> _hidingGroup = new();

        [BoxGroup(SCREEN)]
        [SerializeField] private List<ScreenBase> _screenPrefabs = new();

        [BoxGroup(SCREEN)]
        [SerializeField, ReadOnly] private SerializedDictionary<string, ScreenBase> _showingScreens = new();

        [BoxGroup(SCREEN)]
        [SerializeField, ReadOnly] private SerializedDictionary<string, ScreenBase> _hidingScreens = new();

        private Dictionary<Type, PopupBase> _popupPrefabMap = new();
        private Dictionary<Type, ScreenBase> _screenPrefabMap = new();
        private Dictionary<string, Transform> _layerMap = new();

        public T GetPopup<T>() where T : PopupBase
        {
            Type type = typeof(T);
            if (_showingGroup.TryGetValue(type.Name, out ViewPool<PopupBase> showingPool) && showingPool.Count > 0)
            {
                return (T)showingPool.GetFirst();
            }
            if (_hidingGroup.TryGetValue(type.Name, out ViewPool<PopupBase> hidingPool) && hidingPool.Count > 0)
            {
                return (T)hidingPool.GetFirst();
            }
            return null;
        }

        public void SetLayer(UIViewer instance, string layerName)
        {
            if (!_layerMap.TryGetValue(layerName, out Transform layerTransform))
            {
                throw new UILayerNotExistException(layerName, nameof(_layerMap));
            }

            instance.transform.SetParent(layerTransform, false);
        }

        public PopupBase GetPopup(Type popupType)
        {
            if (_showingGroup.TryGetValue(popupType.Name, out ViewPool<PopupBase> showingPool) && showingPool.Count > 0)
            {
                return showingPool.GetFirst();
            }
            if (_hidingGroup.TryGetValue(popupType.Name, out ViewPool<PopupBase> hidingPool) && hidingPool.Count > 0)
            {
                return hidingPool.GetFirst();
            }
            return null;
        }

        public T ShowPopup<T>(string layer = null) where T : PopupBase
        {
            Type type = typeof(T);
            return (T)ShowPopup(type, layer);
        }

        public PopupBase ShowPopup(Type popupType, string layer = null)
        {
            layer = GetLayer(layer);

            if (!_popupPrefabMap.TryGetValue(popupType, out PopupBase popupPrefab))
            {
                throw new UIViewNotExistException(popupType.Name, nameof(_popupPrefabMap));
            }

            PopupBase popupInstance;
            if (_hidingGroup.TryGetValue(popupType.Name, out ViewPool<PopupBase> hidingPool) && hidingPool.Count > 0)
            {
                popupInstance = hidingPool.GetFirst();
                popupInstance.transform.SetParent(_layerMap[layer], false);
            }
            else
            {
                popupInstance = Instantiate(popupPrefab, _layerMap[layer]);
            }

            if (!_showingGroup.TryGetValue(popupType.Name, out ViewPool<PopupBase> showingPool))
            {
                showingPool = new ViewPool<PopupBase>();
                _showingGroup.Add(popupType.Name, showingPool);
            }

            showingPool.Push(popupInstance.GetInstanceID(), popupInstance);
            popupInstance.OnInitialize(this);
            popupInstance.ShowInternal();
            return popupInstance;
        }

        internal PopupBase ShowPopup(PopupBase popupBase, string layer = null)
        {
            layer = GetLayer(layer);

            if (!_popupPrefabMap.TryGetValue(popupBase.GetType(), out PopupBase popupPrefab))
            {
                throw new UIViewNotExistException(popupBase.GetType().Name, nameof(_popupPrefabMap));
            }

            if (!_showingGroup.TryGetValue(popupBase.GetType().Name, out ViewPool<PopupBase> showingPool))
            {
                showingPool = new ViewPool<PopupBase>();
                _showingGroup.Add(popupBase.GetType().Name, showingPool);
            }

            showingPool.Push(popupBase.GetInstanceID(), popupBase);
            popupBase.transform.SetParent(_layerMap[layer], false);
            popupBase.OnInitialize(this);
            popupBase.ShowInternal();
            return popupBase;
        }

        public T HidePopup<T>(PopupBase popupInstance) where T : PopupBase
        {
            Type type = typeof(T);
            if (!_showingGroup.TryGetValue(type.Name, out ViewPool<PopupBase> showingPool))
            {
                throw new UIViewNotShowingException(type.Name, nameof(_showingGroup));
            }

            if (showingPool.Has(popupInstance.GetInstanceID()))
            {
                showingPool.views.Remove(popupInstance.GetInstanceID());
            }
            else
            {
                throw new UIViewNotExistException(type.Name, nameof(_showingGroup));
            }

            ViewPool<PopupBase> hidingPool;
            if (!_hidingGroup.TryGetValue(type.Name, out hidingPool))
            {
                hidingPool = new ViewPool<PopupBase>();
                _hidingGroup.Add(type.Name, hidingPool);
            }

            hidingPool.Push(popupInstance.GetInstanceID(), popupInstance);

            popupInstance.HideInternal();
            return (T)popupInstance;
        }

        public void HidePopup(PopupBase popupInstance)
        {
            Type type = popupInstance.GetType();
            if (!_showingGroup.TryGetValue(type.Name, out ViewPool<PopupBase> showingPool))
            {
                throw new UIViewNotShowingException(type.Name, nameof(_showingGroup));
            }

            if (showingPool.Has(popupInstance.GetInstanceID()))
            {
                showingPool.views.Remove(popupInstance.GetInstanceID());
            }
            else
            {
                throw new UIViewNotExistException(type.Name, nameof(_showingGroup));
            }

            ViewPool<PopupBase> hidingPool;
            if (!_hidingGroup.TryGetValue(type.Name, out hidingPool))
            {
                hidingPool = new ViewPool<PopupBase>();
                _hidingGroup.Add(type.Name, hidingPool);
            }

            hidingPool.Push(popupInstance.GetInstanceID(), popupInstance);

            popupInstance.HideInternal();
        }

        public T GetScreen<T>() where T : ScreenBase
        {
            Type type = typeof(T);
            if (_showingScreens.TryGetValue(type.Name, out ScreenBase screenInstance))
            {
                return (T)screenInstance;
            }
            if (_hidingScreens.TryGetValue(type.Name, out screenInstance))
            {
                return (T)screenInstance;
            }
            return null;
        }

        public ScreenBase GetScreen(Type screenType)
        {
            if (_showingScreens.TryGetValue(screenType.Name, out ScreenBase screenInstance))
            {
                return screenInstance;
            }
            if (_hidingScreens.TryGetValue(screenType.Name, out screenInstance))
            {
                return screenInstance;
            }
            return null;
        }

        public T ShowScreen<T>(string layer = null) where T : ScreenBase
        {
            Type type = typeof(T);
            return (T)ShowScreen(type, layer);
        }

        public UIViewer ShowScreen(Type screenType, string layer = null)
        {
            layer = GetLayer(layer);
            if (_showingScreens.ContainsKey(screenType.Name))
            {
                throw new UIViewAlreadyExistException(screenType.Name, nameof(_showingScreens));
            }

            ScreenBase screenInstance;
            if (_hidingScreens.TryGetValue(screenType.Name, out screenInstance))
            {
                _hidingScreens.Remove(screenType.Name);
                screenInstance.transform.SetParent(_layerMap[layer], false);
            }
            else if (_screenPrefabMap.TryGetValue(screenType, out ScreenBase screenPrefab))
            {
                screenInstance = Instantiate(screenPrefab, _layerMap[layer]);
            }
            else
            {
                throw new UIViewNotExistException(screenType.Name, nameof(_screenPrefabMap));
            }

            _showingScreens.Add(screenType.Name, screenInstance);
            screenInstance.OnInitialize(this);
            screenInstance.ShowInternal();
            return screenInstance;
        }

        public T HideScreen<T>() where T : ScreenBase
        {
            Type type = typeof(T);
            if (!_showingScreens.TryGetValue(type.Name, out ScreenBase screenInstance))
            {
                throw new UIViewNotShowingException(type.Name, nameof(_showingScreens));
            }
            _showingScreens.Remove(type.Name);
            _hidingScreens.Add(type.Name, screenInstance);

            screenInstance.HideInternal();
            return (T)screenInstance;
        }

        public void HideScreen(ScreenBase screenInstance)
        {
            Type type = screenInstance.GetType();
            if (!_showingScreens.TryGetValue(type.Name, out ScreenBase showingScreen))
            {
                throw new UIViewNotShowingException(type.Name, nameof(_showingScreens));
            }

            if (showingScreen.GetType() != type)
            {
                throw new UIViewNotExistException(type.Name, nameof(_showingScreens));
            }

            _showingScreens.Remove(type.Name);
            _hidingScreens.Add(type.Name, showingScreen);

            showingScreen.HideInternal();
        }

        internal void HideScreen(Type screenType)
        {
            if (!_showingScreens.TryGetValue(screenType.Name, out ScreenBase showingScreen))
            {
                throw new UIViewNotShowingException(screenType.Name, nameof(_showingScreens));
            }

            if (showingScreen.GetType() != screenType)
            {
                throw new UIViewNotExistException(screenType.Name, nameof(_showingScreens));
            }

            _showingScreens.Remove(screenType.Name);
            _hidingScreens.Add(screenType.Name, showingScreen);

            showingScreen.HideInternal();
        }

        private void Awake()
        {
            InitLayer();
            GenerateScreen();
            GeneratePopup();
        }

        private string GetLayer(string layer)
        {
            if (string.IsNullOrEmpty(layer))
            {
                layer = "Default";
            }

            if (!_layerMap.ContainsKey(layer))
            {
                throw new UILayerNotExistException(layer, nameof(_layerMap));
            }

            return layer;
        }

        private void GenerateScreen()
        {
            foreach (ScreenBase screenPrefab in _screenPrefabs)
            {
                Type type = screenPrefab.GetType();
                if (_screenPrefabMap.ContainsKey(type))
                {
                    throw new UIViewAlreadyExistException(type.Name, nameof(_screenPrefabMap));
                }
                _screenPrefabMap.Add(type, screenPrefab);
            }
        }

        private void GeneratePopup()
        {
            foreach (PopupBase popupPrefab in _popupPrefabs)
            {
                Type type = popupPrefab.GetType();
                if (_popupPrefabMap.ContainsKey(type))
                {
                    throw new UIViewAlreadyExistException(type.Name, nameof(_popupPrefabMap));
                }
                _popupPrefabMap.Add(type, popupPrefab);
            }
        }

        private void InitLayer()
        {
            _layerMap.Clear();
            foreach (string layerName in _layerLayoutSO.LayerNames)
            {
                var layerObject = new GameObject(layerName);
                // parent under the UIManager transform without altering local transform
                layerObject.transform.SetParent(transform, false);
                var rect = layerObject.AddComponent<RectTransform>();

                // stretch to full size of parent
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                rect.anchoredPosition = Vector2.zero;
                rect.localScale = Vector3.one;
                _layerMap.Add(layerName, layerObject.transform);
            }
        }
    }
}