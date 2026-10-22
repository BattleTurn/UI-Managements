using DG.Tweening;
using UnityEngine;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class RotateDoTweenAnimator : DoTweenAnimatorBase
    {
        [SerializeField] private Vector3 _onRotation = Vector3.zero;
        [SerializeField] private Vector3 _offRotation = Vector3.zero;
        [SerializeField] private Ease _onEase = Ease.OutBack;
        [SerializeField] private Ease _offEase = Ease.InBack;
        [SerializeField] private float _animationDelay = 0f;
        [SerializeField] private float _animationDuration = 0.3f;
        [SerializeField] private RotateMode _rotateMode = RotateMode.Fast;

        private Sequence _sequence;
        private Tween _tween;

        public override void PlayImmediate(bool isOn, bool notifyEvents = true)
        {
            if (notifyEvents)
            {
                AnimationStarted(isOn);
            }
            DeactivateTween();

            Vector3 targetRotation = isOn ? _onRotation : _offRotation;
            transform.localEulerAngles = targetRotation;
            AnimationCompleted(isOn, notifyEvents);
        }

        public override void Stop()
        {
            DeactivateSequence();
            DeactivateTween();
        }

        public override Tween DoPlay(bool isOn, bool resetToInitialState = false, bool notifyEvents = true)
        {
            OnBeforeDoPlay(isOn, resetToInitialState, notifyEvents);

            Vector3 targetRotation = isOn ? _onRotation : _offRotation;
            Ease targetEase = isOn ? _onEase : _offEase;
            _tween = transform.DOLocalRotate(targetRotation, _animationDuration, _rotateMode)
            .SetEase(targetEase)
            .SetUpdate(deltaTimeType == EDeltaTime.Unscaled);
            return _tween;
        }

        public override Sequence DoPlayLoop(short loopCount, bool notifyEvents = true)
        {
            OnBeforeDoLoop();
            _sequence = DoPlayLoop(notifyEvents).SetLoops(loopCount).Play();
            return _sequence;
        }

        public override Sequence DoPlayLoop(short loopCount, LoopType loopType, bool notifyEvents = true)
        {
            OnBeforeDoLoop();
            _sequence = DoPlayLoop(notifyEvents).SetLoops(loopCount, loopType).Play();
            return _sequence;
        }

        private void OnBeforeDoPlay(bool isOn, bool resetToInitialState, bool notifyEvents)
        {
            if (notifyEvents)
            {
                AnimationStarted(isOn);
            }

            DeactivateTween();

            if (resetToInitialState)
            {
                transform.localEulerAngles = isOn ? _offRotation : _onRotation;
            }
        }

        private void OnBeforeDoLoop()
        {
            DeactivateSequence();
        }

        private Sequence DoPlayLoop(bool notifyEvents)
        {
            Sequence sequence = DOTween.Sequence();
            transform.localEulerAngles = _offRotation;
            sequence.Append(transform.DOLocalRotate(_onRotation, _animationDuration, _rotateMode).SetEase(_onEase).OnStart(() => NotifyAnimationStarted(true, notifyEvents)).SetDelay(_animationDelay));
            sequence.Append(transform.DOLocalRotate(_offRotation, _animationDuration, _rotateMode).SetEase(_offEase).OnStart(() => NotifyAnimationStarted(false, notifyEvents)).SetDelay(_animationDelay));
            sequence.SetUpdate(deltaTimeType == EDeltaTime.Unscaled);
            return sequence;
        }

        private void NotifyAnimationStarted(bool isOn, bool notifyEvents)
        {
            if (notifyEvents)
            {
                AnimationStarted(isOn);
            }
        }

        private void OnDisable()
        {
            if (playOnEnable)
            {
                DeactivateSequence();
                DeactivateTween();
            }
        }

        private void OnDestroy()
        {
            DeactivateSequence();
            DeactivateTween();
        }

        private void DeactivateTween()
        {
            _tween.Deactivate();
        }

        private void DeactivateSequence()
        {
            _sequence.Deactivate();
        }
    }
}