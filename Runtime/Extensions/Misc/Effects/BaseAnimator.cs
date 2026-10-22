
using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public abstract class BaseAnimator : MonoBehaviour
    {
        public event Action<bool> OnAnimationStartEvent;
        public event Action<bool> OnAnimationCompletedEvent;

        [SerializeField] private bool _playOnAwake = false;
        [SerializeField] protected bool playOnEnable = false;
        [Tooltip("If true, the gameObject will be enabled before the ON animation starts playing. If loop animation, the gameObject will be enabled before the loop starts playing.")]
        [SerializeField] protected bool enableOnBeforePlay = false;
        [Tooltip("If true, the gameObject will be disabled after the OFF animation completes.")]
        [SerializeField] private bool _disableOnOffComplete = false;
        [ShowIf(nameof(ShowLoopCountField))]
        [SerializeField] private byte _loopCount = 1;
        [SerializeField] private bool _infiniteLoop = false;
        [ShowIf(nameof(ShowLoopTypeField))]
        [SerializeField] private bool _useLoopType = false;
        [ShowIf(nameof(_useLoopType))]
        [SerializeField] private LoopType _loopType = LoopType.Yoyo;

        [SerializeField, ReadOnly] private bool _isOn;

        public bool IsOn => _isOn;

        public bool EnableBeforeAnimationComplete
        {
            get => enableOnBeforePlay;
            set => enableOnBeforePlay = value;
        }

        public bool DisableOffAnimationComplete
        {
            get => _disableOnOffComplete;
            set => _disableOnOffComplete = value;
        }

        /// <summary>
        /// Plays the animation.
        /// </summary>
        /// <param name="isOn">Indicates whether the animation is turning on or off.</param>
        /// <param name="resetToInitialState">Indicates whether to reset to the initial state before playing.</param>
        /// <param name="notifyEvents">Indicates whether to notify events.</param>
        public abstract void Play(bool isOn, bool resetToInitialState = false, bool notifyEvents = true, Action<bool> onStartCallback = null, Action<bool> onCompleteCallback = null);

        /// <summary>
        /// Plays the animation immediately without any transition.
        /// </summary>
        /// <param name="isOn">Indicates whether the animation is turning on or off.</param>
        /// <param name="notifyEvents">Indicates whether to notify events.</param>
        public abstract void PlayImmediate(bool isOn, bool notifyEvents = true);

        /// <summary>
        /// Plays the animation in a loop.
        /// </summary>
        /// <param name="loopCount">The number of times to loop the animation.</param>
        /// <param name="notifyEvents">Indicates whether to notify events.</param>
        /// <param name="onStartCallback">Callback invoked when the loop starts.</param>
        /// <param name="onCompleteCallback">Callback invoked when the loop completes.</param>
        public abstract void PlayLoop(byte loopCount, bool notifyEvents = true, Action<bool> onStartCallback = null, Action<bool> onCompleteCallback = null);

        /// <summary>
        /// Plays the animation in a loop with a specified loop type.
        /// </summary>
        /// <param name="loopCount">The number of times to loop the animation.</param>
        /// <param name="loopType">The type of loop to use.</param>
        /// <param name="notifyEvents">Indicates whether to notify events.</param>
        /// <param name="onStartCallback">Callback invoked when the loop starts.</param>
        /// <param name="onCompleteCallback">Callback invoked when the loop completes.</param>
        public abstract void PlayLoop(byte loopCount, LoopType loopType, bool notifyEvents = true, Action<bool> onStartCallback = null, Action<bool> onCompleteCallback = null);

        /// <summary>
        /// Plays the animation in a loop.
        /// </summary>
        /// <param name="notifyEvents">Indicates whether to notify events.</param>
        /// <param name="onStartCallback">Callback invoked when the loop starts.</param>
        public abstract void PlayLoopInfinite(bool notifyEvents = true, Action<bool> onStartCallback = null);

        /// <summary>
        /// Plays the animation in a loop with a specified loop type.
        /// </summary>
        /// <param name="loopType">The type of loop to use.</param>
        /// <param name="notifyEvents">Indicates whether to notify events.</param>
        /// <param name="onStartCallback">Callback invoked when the loop starts.</param>
        public abstract void PlayLoopInfinite(LoopType loopType, bool notifyEvents = true, Action<bool> onStartCallback = null);


        /// <summary>
        /// Stops the animation.
        /// </summary>
        public abstract void Stop();

        protected void AnimationCompleted(bool isOn, bool notifyEvents)
        {
            if (notifyEvents)
            {
                OnAnimationCompletedEvent?.Invoke(isOn);
            }

            if (_disableOnOffComplete && !isOn)
            {
                gameObject.SetActive(false);
            }
        }

        protected void AnimationStarted(bool isOn)
        {
            _isOn = isOn;
            OnAnimationStartEvent?.Invoke(isOn);
        }

        [Button]
        private void PlayOff()
        {
            Play(false, true);
        }

        [Button]
        private void PlayOn()
        {
            Play(true, true);
        }

        [Button]
        private void PlayLoop()
        {
            PlaySettingAnim();
        }

        protected virtual void Awake()
        {
            if (!_playOnAwake)
            {
                return;
            }

            PlaySettingAnim();
        }

        protected virtual void OnEnable()
        {
            if (!playOnEnable)
            {
                return;
            }

            PlaySettingAnim();
        }

        private void PlaySettingAnim()
        {
            if (_infiniteLoop)
            {
                if (_useLoopType)
                {
                    PlayLoopInfinite(_loopType);
                }
                else
                {
                    PlayLoopInfinite();
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
                Play(true, true);
        }

        private bool ShowLoopCountField()
        {
            return (playOnEnable || _playOnAwake) && !_infiniteLoop;
        }

        private bool ShowLoopTypeField()
        {
            return (playOnEnable || _playOnAwake) && (_loopCount > 1 || _infiniteLoop);
        }
    }
}