using UnityEngine;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class ScaleVisibleAnimator : ViewVisibleAnimator
    {
        [SerializeField] private ScaleDoTweenAnimator[] _scaleDoTweenAnimators;
        [Tooltip("If true, the canvas group will be initialized the alpha to the opposite state before Hide/Show.")]
        [SerializeField] private bool _initializeState = false;

        [SerializeField, ReadOnly] private byte _completedCount = 0;
        [SerializeField, ReadOnly] private bool _isHideAnimationCompleted = false;

        protected override void Awake()
        {
            base.Awake();
            foreach (var animator in _scaleDoTweenAnimators)
            {
                animator.OnAnimationCompletedEvent += OnAnimationCompleted;
            }
        }

        private void OnAnimationCompleted(bool isCompleted)
        {
            _completedCount++;
            if (_completedCount >= _scaleDoTweenAnimators.Length)
            {
                _completedCount = 0;
                if (isCompleted)
                    base.HandleShow();
                else
                    _isHideAnimationCompleted = true;
            }
        }

        [Button]
        protected override void HandleShow()
        {
            foreach (var animator in _scaleDoTweenAnimators)
            {
                animator.Play(true, _initializeState);
            }
        }

        [Button]
        protected async override UniTask HandleHide()
        {
            _completedCount = 0;
            _isHideAnimationCompleted = false;
            foreach (var animator in _scaleDoTweenAnimators)
            {
                animator.Play(false, _initializeState);
            }
            await UniTask.WaitUntil(() => _isHideAnimationCompleted);
            _isHideAnimationCompleted = false;
            await base.HandleHide();
        }
    }
}