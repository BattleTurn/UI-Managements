using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class FadeDoTweenAnimator : DoTweenAnimatorBase
    {
        [SerializeField] private Image _graphic;
        [SerializeField] private float _onAlpha = 1f;
        [SerializeField] private float _offAlpha = 0f;
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

            float targetAlpha = isOn ? _onAlpha : _offAlpha;
            _graphic.color = new Color(_graphic.color.r, _graphic.color.g, _graphic.color.b, targetAlpha);
            AnimationCompleted(isOn, notifyEvents);
        }

        public override Tween DoPlay(bool isOn, bool resetToInitialState = false, bool notifyEvents = true)
        {
            OnBeforeDoPlay(isOn, resetToInitialState, notifyEvents);

            float targetAlpha = isOn ? _onAlpha : _offAlpha;
            _tween = _graphic.DOFade(targetAlpha, _animationDuration)
            .SetDelay(_animationDelay)
            .SetUpdate(deltaTimeType == EDeltaTime.Unscaled);

            return _tween;
        }

        public override Sequence DoPlayLoop(short loopCount, bool notifyEvents = true)
        {
            OnBeforeDoLoop();

            _sequence = DoPlayLoop(notifyEvents);
            _sequence.SetLoops(loopCount);
            return _sequence;
        }

        public override Sequence DoPlayLoop(short loopCount, LoopType loopType, bool notifyEvents = true)
        {
            OnBeforeDoLoop();
            _sequence = DoPlayLoop(notifyEvents);
            _sequence.SetLoops(loopCount, loopType);
            return _sequence;
        }

        private Sequence DoPlayLoop(bool notifyEvents)
        {
            Sequence sequence = DOTween.Sequence();
            _graphic.color = new Color(_graphic.color.r, _graphic.color.g, _graphic.color.b, _offAlpha);
            sequence.Append(_graphic.DOFade(_onAlpha, _animationDuration).SetEase(Ease.Linear).OnStart(() => NotifyAnimationStarted(true, notifyEvents)).SetDelay(_animationDelay));
            sequence.Append(_graphic.DOFade(_offAlpha, _animationDuration).SetEase(Ease.Linear).OnStart(() => NotifyAnimationStarted(false, notifyEvents)).SetDelay(_animationDelay));
            sequence.SetUpdate(deltaTimeType == EDeltaTime.Scaled);
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

        public override void Stop()
        {
            DeactivateSequence();
            DeactivateTween();
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
                _graphic.color = new Color(_graphic.color.r, _graphic.color.g, _graphic.color.b, isOn ? _offAlpha : _onAlpha);
            }
        }

        private void OnBeforeDoLoop()
        {
            DeactivateSequence();
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

        private void DeactivateSequence()
        {
            _sequence.Deactivate();
        }

        private void DeactivateTween()
        {
            _tween.Deactivate();
        }
    }
}