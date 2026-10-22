using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    public sealed class Tab : Toggle
    {
        public TabController TabController { get; set; }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            // Reject turning off the last selected toggle.
            if (isOn && TabController != null)
                return;

            base.OnPointerClick(eventData);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            // Apply the same rule for keyboard/controller submit.
            if (isOn && TabController != null)
                return;

            base.OnSubmit(eventData);
        }
    }
}