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
        private bool m_Opened;

        public void Open()
        {
            if (m_Opened)
                return;
            gameObject.SetActive(true);
            OnUiOpen();
            m_Opened = true;
        }

        public void Init()
        {
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

        #region Close and Destroy
        private void OnDestroy()
        {
            OnUiDestroy();
        }

        public void Close()
        {
            if (m_Opened == false)
                return;
            gameObject.SetActive(false);
            OnUiClose();
            m_Opened = false;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Calls when the GameObject is disabled.
        /// </summary>
        protected virtual void OnUiClose() { }

        protected virtual void OnUiDestroy() { }
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
