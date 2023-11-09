using Codice.CM.SEIDInfo;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace BbxCommon.Ui
{
    #region ControllerTypeId
    internal interface IUiControllerTypeId
    {
        internal int GetControllerTypeId();
    }
    #endregion

    public abstract class UiControllerBase<TView> : UiControllerBase, IUiControllerTypeId where TView : UiViewBase
    {
        #region Wrapper
        public ControllerWpData ControllerWrapper;
        public struct ControllerWpData
        {
            private UiControllerBase<TView> m_Ref;
            public ControllerWpData(UiControllerBase<TView> obj) { m_Ref = obj; }
            public void Show() => m_Ref.Show();
            public void Hide() => m_Ref.Hide();
            public void Close() => m_Ref.Close();
            public void Destroy() => m_Ref.Destroy();
            public int GetControllerTypeId() => m_Ref.GetControllerTypeId();
            /// <summary>
            /// Enable an item like <see cref="UiDragable"/>, <see cref="UiList"/>.
            /// </summary>
            public void EnableUiItem(Component item) => m_Ref.EnableUiItem(item);
            /// <summary>
            /// Disable an item like <see cref="UiDragable"/>, <see cref="UiList"/>.
            /// </summary>
            public void DisableUiItem(Component item) => m_Ref.DisableUiItem(item);
        }

        /// <summary>
        /// Functions interacting with <see cref="IUiModelItem"/>s.
        /// </summary>
        protected ModelWpData ModelWrapper;
        protected struct ModelWpData
        {
            private UiControllerBase<TView> m_Ref;
            public ModelWpData(UiControllerBase<TView> obj) { m_Ref = obj; }
            public ModelItemListenerInfo AddUiModelVariableListener(EControllerLifeCycle enableDuring, IUiModelItem modelItem, EUiModelVariableEvent listeningEvent, UnityAction<MessageDataBase> callback)
                => m_Ref.AddUiModelVariableListener(enableDuring, modelItem, listeningEvent, callback);
            public ModelItemListenerInfo AddUiModelListener(EControllerLifeCycle enableDuring, IUiModelItem modelItem, int listeningEvent, UnityAction<MessageDataBase> callback)
                => m_Ref.AddUiModelListener(enableDuring, modelItem, listeningEvent, callback);
        }
        #endregion

        #region Common
        protected TView m_View;

        internal override UiViewBase View { get { return m_View; } set { m_View = (TView)value; } }
        internal override Type GenericType => typeof(UiControllerBase<TView>);

        public override void SetView(UiViewBase view)
        {
            m_View = view as TView;
        }
        #endregion

        #region Lifecycle
        protected override sealed void Update()
        {
            OnUiUpdate(UiTimer.DeltaTime);
            foreach (var uiItem in m_View.UiUpdates)
            {
                if (uiItem is IUiUpdate uiUpdate)
                    uiUpdate.OnUiUpdate(this, UiTimer.DeltaTime);
            }
        }

        protected virtual void OnUiUpdate(float deltaTime) { }

        // 1. The full lifecycle of a UI item is: Init() -> Open() -> Show() -> Hide() -> Close() -> Destroy().
        // 2. You can consider them as 3 sets of opposing stages: Init() with Destroy(), Open() with Close(), and Show() with Hide().
        // 3. Show() and Hide() can be skipped.
        // 4. By default, UI items will be collected to the pool, which means they call Close() instead of Destroy(), and they call Init()
        //    only once when being created, not every time getting from the pool.
        // 5. When calling Close() or Destroy(), previous functions will be called. For example, it will call Hide() if it is shown, and
        //    call Close() if it is opened.

        protected bool m_Inited { get; private set; }
        protected bool m_Opened { get; private set; }
        protected bool m_Shown { get; private set; }

        internal override sealed void Init()
        {
            ControllerWrapper = new ControllerWpData(this);
            ModelWrapper = new ModelWpData(this);
            if (m_Inited == false)
            {
                m_InitListeners = SimplePool<List<ModelItemListenerInfo>>.Alloc();
                m_OpenListeners = SimplePool<List<ModelItemListenerInfo>>.Alloc();
                m_ShowListeners = SimplePool<List<ModelItemListenerInfo>>.Alloc();

                InitUiModelListeners();

                foreach (var listener in m_InitListeners)
                {
                    listener.AddListener();
                }

                OnUiInit();
                foreach (var uiItem in m_View.UiInits)
                {
                    if (uiItem is IUiInit uiInit)
                        uiInit.OnUiInit(this);
                }
                m_Inited = true;
            }
        }

        internal override sealed void Open()
        {
            if (m_Opened == false)
            {
                foreach (var listener in m_OpenListeners)
                {
                    listener.AddListener();
                }

                OnUiOpen();
                foreach (var uiItem in m_View.UiOpens)
                {
                    if (uiItem is IUiOpen uiOpen)
                        uiOpen.OnUiOpen(this);
                }
                m_Opened = true;
                UiControllerManager.OnUiOpen(this);
            }
        }

        public override sealed void Show()
        {
            if (m_Shown == false)
            {
                foreach (var listener in m_ShowListeners)
                {
                    listener.AddListener();
                }

                m_View.gameObject.SetActive(true);
                OnUiShow();
                foreach (var uiItem in m_View.UiShows)
                {
                    if (uiItem is IUiShow uiShow)
                        uiShow.OnUiShow(this);
                }
                m_Shown = true;
            }
        }

        public override sealed void Hide()
        {
            if (m_Shown)
            {
                foreach (var listenerInfo in m_ShowListeners)
                {
                    listenerInfo.TryRemoveListener();
                }

                m_View.gameObject.SetActive(false);
                OnUiHide();
                foreach (var uiItem in m_View.UiHides)
                {
                    if (uiItem is IUiHide uiHide)
                        uiHide.OnUiHide(this);
                }
                m_Shown = false;
            }
        }

        public override sealed void Close()
        {
            if (m_Opened)
            {
                foreach (var listenerInfo in m_ShowListeners)
                {
                    listenerInfo.TryRemoveListener();
                }

                Hide();
                OnUiClose();
                foreach (var uiItem in m_View.UiCloses)
                {
                    if (uiItem is IUiClose uiClose)
                        uiClose.OnUiClose(this);
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
            foreach (var listenerInfo in m_InitListeners)
            {
                listenerInfo.TryRemoveListener();
            }

            if (m_Shown)
                OnUiHide();
            if (m_Opened)
                OnUiClose();
            OnUiDestroy();
            foreach (var uiItem in m_View.UiDestroys)
            {
                if (uiItem is IUiDestroy uiDestroy)
                    uiDestroy.OnUiDestroy(this);
            }

            foreach (var listenerInfo in m_InitListeners)
            {
                listenerInfo.ReleaseInfo();
            }
            m_InitListeners.CollectToPool();
            foreach (var listenerInfo in m_OpenListeners)
            {
                listenerInfo.ReleaseInfo();
            }
            m_OpenListeners.CollectToPool();
            foreach (var listenerInfo in m_ShowListeners)
            {
                listenerInfo.ReleaseInfo();
            }
            m_ShowListeners.CollectToPool();
        }
        #endregion

        #region Model Listener
        protected struct ModelItemListenerInfo
        {
            /// <summary>
            /// Stores <see cref="IUiModelItem"/> in fact. As the model item may be collected during listening, and interfaces cannot be
            /// packed by <see cref="ObjRef{T}"/>, we make a explicit converting.
            /// </summary>
            public ObjRef<PooledObject> ModelItem;
            public int MessageKey;

            /// <summary>
            /// Adding a listener reference to <see cref="IMessageDispatcher{TMessageKey}"/> but not directly setting functions, to provide
            /// a GC-free delegate operation.
            /// </summary>
            private SimpleMessageListener<int> m_Listener;

            public ModelItemListenerInfo(IUiModelItem modelItem, int messageKey, UnityAction<MessageDataBase> callback)
            {
                ModelItem = ((PooledObject)modelItem).AsObjRef();
                MessageKey = messageKey;
                m_Listener = ObjectPool<SimpleMessageListener<int>>.Alloc();
                m_Listener.Callback += callback;
            }

            public void AddListener()
            {
                if (ModelItem.IsNull())
                {
                    Debug.LogError("The ModelItem is not set or has been collected.");
                    return;
                }
                ((IUiModelItem)ModelItem.Obj).MessageDispatcher.AddListener(MessageKey, m_Listener);
            }

            public void TryRemoveListener()
            {
                if (ModelItem.IsNotNull())
                    ((IUiModelItem)ModelItem.Obj).MessageDispatcher.RemoveListener(MessageKey, m_Listener);
            }

            public void RebindModelItem(IUiModelItem item)
            {
                TryRemoveListener();
                ModelItem = ((PooledObject)item).AsObjRef();
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

        protected ModelItemListenerInfo AddUiModelVariableListener(EControllerLifeCycle enableDuring, IUiModelItem modelItem, EUiModelVariableEvent listeningEvent, UnityAction<MessageDataBase> callback)
        {
            var info = new ModelItemListenerInfo(modelItem, (int)listeningEvent, callback);
            AddListenerIfConditionMeets(info, enableDuring);
            StoreUiModelListener(enableDuring, info);
            return info;
        }

        protected ModelItemListenerInfo AddUiModelListener(EControllerLifeCycle enableDuring, IUiModelItem modelItem, int listeningEvent, UnityAction<MessageDataBase> callback)
        {
            var info = new ModelItemListenerInfo(modelItem, listeningEvent, callback);
            AddListenerIfConditionMeets(info, enableDuring);
            StoreUiModelListener(enableDuring, info);
            return info;
        }

        protected virtual void InitUiModelListeners() { }

        private void AddListenerIfConditionMeets(ModelItemListenerInfo info, EControllerLifeCycle enableDuring)
        {
            switch (enableDuring)
            {
                case EControllerLifeCycle.Init:
                    if (m_Inited)
                        info.AddListener();
                    break;
                case EControllerLifeCycle.Open:
                    if (m_Opened)
                        info.AddListener();
                    break;
                case EControllerLifeCycle.Show:
                    if (m_Shown)
                        info.AddListener();
                    break;
            }
        }

        // Listeners below will be unload when the controller is uninited, closed or hidden automatically.
        private List<ModelItemListenerInfo> m_InitListeners;
        private List<ModelItemListenerInfo> m_OpenListeners;
        private List<ModelItemListenerInfo> m_ShowListeners;

        private void StoreUiModelListener(EControllerLifeCycle enableDuring, ModelItemListenerInfo info)
        {
            switch (enableDuring)
            {
                case EControllerLifeCycle.Init:
                    m_InitListeners.Add(info);
                    break;
                case EControllerLifeCycle.Open:
                    m_OpenListeners.Add(info);
                    break;
                case EControllerLifeCycle.Show:
                    m_ShowListeners.Add(info);
                    break;
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
                var type = typeof(ClassTypeId<,>).MakeGenericType(typeof(UiControllerBase), this.GetType());
                var method = type.GetMethod("GetId", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
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

        #region BbxUiItem
        /// <summary>
        /// Enable an item like <see cref="UiDragable"/>, <see cref="UiList"/>.
        /// </summary>
        public void EnableUiItem(Component item)
        {
            if (m_Inited && m_View.UiInits.Contains(item))
            {
                if (m_View.UiDestroys.Contains(item))
                {
                    (item as IUiDestroy).OnUiDestroy(this);
                }
            }
            if (m_Opened && m_View.UiOpens.Contains(item))
            {
                if (m_View.UiCloses.Contains(item))
                {
                    (item as IUiClose).OnUiClose(this);
                }
            }
            if (m_Shown && m_View.UiShows.Contains(item))
            {
                if (m_View.UiHides.Contains(item))
                {
                    (item as IUiHide).OnUiHide(this);
                }
            }
            m_View.UiInits.Remove(item);
            m_View.UiOpens.Remove(item);
            m_View.UiShows.Remove(item);
            m_View.UiHides.Remove(item);
            m_View.UiCloses.Remove(item);
            m_View.UiDestroys.Remove(item);
        }

        /// <summary>
        /// Disable an item like <see cref="UiDragable"/>, <see cref="UiList"/>.
        /// </summary>
        public void DisableUiItem(Component item)
        {
            if (m_Inited && item is IUiInit uiInit)
                uiInit.OnUiInit(this);
            if (m_Opened && item is IUiOpen uiOpen)
                uiOpen.OnUiOpen(this);
            if (m_Shown && item is IUiShow uiShow)
                uiShow.OnUiShow(this);
            if (item is IUiInit)
                m_View.UiInits.TryAdd(item);
            if (item is IUiOpen)
                m_View.UiOpens.TryAdd(item);
            if (item is IUiShow)
                m_View.UiShows.TryAdd(item);
            if (item is IUiHide)
                m_View.UiHides.TryAdd(item);
            if (item is IUiClose)
                m_View.UiCloses.TryAdd(item);
            if (item is IUiDestroy)
                m_View.UiDestroys.TryAdd(item);
        }
        #endregion
    }

    public abstract class UiControllerBase : MonoBehaviour
    {
        #region Common
        internal virtual UiViewBase View { get; set; }
        internal virtual Type GenericType { get; }

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
    }
}
