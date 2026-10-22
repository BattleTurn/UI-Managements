using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Text;
using TMPro;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public enum AmountType
    {
        Currency,
        Unit
    }

    public sealed class RewardPopup : PopupBase
    {
        public event Action OnRewardClaimed;

        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TMP_Text _rewardAmountText;
        [SerializeField] private Button _claimButton;

        protected override void Awake()
        {
            base.Awake();
            _claimButton.onClick.AddListener(OnClaimButtonClicked);
        }

        protected override void OnDisable()
        {
            OnRewardClaimed = null;
        }

        private void OnClaimButtonClicked()
        {
            OnRewardClaimed?.Invoke();
            HideSelf();
        }

        public void ShowReward(long reward, AmountType type, Sprite icon)
        {
            _rewardIcon.sprite = icon;
            _rewardAmountText.SetText(GetAmountTypeString(reward, type));
        }

        private string GetAmountTypeString(long reward, AmountType type)
        {
            switch (type)
            {
                case AmountType.Currency:
                    return ZString.Format("x{0:N0}", reward);
                case AmountType.Unit:
                    return ZString.Format("x{0}", reward);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}