using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BattleTurns.UIManagements.Runtime
{
    public static class Utilities
    {
        private static readonly List<RaycastResult> Results = new();

        /// <summary>
        /// Returns true if the current pointer is over any UI element.
        /// Supports both mouse and mobile touch.
        /// </summary>
        public static bool IsPointerOverUI(string uiLayerName = "UI")
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return IsUIObject(uiLayerName);
            }
#else
            if (Input.touchCount == 0 || EventSystem.current == null)
                return false;

            if (EventSystem.current.IsPointerOverGameObject(
                Input.GetTouch(0).fingerId))
            {
                return IsUIObject(uiLayerName);
            }

#endif
            return false;
        }


        private static bool IsUIObject(string uiLayerName)
        {
            if (EventSystem.current == null)
                return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            Results.Clear();

            EventSystem.current.RaycastAll(eventData, Results);

            if (Results.Count == 0)
                return false;

            return Results[0].gameObject.layer == LayerMask.NameToLayer(uiLayerName);
        }
    }
}