using UnityEngine;
using NaughtyAttributes;
using Cysharp.Threading.Tasks;

namespace BattleTurn.UIManagements.Runtime
{
    public abstract class UIViewer : MonoBehaviour
    {
        public delegate void OnShow();
        public delegate UniTask OnHide();

        public event OnShow OnShownEvent;
        public event OnHide OnHiddenEvent;
        public event OnShow OnShowEvent;
        public event OnHide OnHideEvent;

        [SerializeField, ReadOnly] protected UIManager uiManager;
        [SerializeField, ReadOnly] protected bool isShowing;
        [SerializeField, ReadOnly] protected int instanceID;
        [SerializeField] protected bool destroyOnHide = false;

        public bool IsShowing => isShowing;

        protected virtual void Awake()
        {
            instanceID = GetInstanceID();
        }

        protected abstract void ShowSelf();
        protected abstract void HideSelf();

        internal virtual void OnInitialize(UIManager uiManager)
        {
            this.uiManager = uiManager;
        }

        internal void ShowInternal()
        {
            Show();
        }

        internal void HideInternal()
        {
            Hide().Forget();
        }

        [Button("Show")]
        private void ShowPrivate()
        {
            ShowSelf();
        }

        [Button("Hide")]
        private void HidePrivate()
        {
            HideSelf();
        }

        protected virtual void Show()
        {
            if (isShowing)
            {
                return;
            }

            isShowing = true;
            OnShowEvent?.Invoke();
            gameObject.SetActive(true);
            OnShownEvent?.Invoke();
        }

        protected async virtual UniTaskVoid Hide()
        {
            if (!isShowing)
            {
                return;
            }

            isShowing = false;

            await InvokeOnHide();
            MakeUIDisappear();
        }

        protected void MakeUIDisappear()
        {
            if (destroyOnHide)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        protected async UniTask InvokeOnHide()
        {
            if (OnHideEvent != null)
            {
                System.Delegate[] invocationList = OnHideEvent.GetInvocationList();
                UniTask[] tasks = new UniTask[invocationList.Length];
                for (int i = 0; i < invocationList.Length; i++)
                {
                    System.Delegate handler = invocationList[i];
                    tasks[i] = ((OnHide)handler).Invoke();
                }

                await UniTask.WhenAll(tasks);
            }
            OnHiddenEvent?.Invoke();
        }

        protected void InvokeOnShow()
        {
            OnShowEvent?.Invoke();
        }
    }
}