using System;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class CurrencyRewardScreen : ScreenBase
    {
        /// <summary>
        /// Reward collected event handler. This event is triggered when the reward animation is completed and the reward has been collected.
        /// </summary>
        /// <param name="minusPerIcon">
        /// The amount of reward subtracted per icon in total currency amount.
        /// Example: If the total reward is 100 and there are 5 icons, this value will be 20, next will be 40, 60, 80, 100.
        /// Need to add the reward amount to the total currency amount first before the animation starts. Then, during the animation, this value can be used to subtract from the total currency amount to get the fake currency amount for the animation.
        /// User can use this value to subtract with currency amount to get the fake currency amount for the animation. The final value will be the total reward amount.
        /// </param>
        public delegate void RewardCollectedHandler(long minusPerIcon);
        public event RewardCollectedHandler OnRewardCollected;
        public event Action OnRewardAnimationCompleted;

        [SerializeField] private RewardIcon _rewardPrefab;
        [SerializeField] private Transform _rewardContainer;
        [SerializeField] private float _radius = 100f;

        private List<RewardIcon> _rewardIcons = new List<RewardIcon>();
        private Sprite _rewardIcon;

        private void OnEnable()
        {
            OnRewardAnimationCompleted = null;
            OnRewardCollected = null;
            ClearAllIcons();
        }

        private void OnDestroy()
        {
            OnRewardAnimationCompleted = null;
            OnRewardCollected = null;
            ClearAllIcons();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }

        public void Initialize(Sprite icon)
        {
            _rewardIcon = icon;
        }

        public async UniTaskVoid ShowReward(long rewardAmount, Vector3 end, byte spawnCount)
        {
            long rewardPerIcon = (long)(rewardAmount / spawnCount);
            for (byte i = 0; i < spawnCount; i++)
            {
                byte index = i; // Capture the current index for the closure
                RewardIcon rewardIcon = Instantiate(_rewardPrefab, _rewardContainer);
                _rewardIcons.Add(rewardIcon);
                rewardIcon.OnAnimationCompletedEvent += () =>
                {
                    rewardIcon.gameObject.SetActive(false);
                    RewardCollected(rewardPerIcon, index);
                    if (index == spawnCount - 1)
                    {
                        OnRewardAnimationCompleted?.Invoke();
                        HideSelf();
                    }
                };
            }

            for (byte i = 0; i < spawnCount; i++)
            {
                Vector2 randomPosition = UnityEngine.Random.insideUnitCircle * _radius;
                Vector3 spawnPosition = new Vector3(transform.position.x + randomPosition.x, transform.position.y + randomPosition.y, transform.position.z);
                _rewardIcons[i].Initialize(_rewardIcon, spawnPosition, end);
                _rewardIcons[i].PlayAnimation();
                float delay = UnityEngine.Random.Range(0.05f, 0.1f);
                await UniTask.WaitForSeconds(delay);
            }
        }

        private void RewardCollected(long rewardPerIcon, byte index)
        {
            OnRewardCollected?.Invoke(rewardPerIcon * (index + 1));
        }

        private void ClearAllIcons()
        {
            foreach (var rewardIcon in _rewardIcons)
            {
                if (rewardIcon != null)
                {
                    Destroy(rewardIcon.gameObject);
                }
            }
            _rewardIcons.Clear();
        }
    }
}