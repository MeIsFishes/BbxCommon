using UnityEngine;
using System.Collections.Generic;

namespace BbxCommon.GameEngine
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

    public interface IEngineFixedUpdate
    {
        void OnCreate();
        void OnFixedUpdate();
        void OnDestroy();
    }

    public abstract class GameEngineBase<TEngine> : MonoSingleton<TEngine> where TEngine : GameEngineBase<TEngine>
    {
        #region Wrappers
        public EngineLoadingWrapper LoadingWrapper;
        public EngineTickingWrapper TickingWrapper;

        private void InitWrapper()
        {
            LoadingWrapper = new EngineLoadingWrapper(this);
            TickingWrapper = new EngineTickingWrapper(this);
        }

        public struct EngineLoadingWrapper
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineLoadingWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public void AddGlobalLoadingItem(IEngineLoad item) => m_Ref.AddGlobalLoadingItem(item);
        }

        public struct EngineTickingWrapper
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineTickingWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public void AddGlobalUpdateItem(IEngineUpdate item) => m_Ref.AddGlobalUpdateItem(item);
            public void AddGlobalFixedUpdateItem(IEngineFixedUpdate item) => m_Ref.AddGlobalFixedUpdateItem(item);
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
        private List<IEngineLoad> m_GlobalLoadingItems = new List<IEngineLoad>();

        /// <summary>
        /// <para>
        /// Override and implement this function to set loading items those should be loaded when game starts.
        /// </para><para>
        /// Consider adding items via the function <see cref="AddGlobalLoadingItem(IEngineLoad)"/>.
        /// </para><para>
        /// For more functions about loading items, look into <see cref="EngineLoadingWrapper"/>.
        /// </para>
        /// </summary>
        protected abstract void SetGlobalLoadingItems();

        public void AddGlobalLoadingItem(IEngineLoad item)
        {
            m_GlobalLoadingItems.Add(item);
        }

        private void OnAwakeEngineLoad()
        {
            SetGlobalLoadingItems();
            foreach (var item in m_GlobalLoadingItems)
            {
                item.Load();
            }
        }

        private void OnDestroyEngineLoad()
        {
            foreach (var item in m_GlobalLoadingItems)
            {
                item.Unload();
            }
        }
        #endregion

        #region EngineTick
        private List<IEngineUpdate> m_GlobalUpdateItems = new List<IEngineUpdate>();
        private List<IEngineFixedUpdate> m_GlobalFixedUpdateItems = new List<IEngineFixedUpdate>();

        /// <summary>
        /// <para>
        /// Override and implement this function to set tickable items which run throughout the whole game.
        /// </para><para>
        /// For different call timings, use <see cref="AddGlobalUpdateItem(IEngineUpdate)"/> or <see cref="AddGlobalFixedUpdateItem(IEngineFixedUpdate)"/>.
        /// </para><para>
        /// Recommends implementing tickable items by inheriting <see cref="EcsSystemBase"/> and its derived classes.
        /// </para><para>
        /// For more functions about ticking items, look into <see cref="EngineTickingWrapper"/>.
        /// </para>
        /// </summary>
        protected abstract void SetGlobalTickingItems();

        public void AddGlobalUpdateItem(IEngineUpdate item)
        {
            m_GlobalUpdateItems.Add(item);
        }

        public void AddGlobalFixedUpdateItem(IEngineFixedUpdate item)
        {
            m_GlobalFixedUpdateItems.Add(item);
        }

        private void OnAwakeEngineTick()
        {
            SetGlobalTickingItems();
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
                item.OnFixedUpdate();
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
