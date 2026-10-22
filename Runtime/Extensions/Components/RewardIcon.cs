using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class RewardIcon : MonoBehaviour
    {
        public event Action OnAnimationCompletedEvent;

        [SerializeField] private Image _icon;
        [SerializeField] private ScaleDoTweenAnimator _scaleAnimator;
        [SerializeField] private PathDoTweenAnimator _moveAnimator;

        private void Awake()
        {
            if (_moveAnimator != null)
            {
                _moveAnimator.OnAnimationCompletedEvent += OnAnimationCompleted;
            }

            _scaleAnimator.OnAnimationCompletedEvent += PlayPathAnimation;
        }

        public void Initialize(Sprite icon, Vector3 start, Vector3 end)
        {
            _icon.sprite = icon;
            transform.position = start;
            _moveAnimator.StartPosition = start;
            _moveAnimator.EndPosition = end;
        }

        public void PlayAnimation()
        {
            _scaleAnimator.Play(true, true);
        }

        private void PlayPathAnimation(bool isOn)
        {
            AsyncPlayPathAnimation().Forget();
        }

        private async UniTaskVoid AsyncPlayPathAnimation()
        {
            await UniTask.WaitForSeconds(0.1f);
            _moveAnimator.Play(true);
        }

        private void OnAnimationCompleted(bool obj)
        {
            OnAnimationCompletedEvent?.Invoke();
        }
    }
}