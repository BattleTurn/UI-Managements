using System;
using DG.Tweening;
using UnityEngine;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public abstract class DoTweenAnimatorBase : BaseAnimator
    {
        [SerializeField] protected EDeltaTime deltaTimeType = EDeltaTime.Scaled;

        public override void Play(bool isOn, bool resetToInitialState = false, bool notifyEvents = true, Action<bool> onStartCallback = null, Action<bool> onCompleteCallback = null)
        {
            if (enableOnBeforePlay && isOn)
                gameObject.SetActive(true);
            DoPlay(isOn, resetToInitialState, notifyEvents).OnStart(() =>
            {
                onStartCallback?.Invoke(isOn);
            }).OnComplete(() =>
            {
                onCompleteCallback?.Invoke(isOn);
                AnimationCompleted(isOn, notifyEvents);
            });
        }

        public override void PlayLoop(byte loopCount, bool notifyEvents = true, Action<bool> onStartCallback = null, Action<bool> onCompleteCallback = null)
        {
            if (enableOnBeforePlay)
                gameObject.SetActive(true);
            DoPlayLoop(loopCount, notifyEvents).OnStart(() =>
            {
                onStartCallback?.Invoke(true);
            }).OnComplete(() =>
            {
                onCompleteCallback?.Invoke(true);
                AnimationCompleted(loopCount % 2 == 0, notifyEvents);
            });
        }

        public override void PlayLoop(byte loopCount, LoopType loopType, bool notifyEvents = true, Action<bool> onStartCallback = null, Action<bool> onCompleteCallback = null)
        {
            if (enableOnBeforePlay)
                gameObject.SetActive(true);
            DoPlayLoop(loopCount, loopType, notifyEvents).OnStart(() =>
            {
                onStartCallback?.Invoke(true);
            }).OnComplete(() =>
            {
                onCompleteCallback?.Invoke(true);
                AnimationCompleted(loopCount % 2 == 0, notifyEvents);
            });
        }

        public override void PlayLoopInfinite(bool notifyEvents = true, Action<bool> onStartCallback = null)
        {
            if (enableOnBeforePlay)
                gameObject.SetActive(true);
            Debug.Log($"PlayLoopInfinite: Starting infinite loop animation on {gameObject.name}");
            DoPlayLoop(-1, notifyEvents).OnStart(() =>
            {
                onStartCallback?.Invoke(true);
            });
        }

        public override void PlayLoopInfinite(LoopType loopType, bool notifyEvents = true, Action<bool> onStartCallback = null)
        {
            if (enableOnBeforePlay)
                gameObject.SetActive(true);
            DoPlayLoop(-1, loopType, notifyEvents).OnStart(() =>
            {
                onStartCallback?.Invoke(true);
            });
        }

        public abstract Tween DoPlay(bool isOn, bool resetToInitialState = false, bool notifyEvents = true);
        public abstract Sequence DoPlayLoop(short loopCount, bool notifyEvents = true);
        public abstract Sequence DoPlayLoop(short loopCount, LoopType loopType, bool notifyEvents = true);
    }
}