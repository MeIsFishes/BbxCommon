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

        #region Init and Open
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
            var uiItems = SimplePool<List<IBbxUiItem>>.Alloc();
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

        #region ChildController
        // TODO: Maybe call OnUiOpen, OnUiClose and other functions follow the parent's state.(?)
        private UiControllerBase m_Parent;
        public UiControllerBase ParentController => m_Parent;
        private List<UiControllerBase> m_ChildControllers = new List<UiControllerBase>();

        protected T CreateChildController<T>(GameObject uiGameObject) where T : UiControllerBase
        {
            var controller = UiSceneBase.CreateUiController<T>(uiGameObject);
            controller.m_Parent = this;
            m_ChildControllers.Add(controller);
            return controller;
        }

        protected void DestroyChildController(UiControllerBase controller)
        {
            controller.Close();
            m_ChildControllers.Remove(controller);
            Destroy(controller.gameObject);
        }
        #endregion
    }
}
