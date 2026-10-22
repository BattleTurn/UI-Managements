using DG.Tweening;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public static class DoTweenExtension
    {
        public static void Deactivate(this Tween tween)
        {
            if (tween != null && tween.IsActive())
            {
                tween.Kill();
            }
        }

        public static void Deactivate(this Sequence sequence)
        {
            if (sequence != null && sequence.IsActive())
            {
                sequence.Kill();
            }
        }
    }
}