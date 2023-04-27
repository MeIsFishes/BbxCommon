using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BbxCommon.Ui
{
    #region ControllerTypeId
    internal static class ControllerTypeId
    {
        internal static int CurIndex;
    }

    internal static class ControllerTypeId<T> where T : UiControllerBase
    {
        private static bool Inited;
        private static int m_Id;
        internal static int Id
        {
            get
            {
                if (Inited)
                    return m_Id;
                else
                {
                    m_Id = ControllerTypeId.CurIndex++;
                    Inited = true;
                    return m_Id;
                }
            }
        }

        internal static int GetId()
        {
            return Id;
        }
    }
    #endregion

    public abstract class UiControllerBase<TView> : UiControllerBase where TView : UiViewBase
    {
        #region Common
        protected TView m_View;

        public override void SetView(UiViewBase view)
        {
            m_View = view as TView;
        }
        #endregion

        #region ControllerTypeId
        private static bool m_ControllerTypeIdInited;
        internal static int ControllerTypeId;

        internal int GetControllerTypeId()
        {
            if (m_ControllerTypeIdInited)
                return ControllerTypeId;
            else
            {
                // register type id via reflection
                var method = typeof(ControllerTypeId<>).MakeGenericType(this.GetType()).GetMethod("GetId", BindingFlags.Static);
                SetControllerTypeId((int)method.Invoke(null, null));
                return ControllerTypeId;
            }
        }

        internal void SetControllerTypeId(int id)
        {
            if (m_ControllerTypeIdInited == false)
            {
                ControllerTypeId = id;
                m_ControllerTypeIdInited = true;
            }
        }
        #endregion
    }

    public abstract class UiControllerBase : MonoBehaviour
    {
        #region Common
        public abstract void SetView(UiViewBase view);
        #endregion

        #region Init, Open, Show
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
        protected virtual void OnUiShow() { }
        #endregion

        #region Hide, Close, Destroy
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

        protected virtual void OnUiHide() { }
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
