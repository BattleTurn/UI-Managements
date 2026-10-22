using UnityEngine;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class AnimatorController : MonoBehaviour
    {
        [SerializeField] private BaseAnimator[] _additionalAnimators;
        
        public void Hide()
        {
            foreach (var animator in _additionalAnimators)
            {
                animator.Play(false);
            }
        }

        public void Show()
        {
            foreach (var animator in _additionalAnimators)
            {
                animator.Play(true);
            }
        }
    }
}