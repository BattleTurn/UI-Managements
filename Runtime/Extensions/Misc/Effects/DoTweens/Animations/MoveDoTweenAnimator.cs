using UnityEngine;
using DG.Tweening;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class MoveDoTweenAnimator : DoTweenAnimatorBase
    {
        public Transform On;
        public Transform Off;

        [SerializeField] private Ease _onEase = Ease.OutBack;
        [SerializeField] private Ease _offEase = Ease.InBack;
        [SerializeField] private float _animationDelay = 0f;
        [SerializeField] private float _animationDuration = 0.3f;

        private Sequence _sequence;
        private Tween _tween;

        public float Duration
        {
            get => _animationDuration;
            set => _animationDuration = value;
        }

        public override void PlayImmediate(bool isOn, bool notifyEvents = true)
        {
            if (notifyEvents)
            {
                AnimationStarted(isOn);
            }
            transform.DOKill();
            Vector3 targetPosition = isOn ? On.position : Off.position;
            transform.position = targetPosition;
            AnimationCompleted(isOn, notifyEvents);
        }

        public override void Stop()
        {
            DeactivateSequence();

            DeactivateTween();
        }

        public override Tween DoPlay(bool isOn, bool resetToInitialState = false, bool notifyEvents = true)
        {
            OnBeforeDoPlay(isOn, notifyEvents);

            if (resetToInitialState)
            {
                transform.position = isOn ? Off.position : On.position;
            }
            Ease targetEase = isOn ? _onEase : _offEase;
            Vector3 targetPosition = isOn ? On.position : Off.position;
            _tween = transform.DOMove(targetPosition, _animationDuration)
                .SetEase(targetEase)
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

        private void OnBeforeDoPlay(bool isOn, bool notifyEvents)
        {
            if (notifyEvents)
            {
                AnimationStarted(isOn);
            }

            DeactivateTween();
        }

        private void OnBeforeDoLoop()
        {
            DeactivateSequence();
        }

        private Sequence DoPlayLoop(bool notifyEvents)
        {
            Sequence sequence = DOTween.Sequence();
            transform.position = Off.position;
            sequence.Append(transform.DOMove(On.position, _animationDuration).SetEase(_onEase).OnStart(() => NotifyAnimationStarted(true, notifyEvents)).SetDelay(_animationDelay));
            sequence.Append(transform.DOMove(Off.position, _animationDuration).SetEase(_offEase).OnStart(() => NotifyAnimationStarted(false, notifyEvents)).SetDelay(_animationDelay));
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