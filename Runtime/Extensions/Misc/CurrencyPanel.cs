using TMPro;
using UnityEngine;
using Cysharp.Text;
using DG.Tweening;
using System;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class CurrencyPanel : MonoBehaviour
    {
        public event Action OnEffectCompleted;

        [SerializeField] private string _prefix = "";
        [SerializeField] private string _suffix = "";
        [SerializeField] private BaseAnimator _icon;
        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private bool _useCommaSeparated = true;
        [SerializeField] private float _effectDuration = 1f;

        private long _currentAmount, _effectFakeAmount;
        private Tween _effectTween;

        public Transform Icon => _icon?.transform;

        public long UpdateCurrency(long amount, bool immediately, bool notify = true)
        {
            _currentAmount = amount;
            return UpdateValue(immediately, notify);
        }

        public ulong UpdateCurrency(ulong amount, bool immediately, bool notify = true)
        {
            _currentAmount = (long)Math.Clamp(amount, 0, long.MaxValue);
            return (ulong)UpdateValue(immediately, notify);
        }

        public ulong AddCurrency(ulong amount, bool immediately, bool notify = true)
        {
            _currentAmount += (long)Math.Clamp(amount, 0, long.MaxValue);
            return (ulong)UpdateValue(immediately, notify);
        }

        public long AddCurrency(long amount, bool immediately, bool notify = true)
        {
            _currentAmount += amount;
            return UpdateValue(immediately, notify);
        }

        private long UpdateValue(bool immediately, bool notify)
        {
            if (immediately)
            {
                UpdateTextImmediately();
                return _currentAmount;
            }

            if (notify)
            {
                _icon?.PlayLoop(1, true);
            }

            DoEffect();
            return _currentAmount;
        }

        private void UpdateTextImmediately()
        {
            UpdateText(_currentAmount);
            _effectFakeAmount = _currentAmount;
        }

        private void DoEffect()
        {
            if (_effectTween != null)
            {
                _effectTween.Kill(true);
                _effectTween = null;
            }
            _effectTween = DOTween.To(() => _effectFakeAmount, x => _effectFakeAmount = x, _currentAmount, _effectDuration)
                .OnUpdate(() => UpdateText(_effectFakeAmount))
                .OnComplete(() =>
                {
                    UpdateText(_currentAmount);
                    _effectFakeAmount = _currentAmount;
                    OnEffectCompleted?.Invoke();
                })
                .OnKill(() => _effectTween = null);
        }

        private void UpdateText(long amount)
        {
            _currencyText.SetText(ZString.Format("{0}{1}{2}", _prefix, GetAmountString(amount), _suffix));
        }

        private string GetAmountString(long amount)
        {
            if (_useCommaSeparated)
            {
                return amount.ToString("N0");
            }
            return amount.ToString();
        }

        #region UNITY CALLBACKS
        private void OnDestroy()
        {
            OnEffectCompleted = null;
            if (_effectTween != null)
            {
                _effectTween.Kill(true);
                _effectTween = null;
            }
        }

        private void OnDisable()
        {
            OnEffectCompleted = null;
            if (_effectTween != null)
            {
                _effectTween.Kill(true);
                _effectTween = null;
            }
        }
        #endregion
    }
}