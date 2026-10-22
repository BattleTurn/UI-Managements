using System;
using UnityEngine;
using UnityEngine.UI;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    [RequireComponent(typeof(Toggle))]
    public class LockableToggle : MonoBehaviour
    {
        [SerializeField] private bool _isLocked;
        [SerializeField] private AnimatorSettings _graphicAnimatorSettings;
        [SerializeField] private AnimatorSettings _lockGraphicAnimatorSettings;

        [Serializable]
        private sealed class AnimatorSettings
        {
            public BaseAnimator[] animators;
            [SerializeField] private SwitchSettings on;
            [SerializeField] private SwitchSettings off;

            public SwitchSettings GetSwitchSettings(bool isOn)
            {
                return isOn ? on : off;
            }
        }

        [Serializable]
        private sealed class SwitchSettings
        {
            public bool enableOnStart;
            public bool disableOnComplete;
        }

        private Toggle _toggle;

        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                RefreshLockUI(value, Application.isPlaying);
            }
        }

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            SetupAnimators();
            RefreshLockUI(_isLocked, true);
        }


#if UNITY_EDITOR
        private void OnValidate()
        {
            RefreshLockUI(_isLocked, Application.isPlaying);
        }
#endif

        private void SetupAnimators()
        {
            foreach (var animator in _graphicAnimatorSettings.animators)
            {
                if (animator != null)
                {
                    animator.PlayImmediate(!_isLocked, false);
                }
            }

            foreach (var animator in _lockGraphicAnimatorSettings.animators)
            {
                if (animator != null)
                {
                    animator.PlayImmediate(_isLocked, false);
                }
            }
        }

        private void OnStartPlay(BaseAnimator animator)
        {
            if (animator.IsOn)
                animator.gameObject.SetActive(true);
        }

        private void OnPlayCompleted(BaseAnimator animator)
        {
            if (!animator.IsOn)
                animator.gameObject.SetActive(false);
        }

        private void RefreshLockUI(bool value, bool isAnimating)
        {
            _isLocked = value;
            if (_toggle == null)
            {
                _toggle = GetComponent<Toggle>();
            }
            _toggle.interactable = !value;

            if (isAnimating)
            {
                SwitchSettings switchSettings = _graphicAnimatorSettings.GetSwitchSettings(!_isLocked);
                SwitchSettings lockSwitchSettings = _lockGraphicAnimatorSettings.GetSwitchSettings(_isLocked);
                DoAnimate(_graphicAnimatorSettings.animators, !_isLocked, switchSettings.enableOnStart, switchSettings.disableOnComplete);
                DoAnimate(_lockGraphicAnimatorSettings.animators, _isLocked, lockSwitchSettings.enableOnStart, lockSwitchSettings.disableOnComplete);
            }
            else
            {
                DoImmediately(_graphicAnimatorSettings.animators, !_isLocked);
                DoImmediately(_lockGraphicAnimatorSettings.animators, _isLocked);
            }
        }

        private void DoImmediately(BaseAnimator[] animators, bool isOn)
        {
            if (animators == null || animators.Length == 0)
            {
                return;
            }

            foreach (var animator in animators)
            {
                if (animator != null)
                {
                    animator.PlayImmediate(isOn);
                }
            }
        }

        private void DoAnimate(BaseAnimator[] animators, bool isOn, bool enableOnStart, bool disableOnComplete)
        {
            if (animators == null || animators.Length == 0)
            {
                return;
            }

            foreach (var animator in animators)
            {
                if (animator != null)
                {
                    animator.Play(isOn, true, true, (isPlayOn) => { if (enableOnStart) OnStartPlay(animator); }
                    , (isPlayOn) => { if (disableOnComplete) OnPlayCompleted(animator); });
                }
            }
        }
    }
}