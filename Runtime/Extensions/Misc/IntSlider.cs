using System;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    [RequireComponent(typeof(Slider))]
    public class IntSlider : MonoBehaviour
    {
        public event Action<int> OnValueChanged;
        [SerializeField, ReadOnly] private Slider _slider;

        public int Value
        {
            get
            {
                GetSlider();
                return Mathf.RoundToInt(_slider.value);
            }
            set
            {
                GetSlider();
                _slider.value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        public int MinValue
        {
            get
            {
                GetSlider();
                return Mathf.RoundToInt(_slider.minValue);
            }
            set
            {
                GetSlider();
                _slider.minValue = value;
            }
        }

        public int MaxValue
        {
            get
            {
                GetSlider();
                return Mathf.RoundToInt(_slider.maxValue);
            }
            set
            {
                GetSlider();
                _slider.maxValue = value;
            }
        }

        public void SetValueWithoutNotify(int value)
        {
            _slider.SetValueWithoutNotify(value);
        }

        private void Awake()
        {
            if (_slider == null) _slider = GetComponent<Slider>();
            _slider.wholeNumbers = true;
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_slider == null) _slider = GetComponent<Slider>();
            _slider.wholeNumbers = true;
        }
#endif

        private void OnSliderValueChanged(float value)
        {
            OnValueChanged?.Invoke(Mathf.RoundToInt(value));
        }

        private void GetSlider()
        {
            if (_slider == null) _slider = GetComponent<Slider>();
        }
    }
}