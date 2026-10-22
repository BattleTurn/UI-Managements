using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleAnimatorController : MonoBehaviour
    {
        [ShowIf(nameof(_useOwnGraphic))]
        [SerializeField] private BaseAnimator _targetGraphic;
        [SerializeField] private BaseAnimator[] _additionalGraphics;
        [SerializeField] private bool _useOwnGraphic = true;

        private Toggle _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
            PlayImmediate(_toggle.isOn);
        }

        private void OnDestroy()
        {
            _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            Play(isOn);
        }

        private void Play(bool isOn)
        {
            _targetGraphic?.Play(isOn);
            if (_additionalGraphics != null)
            {
                foreach (var graphic in _additionalGraphics)
                {
                    graphic?.Play(isOn);
                }
            }
        }

        private void PlayImmediate(bool isOn)
        {
            _targetGraphic?.PlayImmediate(isOn);
            if (_additionalGraphics != null)
            {
                foreach (var graphic in _additionalGraphics)
                {
                    graphic?.PlayImmediate(isOn);
                }
            }
        }

        private void OnValidate()
        {
            if (_useOwnGraphic)
            {
                if (GetComponent<Toggle>().graphic == null)
                {
                    return;
                }

                if (GetComponent<Toggle>().graphic.TryGetComponent(out BaseAnimator animator))
                {
                    _targetGraphic = animator;
                }
                GetComponent<Toggle>().graphic = null;
            }

        }
    }
}