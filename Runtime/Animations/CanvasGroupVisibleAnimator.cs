using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace BattleTurn.UIManagements.Runtime
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupVisibleAnimator : ViewVisibleAnimator
    {
        [SerializeField] private EDeltaTime _deltaTimeType = EDeltaTime.Scaled;
        [SerializeField] private float _fadeDuration = 0.3f;
        [Tooltip("If true, the canvas group will be initialized the alpha to the opposite state before Hide/Show.")]
        [SerializeField] private bool _initializeState = true;

        private CanvasGroup _canvasGroup;

        protected override void Awake()
        {
            base.Awake();
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        [Button]
        protected override void HandleShow()
        {
            if (_initializeState)
            {
                _canvasGroup.alpha = 0f;
            }
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(1f, _fadeDuration).SetUpdate(_deltaTimeType == EDeltaTime.Unscaled).OnComplete(InvokeOnShowComplete);
        }

        [Button]
        protected async override UniTask HandleHide()
        {
            if (_initializeState)
            {
                _canvasGroup.alpha = 1f;
            }

            _canvasGroup.blocksRaycasts = false;
            await _canvasGroup.DOFade(0f, _fadeDuration).SetUpdate(_deltaTimeType == EDeltaTime.Unscaled).OnComplete(InvokeOnHideComplete);
        }

        protected override void InvokeOnShowComplete()
        {
            _canvasGroup.blocksRaycasts = true;
            base.InvokeOnShowComplete();
        }
    }
}