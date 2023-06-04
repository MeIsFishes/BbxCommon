using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

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

        #region Lifecycle
        protected override sealed void Update()
        {
            OnUpdate();
            foreach (var uiItem in m_View.UiItems)
            {
                uiItem.OnUiUpdate(this);
            }
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

        internal override sealed void Init()
        {
            if (m_Inited == false)
            {
                OnUiInit();
                foreach (var uiItem in m_View.UiItems)
                {
                    uiItem.OnUiInit(this);
                }
                m_Inited = true;
            }
        }

        internal override sealed void Open()
        {
            if (m_Opened == false)
            {
                OnUiOpen();
                foreach (var uiItem in m_View.UiItems)
                {
                    uiItem.OnUiOpen(this);
                }
                m_Opened = true;
                UiControllerManager.OnUiOpen(this);
            }
        }

        public override sealed void Show()
        {
            if (m_Visible == false)
            {
                gameObject.SetActive(true);
                OnUiShow();
                foreach (var uiItem in m_View.UiItems)
                {
                    uiItem.OnUiShow(this);
                }
                m_Visible = true;
            }
        }

        public override sealed void Hide()
        {
            if (m_Visible)
            {
                gameObject.SetActive(false);
                OnUiHide();
                foreach (var uiItem in m_View.UiItems)
                {
                    uiItem.OnUiHide(this);
                }
                m_Visible = false;
            }
        }

        public override sealed void Close()
        {
            if (m_Opened)
            {
                gameObject.SetActive(false);
                Hide();
                OnUiClose();
                foreach (var uiItem in m_View.UiItems)
                {
                    uiItem.OnUiClose(this);
                }
                UiControllerManager.CollectUiController(this);
                m_Opened = false;
            }
        }

        public override sealed void Destroy()
        {
            Destroy(gameObject);
        }

        protected override sealed void OnDestroy()
        {
            if (m_Visible)
                OnUiHide();
            if (m_Opened)
                OnUiClose();
            OnUiDestroy();
            foreach (var uiItem in m_View.UiItems)
            {
                uiItem.OnUiDestroy(this);
            }
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
        protected enum EControllerLifeCycle
        {
            Init,
            Open,
            Show,
        }

        protected abstract void Update();
        protected abstract void OnDestroy();
        internal abstract void Init();
        internal abstract void Open();
        public abstract void Show();
        public abstract void Hide();
        public abstract void Close();
        public abstract void Destroy();


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

        #region ModelListener
        protected struct ModelItemListenerInfo
        {
            public ObjRef<UiModelItemBase> ModelItem;
            public int MessageKey;

            /// <summary>
            /// Adding a listener reference to <see cref="IMessageDispatcher{TMessageKey}"/> but not directly setting functions, to provide
            /// a GC-free delegate operation.
            /// </summary>
            private SimpleMessageListener<int> m_Listener;

            public ModelItemListenerInfo(UiModelItemBase modelItem, int messageKey, UnityAction callback)
            {
                ModelItem = modelItem.AsObjRef();
                MessageKey = messageKey;
                m_Listener = ObjectPool<SimpleMessageListener<int>>.Alloc();
                m_Listener.Callback += (MessageData messageData) => { callback.Invoke(); };
            }

            public void AddListener()
            {
                if (ModelItem.IsNull())
                {
                    Debug.LogError("The ModelItem is not set or has been collcted.");
                    return;
                }
                ModelItem.Obj.MessageDispatcher.AddListener(MessageKey, m_Listener);
            }

            public void TryRemoveListener()
            {
                if (ModelItem.IsNotNull())
                    ModelItem.Obj.MessageDispatcher.RemoveListener(MessageKey, m_Listener);
            }

            public void RebindModelItem(UiModelItemBase item)
            {
                TryRemoveListener();
                ModelItem = item.AsObjRef();
                AddListener();
            }

            /// <summary>
            /// In most cases, this function is unnecessary to call, unless the <see cref="UiControllerBase"/> need to be destroyed but not closed.
            /// </summary>
            public void ReleaseInfo()
            {
                m_Listener.CollectToPool();
            }
        }

        protected ModelItemListenerInfo AddUiModelListener(UiModelItemBase modelItem, EUiModelVariableEvent listeningEvent, UnityAction callback)
        {
            var info = new ModelItemListenerInfo();
            return info;
        }
        #endregion
    }
}
