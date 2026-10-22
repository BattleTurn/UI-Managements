using System;
using DG.Tweening;
using UnityEngine;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class ScaleDoTweenAnimator : DoTweenAnimatorBase
    {
        [SerializeField] private Vector3 _onScale = Vector3.one;
        [SerializeField] private Vector3 _offScale = Vector3.zero;
        [SerializeField] private Ease _onEase = Ease.OutBack;
        [SerializeField] private Ease _offEase = Ease.InBack;
        [SerializeField] private float _animationDelay = 0f;
        [SerializeField] private float _animationDuration = 0.3f;

        private Sequence _sequence;
        private Tween _tween;

        public override void PlayImmediate(bool isOn, bool notifyEvents = true)
        {
            if (notifyEvents)
            {
                AnimationStarted(isOn);
            }

            DeactivateTween();
            Vector3 targetScale = isOn ? _onScale : _offScale;
            transform.localScale = targetScale;
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
            Vector3 targetScale = isOn ? _onScale : _offScale;
            Ease targetEase = isOn ? _onEase : _offEase;
            _tween = transform.DOScale(targetScale, _animationDuration)
            .SetEase(targetEase)
            .SetDelay(_animationDelay)
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
                transform.localScale = isOn ? _offScale : _onScale;
            }
        }

        private void OnBeforeDoLoop()
        {
            DeactivateSequence();
        }

        private Sequence DoPlayLoop(bool notifyEvents)
        {
            Sequence sequence = DOTween.Sequence();
            transform.localScale = _offScale;
            sequence.Append(transform.DOScale(_onScale, _animationDuration).SetEase(_onEase).OnStart(() => NotifyAnimationStarted(true, notifyEvents)).SetDelay(_animationDelay));
            sequence.Append(transform.DOScale(_offScale, _animationDuration).SetEase(_offEase).OnStart(() => NotifyAnimationStarted(false, notifyEvents)).SetDelay(_animationDelay));
            sequence.SetUpdate(deltaTimeType == EDeltaTime.Unscaled);
            sequence.OnComplete(() =>
            {
                AnimationCompleted(true, notifyEvents);
            });
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