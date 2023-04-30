using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BbxCommon.Ui
{
    #region ControllerTypeId
    internal static class UiControllerTypeId
    {
        internal static int CurIndex;
    }

    internal static class UiControllerTypeId<T> where T : UiControllerBase
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
                    m_Id = UiControllerTypeId.CurIndex++;
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

    public interface IUiControllerTypeId
    {
        internal int GetControllerTypeId();
    }
    #endregion

    public abstract class UiControllerBase<TView> : UiControllerBase where TView : UiViewBase, IUiControllerTypeId
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
        internal static int m_ControllerTypeId;

        internal int GetControllerTypeId()
        {
            if (m_ControllerTypeIdInited)
                return m_ControllerTypeId;
            else
            {
                // register type id via reflection
                var method = typeof(UiControllerTypeId<>).MakeGenericType(this.GetType()).GetMethod("GetId", BindingFlags.Static);
                SetControllerTypeId((int)method.Invoke(null, null));
                return m_ControllerTypeId;
            }
        }

        private void SetControllerTypeId(int id)
        {
            if (m_ControllerTypeIdInited == false)
            {
                m_ControllerTypeId = id;
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

        #region Lifecycle
        internal enum EControllerState
        {
            None,
            Inited,
            Opened,
            Visible,
            Invisible,
            Closed,
        }

        internal bool ControllerInPool;

        private EControllerState m_State;

        public void Init()
        {
            switch (m_State)
            {
                case EControllerState.None:
                    OnUiInit();
                    m_State = EControllerState.Inited;
                    break;
                default:
                    Debug.LogWarning("The controller's current state is " + m_State + ". There should be a bug!");
                    break;
            }
        }

        public void Open()
        {
            switch (m_State)
            {
                case EControllerState.Inited:
                case EControllerState.Closed:
                    OnUiOpen();
                    m_State = EControllerState.Inited;
                    break;
                default:
                    Debug.LogWarning("The controller's current state is " + m_State + ". There should be a bug!");
                    break;
            }
        }

        public void Show()
        {
            switch (m_State)
            {
                case EControllerState.Opened:
                    gameObject.SetActive(true);
                    OnUiShow();
                    m_State = EControllerState.Visible;
                    break;
                case EControllerState.Visible:
                    break;
                default:
                    Debug.LogWarning("The controller's current state is " + m_State + ". There should be a bug!");
                    break;
            }
        }

        public void Hide()
        {
            switch (m_State)
            {
                case EControllerState.Visible:
                    gameObject.SetActive(false);
                    OnUiHide();
                    m_State = EControllerState.Invisible;
                    break;
                case EControllerState.Invisible:
                    break;
                default:
                    Debug.LogWarning("The controller's current state is " + m_State + ". There should be a bug!");
                    break;
            }
        }

        public void Close()
        {
            switch (m_State)
            {
                case EControllerState.Visible:
                    gameObject.SetActive(false);
                    OnUiHide();
                    OnUiClose();
                    m_State = EControllerState.Closed;
                    break;
                case EControllerState.Invisible:
                    OnUiClose();
                    m_State = EControllerState.Closed;
                    break;
                default:
                    Debug.LogWarning("The controller's current state is " + m_State + ". There should be a bug!");
                    break;
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            OnUiHide();
            OnUiClose();
            OnUiDestroy();
        }

        /// <summary>
        /// Calls only once when the UI object is created, before <see cref="OnUiOpen"/>.
        /// Notice that <see cref="OnUiInit"/> will not be called when the object is got out from pool.
        /// </summary>
        protected virtual void OnUiInit() { }
        /// <summary>
        /// Calls when the UI object is enabled.
        /// </summary>
        protected virtual void OnUiOpen() { }
        /// <summary>
        /// Calls when the UI object is set as visible.
        /// </summary>
        protected virtual void OnUiShow() { }

        /// <summary>
        /// Calls when the UI object is set as unvisible.
        /// </summary>
        protected virtual void OnUiHide() { }
        /// <summary>
        /// Calls when the UI object is close. The closed object will be collected to pool if you don't ask for destroying.
        /// </summary>
        protected virtual void OnUiClose() { }
        /// <summary>
        /// Calls when the UI object is destroyed. In most cases, the object will be collected to pool instead of being destroyed,
        /// unless you declare a destroying request.
        /// </summary>
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
