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

    internal interface IUiControllerTypeId
    {
        internal int GetControllerTypeId();
    }
    #endregion

    public abstract class UiControllerBase<TView> : UiControllerBase, IUiControllerTypeId where TView : UiViewBase
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

        public override int GetControllerTypeId()
        {
            if (m_ControllerTypeIdInited)
                return m_ControllerTypeId;
            else
            {
                // register type id via reflection
                var type = typeof(UiControllerTypeId<>).MakeGenericType(this.GetType());
                var method = type.GetMethod("GetId", BindingFlags.Static | BindingFlags.NonPublic);
                SetControllerTypeId((int)method.Invoke(null, null));
                return m_ControllerTypeId;
            }
        }

        int IUiControllerTypeId.GetControllerTypeId()
        {
            return GetControllerTypeId();
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

        public abstract int GetControllerTypeId();
        #endregion

        #region Lifecycle
        private void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate() { }

        // 1. The full lifecycle of a UI item is: Init() -> Open() -> Show() -> Hide() -> Close() -> Destroy().
        // 2. You can consider them as 3 sets of opposing stages: Init() with Destroy(), Open() with Close(), Show() and Hide().
        // 3. Show() and Hide() can be skipped.
        // 4. By default, UI items will be collected to the pool, which means they call Close() instead of Destroy(), and they call Init()
        //    only once when being created, not every time getting from the pool.
        // 5. When calling Close() or Destroy(), previous functions will be called. For example, it will call Hide() if it is shown, and
        //    call Close() if it is opened.

        private bool m_Inited = false;
        private bool m_Opened = false;
        private bool m_Visible = false;

        internal void Init()
        {
            if (m_Inited == false)
            {
                OnUiInit();
                m_Inited = true;
            }
        }

        internal void Open()
        {
            if (m_Opened == false)
            {
                OnUiOpen();
                m_Opened = true;
            }
        }

        public void Show()
        {
            if (m_Visible == false)
            {
                gameObject.SetActive(true);
                OnUiShow();
                m_Visible = true;
            }
        }

        public void Hide()
        {
            if (m_Visible)
            {
                gameObject.SetActive(false);
                OnUiHide();
                m_Visible = false;
            }
        }

        public void Close()
        {
            if (m_Opened)
            {
                gameObject.SetActive(false);
                Hide();
                OnUiClose();
                UiControllerManager.CollectUiController(this);
                m_Opened = false;
            }
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (m_Visible)
                OnUiHide();
            if (m_Opened)
                OnUiClose();
            OnUiDestroy();
        }

        /// <summary>
        /// Calls only once when the UI object is created, before <see cref="OnUiOpen"/>.
        /// Notice that <see cref="OnUiInit"/> will not be called when the object is got out from pool.
        /// </summary>
        protected virtual void OnUiInit() { }
        /// <summary>
        /// Calls when the UI object is got out from pool or created as new.
        /// Notice that the object's position data has not been set while <see cref="Open"/>, but been set while <see cref="Show"/>.
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
