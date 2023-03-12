using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon.Ui
{
    public abstract class UiControllerBase<TView> : UiControllerBase where TView : UiViewBase
    {
        protected TView m_View;

        public override void SetView(UiViewBase view)
        {
            m_View = view as TView;
        }
    }

    public abstract class UiControllerBase : MonoBehaviour
    {
        #region Common
        public abstract void SetView(UiViewBase view);
        #endregion

        #region InitAndOpen
        private void OnEnable()
        {
            OnUiOpen();
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Init()
        {
            var uiItems = SimplePool<List<IExtendUiItem>>.Alloc();
            GetComponentsInChildren(uiItems);
            foreach (var item in uiItems)
            {
                item.Init(this);
            }
            uiItems.CollectToPool();

            OnUiInit();
        }

        /// <summary>
        /// Calls on first opens before call OnUiOpen().
        /// </summary>
        protected virtual void OnUiInit() { }
        /// <summary>
        /// Calls when the GameObject is enabled.
        /// </summary>
        protected virtual void OnUiOpen() { }
        #endregion

        #region Close
        private void OnDisable()
        {
            OnUiClose();
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// Calls when the GameObject is disabled.
        /// </summary>
        protected virtual void OnUiClose() { }
        #endregion
    }
}
