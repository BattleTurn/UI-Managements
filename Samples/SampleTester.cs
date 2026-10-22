using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace BattleTurn.UIManagements.Runtime.Samples
{
    public sealed class SampleTester : MonoBehaviour
    {
        [SerializeField] private UIManager _uiManager;

        [SerializeField] private List<SamplePopup> _popupInstances;

        [Button]
        private void TestShowScreenAtDefaultLayer()
        {
            _uiManager.ShowScreen<SampleScreen>(UILayer.DEFAULT);
        }

        [Button]
        private void TestShowScreenAtOverlayLayer()
        {
            _uiManager.ShowScreen<SampleScreen>(UILayer.OVERLAY);
        }

        [Button]
        private void TestHideScreen()
        {
            _uiManager.HideScreen<SampleScreen>();
        }

        [Button]
        private void TestShowPopupAtDefaultLayer()
        {
            SamplePopup popupInstance = _uiManager.ShowPopup<SamplePopup>(UILayer.DEFAULT);
            _popupInstances.Add(popupInstance);
        }

        [Button]
        private void TestShowPopupAtOverlayLayer()
        {
            SamplePopup popupInstance = _uiManager.ShowPopup<SamplePopup>(UILayer.OVERLAY);
            _popupInstances.Add(popupInstance);
        }

        [Button]
        private void TestHidePopup()
        {
            if (_popupInstances.Count > 0)
            {
                SamplePopup popupInstance = _popupInstances[0];
                _uiManager.HidePopup<SamplePopup>(popupInstance);
                _popupInstances.RemoveAt(0);
            }
        }
    }
}