using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    [RequireComponent(typeof(Button))]
    public class ButtonAnimatorController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private BaseAnimator[] _enterAnimators;
        [SerializeField] private BaseAnimator[] _downAnimators;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            PlayImmediately(_enterAnimators, false);
            PlayImmediately(_downAnimators, false);
        }

        private void Play(BaseAnimator[] animators, bool isOn)
        {
            foreach (var graphic in animators)
            {
                graphic?.Play(isOn);
            }
        }

        private void PlayImmediately(BaseAnimator[] animators, bool isOn)
        {
            foreach (var graphic in animators)
            {
                graphic?.PlayImmediate(isOn);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Play(_enterAnimators, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Play(_enterAnimators, false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Play(_downAnimators, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Play(_downAnimators, false);
        }
    }
}