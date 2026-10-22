using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using TMPro;

namespace BattleTurn.UIManagements.Runtime
{
    public interface IYesNoPopupData
    {
        string Title { get; }
        string Message { get; }
        string ConfirmButtonText { get; }
        string CancelButtonText { get; }
    }

    public readonly struct YesNoPopupData : IYesNoPopupData
    {
        private readonly string _title;
        private readonly string _message;
        private readonly string _confirmButtonText;
        private readonly string _cancelButtonText;

        public string Title => _title;
        public string Message => _message;
        public string ConfirmButtonText => _confirmButtonText;
        public string CancelButtonText => _cancelButtonText;

        public YesNoPopupData(string title, string message, string confirmButtonText = "Yes", string cancelButtonText = "No")
        {
            _title = title;
            _message = message;
            _confirmButtonText = confirmButtonText;
            _cancelButtonText = cancelButtonText;
        }
    }

    public class YesNoPopup : PopupBase
    {
        public event Action OnConfirm;
        public event Action OnCancel;

        [BoxGroup("BUTTONS")]
        [SerializeField] protected Button confirmButton;

        [BoxGroup("BUTTONS")]
        [SerializeField] private Button _cancelButton;

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private TMP_Text _confirmButtonText;
        [SerializeField] private TMP_Text _cancelButtonText;

        protected override void Awake()
        {
            base.Awake();
            confirmButton.onClick.AddListener(Confirm);
            _cancelButton.onClick.AddListener(Cancel);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnConfirm = null;
            OnCancel = null;
        }

        private void OnDestroy()
        {
            confirmButton.onClick.RemoveListener(Confirm);
            _cancelButton.onClick.RemoveListener(Cancel);
        }

        public void Initialize(IYesNoPopupData data)
        {
            _confirmButtonText.text = data.ConfirmButtonText;
            _cancelButtonText.text = data.CancelButtonText;
            _titleText.text = data.Title;
            _messageText.text = data.Message;
        }

        private void Confirm()
        {
            OnConfirm?.Invoke();
            HideSelf();
        }

        private void Cancel()
        {
            OnCancel?.Invoke();
            HideSelf();
        }
    }
}