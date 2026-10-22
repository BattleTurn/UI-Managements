using System;
using UnityEngine;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class TabController : MonoBehaviour
    {
        public event Action<byte> OnTabChanged;

        [SerializeField] private Tab[] _tabs;

        private void Awake()
        {
            RegisterTabs();
            EnsureSelection();
        }

        private void OnDestroy()
        {
            foreach (Tab tab in _tabs)
                tab.TabController = null;
        }

        public void SwitchTab(byte index)
        {
            if (index >= _tabs.Length)
            {
                Debug.LogWarning($"Invalid tab index: {index}. It should be between 0 and {_tabs.Length - 1}.");
                return;
            }

            _tabs[index].isOn = true;
        }

        private void RegisterTabs()
        {
            for (byte i = 0; i < _tabs.Length; i++)
            {
                byte index = i;
                Tab tab = _tabs[i];

                tab.TabController = this;
                tab.onValueChanged.AddListener(isOn => OnValueChanged(tab, index, isOn));
            }
        }

        private void OnValueChanged(Tab selected, byte index, bool isOn)
        {
            if (!isOn)
                return;

            DeactivateOthers(selected);
            OnTabChanged?.Invoke(index);
        }

        private void DeactivateOthers(Tab selected)
        {
            foreach (Tab tab in _tabs)
            {
                if (tab != selected && tab.isOn)
                    tab.isOn = false;
            }
        }

        private void EnsureSelection()
        {
            Tab selected = null;

            foreach (Tab tab in _tabs)
            {
                if (!tab.isOn)
                    continue;

                if (selected == null)
                    selected = tab;
                else
                    tab.SetIsOnWithoutNotify(false);
            }

            if (selected == null && _tabs.Length > 0)
                _tabs[0].isOn = true;
        }
    }
}