using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace BattleTurn.UIManagements.Runtime
{
    public abstract class PopupBase : UIViewer
    {
        public event Action OnCloseButtonClickedEvent;

        [SerializeField] private bool _useBackgroundAsClose = false;
        [SerializeField] private bool _useCloseButton = false;

        [ShowIf(nameof(_useCloseButton))]
        [SerializeField] private Button _closeButton;

        [ShowIf(nameof(_useBackgroundAsClose))]
        [SerializeField] private Button _backgroundButton;

        protected override void Awake()
        {
            base.Awake();
            OnShownEvent += () => { if (_backgroundButton != null) _backgroundButton.interactable = true; };
            _backgroundButton?.onClick.AddListener(CloseButtonClicked);
            _closeButton?.onClick.AddListener(CloseButtonClicked);
        }

        protected virtual void OnDisable()
        {
            OnCloseButtonClickedEvent = null;
        }

        protected override void HideSelf()
        {
            uiManager.HidePopup(this);
        }

        protected override void ShowSelf()
        {
            uiManager.ShowPopup(this);
        }

        protected void CloseButtonClicked()
        {
            if (_backgroundButton != null)
                _backgroundButton.interactable = false;
            OnCloseButtonClickedEvent?.Invoke();
            HideSelf();
        }
    }
}