using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;

namespace BattleTurn.UIManagements.Runtime.Extensions
{
    /// <summary>
    /// An enumeration of selected states of objects
    /// </summary>
    public enum SelectionState
    {
        /// <summary>
        /// The UI object can be selected.
        /// </summary>
        Normal,

        /// <summary>
        /// The UI object is highlighted.
        /// </summary>
        Highlighted,

        /// <summary>
        /// The UI object is pressed.
        /// </summary>
        Pressed,

        /// <summary>
        /// The UI object is selected
        /// </summary>
        Selected,

        /// <summary>
        /// The UI object cannot be selected.
        /// </summary>
        Disabled,
    }

    /// <summary>
    /// A custom button that allows multiple Graphic components to be tinted simultaneously.
    /// </summary>
    [RequireComponent(typeof(Selectable))]
    public class MultiGraphicSelectable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler,
            IPointerEnterHandler, IPointerExitHandler,
            ISelectHandler, IDeselectHandler,
            ISubmitHandler
    {
        [SerializeField] private Graphic[] _excludedGraphics;
        [SerializeField, ReadOnly] private bool _isPointerInside;
        [SerializeField, ReadOnly] private bool _isPointerDown;
        [SerializeField, ReadOnly] private bool _hasSelection;

        private Graphic[] _childGraphics;
        private HashSet<Graphic> _excludedGraphicsSet;
        private Selectable _selectable;

        private bool IsPointerInside { get => _isPointerInside; }
        private bool IsPointerDown { get => _isPointerDown; }
        private bool HasSelection { get => _hasSelection; }

        private void Awake()
        {
            _selectable = GetComponent<Selectable>();

            // Gather all Graphic components (Image, TextMeshProUGUI, etc.) from children
            _childGraphics = GetComponentsInChildren<Graphic>(true);
            _excludedGraphicsSet = new HashSet<Graphic>(_excludedGraphics);
        }

        // Overrides the state transition to tint all gathered graphics
        protected void DoStateTransition(SelectionState state, bool instant)
        {
            // Get the target color based on current button state
            Color targetColor = state switch
            {
                SelectionState.Normal => _selectable.colors.normalColor,
                SelectionState.Highlighted => _selectable.colors.highlightedColor,
                SelectionState.Pressed => _selectable.colors.pressedColor,
                SelectionState.Selected => _selectable.colors.selectedColor,
                SelectionState.Disabled => _selectable.colors.disabledColor,
                _ => _selectable.colors.normalColor
            };

            if (_childGraphics == null) return;

            // Apply color to all child graphics smoothly using the Button's Fade Duration
            foreach (var graphic in _childGraphics)
            {
                if (graphic == null || _excludedGraphicsSet.Contains(graphic)) continue;
                graphic.CrossFadeColor(targetColor, instant ? 0f : _selectable.colors.fadeDuration, true, true);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPointerInside = true;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isPointerInside = false;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPointerDown = true;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPointerDown = false;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnSelect(BaseEventData eventData)
        {
            _hasSelection = true;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _hasSelection = false;
            EvaluateAndTransitionToSelectionState();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            OnFinishSubmit().Forget();
        }

        protected bool IsInteractable()
        {
            if (_selectable == null) _selectable = GetComponent<Button>();

            return _selectable.interactable;
        }

        protected SelectionState currentSelectionState
        {
            get
            {
                if (!IsInteractable())
                    return SelectionState.Disabled;
                if (IsPointerDown)
                    return SelectionState.Pressed;
                if (HasSelection)
                    return SelectionState.Selected;
                if (IsPointerInside)
                    return SelectionState.Highlighted;
                return SelectionState.Normal;
            }
        }

        /// <summary>
        /// Returns true if the GameObject and the Component are active.
        /// </summary>
        protected bool IsActive()
        {
            return isActiveAndEnabled;
        }

        private void EvaluateAndTransitionToSelectionState()
        {
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(currentSelectionState, false);
        }

        private async UniTaskVoid OnFinishSubmit()
        {
            var fadeTime = _selectable.colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                await UniTask.WaitForEndOfFrame();
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}