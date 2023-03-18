using System.Collections.Generic;

namespace BbxCommon.Framework
{
    public interface IEngineLoad
    {
        void Load();
        void Unload();
    }

    public interface IEngineUpdate
    {
        void OnCreate();
        void OnUpdate();
        void OnDestroy();
    }

    public abstract class GameEngineBase<TEngine> : MonoSingleton<TEngine> where TEngine : GameEngineBase<TEngine>
    {
        #region Wrappers
        public EngineLoadingWrapper LoadWrapper;
        public EngineTickingWrapper TickWrapper;

        private void InitWrapper()
        {
            LoadWrapper = new EngineLoadingWrapper(this);
            TickWrapper = new EngineTickingWrapper(this);
        }

        public struct EngineLoadingWrapper
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineLoadingWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public void AddGlobalLoadItem(IEngineLoad item) => m_Ref.AddGlobalLoadItem(item);
        }

        public struct EngineTickingWrapper
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineTickingWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public void AddGlobalUpdateItem(IEngineUpdate item) => m_Ref.AddGlobalUpdateItem(item);
            public void AddGlobalUpdateItem<T>() where T : IEngineUpdate, new() => m_Ref.AddGlobalUpdateItem<T>();
            public void AddGlobalFixedUpdateItem(IEngineUpdate item) => m_Ref.AddGlobalFixedUpdateItem(item);
            public void AddGlobalFixedUpdateItem<T>() where T : IEngineUpdate, new() => m_Ref.AddGlobalFixedUpdateItem<T>();
        }
        #endregion

        #region UnityCallbacks
        private void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this);
            
            InitWrapper();

            OnAwakeEngineLoad();
            OnAwakeEngineTick();
        }

        private void Start()
        {
            OnStartEngineTick();
        }

        private void Update()
        {
            OnUpdateEngineTick();
        }

        private void FixedUpdate()
        {
            OnFixedUpdateEngineTick();
        }

        private void OnDestroy()
        {
            OnDestroyEngineLoad();
            OnDestroyEngineTick();
        }
        #endregion

        #region EngineLoad
        private List<IEngineLoad> m_GlobalLoadItems = new List<IEngineLoad>();

        /// <summary>
        /// <para>
        /// Override and implement this function to set loading items those should be loaded when game starts.
        /// </para><para>
        /// Consider adding items via the function <see cref="AddGlobalLoadItem(IEngineLoad)"/>.
        /// </para><para>
        /// For more functions about loading items, look into <see cref="EngineLoadingWrapper"/>.
        /// </para>
        /// </summary>
        protected abstract void SetGlobalLoadItems();

        public void AddGlobalLoadItem(IEngineLoad item)
        {
            m_GlobalLoadItems.Add(item);
        }

        private void OnAwakeEngineLoad()
        {
            SetGlobalLoadItems();
            foreach (var item in m_GlobalLoadItems)
            {
                item.Load();
            }
        }

        private void OnDestroyEngineLoad()
        {
            foreach (var item in m_GlobalLoadItems)
            {
                item.Unload();
            }
        }
        #endregion

        #region EngineTick
        private List<IEngineUpdate> m_GlobalUpdateItems = new List<IEngineUpdate>();
        private List<IEngineUpdate> m_GlobalFixedUpdateItems = new List<IEngineUpdate>();

        /// <summary>
        /// <para>
        /// Override and implement this function to set tickable items which run throughout the whole game.
        /// </para><para>
        /// For different call timings, use <see cref="AddGlobalUpdateItem(IEngineUpdate)"/> or <see cref="AddGlobalFixedUpdateItem(IEngineUpdate)"/>.
        /// </para><para>
        /// Recommends implementing tickable items by inheriting <see cref="EcsSystemBase"/> and its derived classes.
        /// </para><para>
        /// For more functions about ticking items, look into <see cref="EngineTickingWrapper"/>.
        /// </para>
        /// </summary>
        protected abstract void SetGlobalTickItems();

        public void AddGlobalUpdateItem(IEngineUpdate item)
        {
            m_GlobalUpdateItems.Add(item);
        }

        public void AddGlobalUpdateItem<T>() where T : IEngineUpdate, new()
        {
            AddGlobalUpdateItem(new T());
        }

        public void AddGlobalFixedUpdateItem(IEngineUpdate item)
        {
            m_GlobalFixedUpdateItems.Add(item);
        }

        public void AddGlobalFixedUpdateItem<T>() where T :IEngineUpdate, new()
        {
            AddGlobalFixedUpdateItem(new T());
        }

        private void OnAwakeEngineTick()
        {
            SetGlobalTickItems();
        }

        private void OnStartEngineTick()
        {
            foreach (var item in m_GlobalUpdateItems)
            {
                item.OnCreate();
            }
            foreach (var item in m_GlobalFixedUpdateItems)
            {
                item.OnCreate();
            }
        }

        private void OnUpdateEngineTick()
        {
            foreach (var item in m_GlobalUpdateItems)
            {
                item.OnUpdate();
            }
        }

        private void OnFixedUpdateEngineTick()
        {
            foreach (var item in m_GlobalFixedUpdateItems)
            {
                item.OnUpdate();
            }
        }

        private void OnDestroyEngineTick()
        {
            foreach (var item in m_GlobalUpdateItems)
            {
                item.OnDestroy();
            }
            foreach (var item in m_GlobalFixedUpdateItems)
            {
                item.OnDestroy();
            }
        }
        #endregion
    }
}
