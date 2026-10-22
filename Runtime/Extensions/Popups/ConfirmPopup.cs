using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleTurn.UIManagements.Runtime
{
    public interface IConfirmPopupData
    {
        public string Title { get; }
        public string Message { get; }
        public string ConfirmButtonText { get; }
    }
    public readonly struct ConfirmPopupData : IConfirmPopupData
    {
        private readonly string _title;
        private readonly string _message;
        private readonly string _confirmButtonText;

        public string Title => _title;
        public string Message => _message;
        public string ConfirmButtonText => _confirmButtonText;

        public ConfirmPopupData(string title, string message, string confirmButtonText = "Confirm")
        {
            _title = title;
            _message = message;
            _confirmButtonText = confirmButtonText;
        }
    }

    public class ConfirmPopup : PopupBase
    {
        public event System.Action OnConfirmEvent;

        [SerializeField] private Button _confirmButton;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private TMP_Text _confirmButtonText;

        public void Initialize(IConfirmPopupData data)
        {
            _titleText.text = data.Title;
            _messageText.text = data.Message;
            _confirmButtonText.text = data.ConfirmButtonText;
        }

        protected override void Awake()
        {
            base.Awake();
            _confirmButton.onClick.AddListener(Confirm);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnConfirmEvent = null;
        }

        private void OnDestroy()
        {
            _confirmButton.onClick.RemoveListener(Confirm);
        }

        private void Confirm()
        {
            OnConfirmEvent?.Invoke();
            HideSelf();
        }
    }
}