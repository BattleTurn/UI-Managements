using System;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class SequenceDoTweenAnimator : MonoBehaviour
    {
        public event Action<bool> OnAnimationStarted;
        public event Action<bool> OnAnimationCompleted;

        private enum EExecuteType
        {
            Join,
            Append
        }

        [Serializable]
        private sealed class SequenceExecutor
        {
            [SerializeField] private DoTweenAnimatorBase _animator;
            [SerializeField] private EExecuteType _executeType;
            [SerializeField] private bool _isPlayOn = true;
            [SerializeField] private bool _isInitialStateBeforePlay = false;
            [SerializeField] private float _delay = 0f;

            public EExecuteType ExecuteType => _executeType;

            public Tween ExecuteTween()
            {
                if (_animator == null)
                {
                    return null;
                }

                return _animator.DoPlay(_isPlayOn, _isInitialStateBeforePlay, false).SetDelay(_delay);
            }
        }

        [SerializeField] private bool _playOnAwake = false;
        [SerializeField] private bool _playOnEnable = false;
        [SerializeField] private bool _playOnStart = false;
        [ShowIf(nameof(ShowLoopCountField))]
        [SerializeField] private byte _loopCount = 1;
        [SerializeField] private bool _infiniteLoop = false;
        [ShowIf(nameof(ShowLoopTypeField))]
        [SerializeField] private bool _useLoopType = false;
        [ShowIf(nameof(_useLoopType))]
        [SerializeField] private LoopType _loopType = LoopType.Yoyo;

        [SerializeField] private SequenceExecutor[] _executors;

        private Sequence _sequence;

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Play()
        {
            StartExecute();

            _sequence.OnStart(() =>
            {
                OnAnimationStarted?.Invoke(true);
            });
            _sequence.OnComplete(() =>
            {
                OnAnimationCompleted?.Invoke(true);
            });

            _sequence.Play();
        }

        public void PlayLoop(short loopCount, bool notifyEvents = true)
        {
            StartExecute();

            DoPlayLoop(loopCount, notifyEvents);
        }

        public void PlayLoop(short loopCount, LoopType loopType, bool notifyEvents = true)
        {
            StartExecute();

            DoPlayLoop(loopCount, notifyEvents, loopType);
        }

        private void StartExecute()
        {
            KillSequence();

            _sequence = DOTween.Sequence();

            if (_executors == null || _executors.Length == 0)
            {
                return;
            }

            foreach (var executor in _executors)
            {
                ExecuteHandle(executor);
            }
        }

        private void KillSequence()
        {
            if (_sequence == null)
            {
                return;
            }

            if (_sequence.IsActive())
            {
                _sequence.Kill(false);
            }

            _sequence = null;
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void Stop()
        {
            KillSequence();
        }

        private void Awake()
        {
            if (!_playOnAwake)
            {
                return;
            }

            PlaySettingAnim();
        }

        private void Start()
        {
            if (!_playOnStart)
            {
                return;
            }

            PlaySettingAnim();
        }

        private void OnEnable()
        {
            if (!_playOnEnable)
            {
                return;
            }

            PlaySettingAnim();
        }

        private void OnDisable()
        {
            if (_playOnEnable)
            {
                Stop();
            }
        }

        private void OnDestroy()
        {
            Stop();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        private void PlayLoop()
        {
            StartExecute();
            DoPlayLoop(_infiniteLoop ? (short)-1 : _loopCount, true, _useLoopType ? _loopType : null);
        }

        private void DoPlayLoop(short loopCount, bool notifyEvents = true, LoopType? loopType = null)
        {
            if (loopType.HasValue)
            {
                _sequence.SetLoops(loopCount, loopType.Value);
            }
            else
            {
                _sequence.SetLoops(loopCount);
            }

            if (notifyEvents)
            {
                _sequence.OnStart(() =>
                {
                    OnAnimationStarted?.Invoke(true);
                });

                _sequence.OnComplete(() =>
                {
                    OnAnimationCompleted?.Invoke(true);
                });
            }

            _sequence.Play();
        }

        private void PlaySettingAnim()
        {
            if (_infiniteLoop)
            {
                if (_useLoopType)
                {
                    PlayLoop(-1, _loopType);
                }
                else
                {
                    PlayLoop(-1);
                }
            }
            else if (_loopCount > 1)
            {
                if (_useLoopType)
                {
                    PlayLoop(_loopCount, _loopType);
                }
                else
                {
                    PlayLoop(_loopCount);
                }
            }
            else
                Play();
        }

        private void ExecuteHandle(SequenceExecutor executor)
        {
            if (executor == null)
            {
                return;
            }

            var tween = executor.ExecuteTween();
            if (tween == null)
            {
                return;
            }

            if (executor.ExecuteType == EExecuteType.Join)
            {
                _sequence.Join(tween);
            }
            else if (executor.ExecuteType == EExecuteType.Append)
            {
                _sequence.Append(tween);
            }
        }

        private bool ShowLoopCountField()
        {
            return (_playOnEnable || _playOnAwake || _playOnStart) && !_infiniteLoop;
        }

        private bool ShowLoopTypeField()
        {
            return (_playOnEnable || _playOnAwake || _playOnStart ) && (_loopCount > 1 || _infiniteLoop);
        }
    }
}