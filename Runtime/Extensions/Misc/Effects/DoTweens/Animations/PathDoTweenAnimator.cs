using System;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class PathDoTweenAnimator : DoTweenAnimatorBase
    {
        [ShowIf(nameof(IsUseTransformTarget))]
        [SerializeField] private Transform _start;
        [ShowIf(nameof(IsUseTransformTarget))]
        [SerializeField] private Transform _end;

        [ShowIf(nameof(_useVectorTarget))]
        [SerializeField] private Vector3 _startPosition;
        [ShowIf(nameof(_useVectorTarget))]
        [SerializeField] private Vector3 _endPosition;

        [SerializeField] private bool _useVectorTarget = false;
        [SerializeField] private Ease _endEase = Ease.OutBack;
        [SerializeField] private Ease _startEase = Ease.InBack;

        [SerializeField] private float _animationDelay = 0f;
        [SerializeField] private float _animationDuration = 0.3f;
        [MinValue(0.1f)]
        [SerializeField] private float _radius = 1f;

        [SerializeField] private PathType _pathType = PathType.Linear;

        private Sequence _sequence;
        private Tween _tween;

        public Transform End
        {
            get
            {
                if (_useVectorTarget)
                {
                    throw new InvalidOperationException("End is not available when using vector target.");
                }
                return _end;
            }

            set
            {
                if (_useVectorTarget)
                {
                    throw new InvalidOperationException("End is not available when using vector target.");
                }
                _end = value;
            }
        }

        public Transform Start
        {
            get
            {
                if (_useVectorTarget)
                {
                    throw new InvalidOperationException("Start is not available when using vector target.");
                }
                return _start;
            }
            set
            {
                if (_useVectorTarget)
                {
                    throw new InvalidOperationException("Start is not available when using vector target.");
                }
                _start = value;
            }
        }

        public Vector3 EndPosition
        {
            get
            {
                if (!_useVectorTarget)
                {
                    throw new InvalidOperationException("EndPosition is not available when using transform target.");
                }
                return _endPosition;
            }
            set
            {
                if (!_useVectorTarget)
                {
                    throw new InvalidOperationException("EndPosition is not available when using transform target.");
                }
                _endPosition = value;
            }
        }

        public Vector3 StartPosition
        {
            get
            {
                if (!_useVectorTarget)
                {
                    throw new InvalidOperationException("StartPosition is not available when using transform target.");
                }
                return _startPosition;
            }
            set
            {
                if (!_useVectorTarget)
                {
                    throw new InvalidOperationException("StartPosition is not available when using transform target.");
                }
                _startPosition = value;
            }
        }

        public override void PlayImmediate(bool isOn, bool notifyEvents = true)
        {
            if (notifyEvents)
            {
                AnimationStarted(isOn);
            }

            DeactivateTween();

            Vector3 targetPosition = isOn ? (IsUseTransformTarget() ? _start.position : _startPosition) : (IsUseTransformTarget() ? _end.position : _endPosition);
            transform.position = targetPosition;
            AnimationCompleted(isOn, notifyEvents);
        }

        [Button]
        public override void Stop()
        {
            DeactivateSequence();
            DeactivateTween();
        }

        public override Tween DoPlay(bool isOn, bool resetToInitialState = false, bool notifyEvents = true)
        {
            OnBeforeDoPlay(isOn, resetToInitialState, notifyEvents);
            _tween = transform.DOPath(GetPath(isOn), _animationDuration, _pathType)
                .SetEase(isOn ? _startEase : _endEase)
                .SetDelay(_animationDelay)
                .SetUpdate(deltaTimeType == EDeltaTime.Unscaled);
            return _tween;
        }

        public override Sequence DoPlayLoop(short loopCount, bool notifyEvents)
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

        private Sequence DoPlayLoop(bool notifyEvents)
        {
            Sequence sequence = DOTween.Sequence();
            transform.position = IsUseTransformTarget() ? _end.position : _endPosition;
            sequence.Append(transform.DOPath(GetPath(true), _animationDuration, _pathType).SetEase(_startEase).OnStart(() => NotifyAnimationStarted(true, notifyEvents)).SetDelay(_animationDelay));
            sequence.Append(transform.DOPath(GetPath(false), _animationDuration, _pathType).SetEase(_endEase).OnStart(() => NotifyAnimationStarted(false, notifyEvents)).SetDelay(_animationDelay));
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

        private void OnBeforeDoPlay(bool isOn, bool resetToInitialState, bool notifyEvents)
        {
            if (notifyEvents)
            {
                AnimationStarted(isOn);
            }

            DeactivateTween();

            if (resetToInitialState)
            {
                transform.position = isOn ? (IsUseTransformTarget() ? _start.position : _startPosition) : (IsUseTransformTarget() ? _end.position : _endPosition);
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

        private Vector3[] GetPath(bool isOn)
        {
            Vector3 start = IsUseTransformTarget() ? _start.position : _startPosition;
            Vector3 end = IsUseTransformTarget() ? _end.position : _endPosition;

            Vector3 dir = (end - start).normalized;
            float distance = Vector3.Distance(start, end);

            Vector3 normal = Vector3.Cross(dir, Vector3.forward).normalized;

            Vector3 controlPoint1 = isOn ? Vector3.Lerp(start, end, 0.33f) : Vector3.Lerp(end, start, 0.33f);
            Vector3 controlPoint2 = isOn ? Vector3.Lerp(start, end, 0.66f) : Vector3.Lerp(end, start, 0.66f);

            // Offset sang hai bên
            float maxOffset = distance * _radius;

            controlPoint1 += normal * UnityEngine.Random.Range(-maxOffset, maxOffset);
            controlPoint2 += normal * UnityEngine.Random.Range(-maxOffset, maxOffset);
            return isOn ? new Vector3[] { end, controlPoint1, controlPoint2 } : new Vector3[] { start, controlPoint1, controlPoint2 };
        }

        private bool IsUseTransformTarget()
        {
            return !_useVectorTarget;
        }

    }
}