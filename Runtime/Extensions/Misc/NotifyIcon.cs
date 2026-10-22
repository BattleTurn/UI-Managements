using UnityEngine;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class NotifyIcon : MonoBehaviour
    {
        [SerializeField] private Animation _animation;

        public void ShowNotifyIcon()
        {
            gameObject.SetActive(true);
            _animation?.Play();
        }

        public void HideNotifyIcon()
        {
            gameObject.SetActive(false);
            _animation?.Stop();
        }
    }
}