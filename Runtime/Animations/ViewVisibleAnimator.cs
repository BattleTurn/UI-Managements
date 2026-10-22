using UnityEngine;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;
using System;

namespace BattleTurn.UIManagements.Runtime
{
    [RequireComponent(typeof(UIViewer))]
    public class ViewVisibleAnimator : MonoBehaviour
    {
        public event Action OnShowComplete;
        public event Action OnHideComplete;

        [SerializeField, ReadOnly] protected UIViewer viewer;

        private void OnValidate()
        {
            if (viewer == null)
            {
                viewer = GetComponent<UIViewer>();
            }
        }

        protected virtual void Awake()
        {
            if (viewer == null)
            {
                viewer = GetComponent<UIViewer>();
            }

            viewer.OnShowEvent += HandleShow;
            viewer.OnHideEvent += HandleHide;
        }

        protected virtual void HandleShow()
        {
            OnShowComplete?.Invoke();
        }

        protected async virtual UniTask HandleHide()
        {
            OnHideComplete?.Invoke();
            await UniTask.Yield();
        }

        protected virtual void InvokeOnShowComplete()
        {
            OnShowComplete?.Invoke();
        }

        protected virtual void InvokeOnHideComplete()
        {
            OnHideComplete?.Invoke();
        }
    }
}