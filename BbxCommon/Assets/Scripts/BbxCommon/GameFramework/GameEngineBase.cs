using System.Collections.Generic;
using Unity.Entities;

namespace BbxCommon.Framework
{
    #region SystemGroup
    public class UpdateSystemGroup : ComponentSystemGroup { }
    #endregion

    #region Interfaces
    public interface IEngineLoad
    {
        void Load();
        void Unload();
    }
    #endregion

    public abstract class GameEngineBase<TEngine> : MonoSingleton<TEngine> where TEngine : GameEngineBase<TEngine>
    {
        #region Wrappers
        public EngineEcsWrapper EcsWrapper;
        public EngineLoadingWrapper LoadWrapper;
        public EngineTickingWrapper TickWrapper;

        private void InitWrapper()
        {
            EcsWrapper = new EngineEcsWrapper(this);
            LoadWrapper = new EngineLoadingWrapper(this);
            TickWrapper = new EngineTickingWrapper(this);
        }

        public struct EngineEcsWrapper
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineEcsWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public T AddSingletonRawComponent<T>() where T : EcsSingletonRawComponent, new() => m_Ref.AddSingletonRawComponent<T>();
            public T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent => m_Ref.GetSingletonRawComponent<T>();
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

            public void AddGlobalUpdateItem<T>() where T : EcsHpSystemBase, new() => m_Ref.AddGlobalUpdateItem<T>();
            public void AddGlobalFixedUpdateItem<T>() where T : EcsHpSystemBase, new() => m_Ref.AddGlobalFixedUpdateItem<T>();
        }
        #endregion

        #region UnityCallbacks
        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this);
            
            InitWrapper();

            OnAwakeWorld();
            OnAwakeEngineLoad();
            OnAwakeEngineTick();
        }

        private void OnDestroy()
        {
            OnDestroyEngineLoad();
            OnDestroyEngineTick();
        }
        #endregion

        #region Create World
        private World m_EcsWorld;
        private Entity m_SingletonEntity;

        public T AddSingletonRawComponent<T>() where T : EcsSingletonRawComponent, new()
        {
            return m_SingletonEntity.AddRawComponent<T>();
        }

        public T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return m_SingletonEntity.GetRawComponent<T>();
        }

        protected abstract void InitSingletonComponents();

        private void OnAwakeWorld()
        {
            m_EcsWorld = World.DefaultGameObjectInjectionWorld;
            m_EcsWorld.CreateSystem<UpdateSystemGroup>();
            m_SingletonEntity = EcsApi.CreateEntity();
            InitSingletonComponents();
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
        /// <summary>
        /// <para>
        /// Override and implement this function to set tickable items which run throughout the whole game.
        /// </para><para>
        /// For different call timings, use <see cref="AddGlobalUpdateItem()"/> or <see cref="AddGlobalFixedUpdateItem()"/>.
        /// </para><para>
        /// Recommends implementing tickable items by inheriting <see cref="EcsHpSystemBase"/> and its derived classes.
        /// </para><para>
        /// For more functions about ticking items, look into <see cref="EngineTickingWrapper"/>.
        /// </para>
        /// </summary>
        protected abstract void SetGlobalTickItems();

        public void AddGlobalUpdateItem<T>() where T : EcsHpSystemBase, new()
        {
            var systemGroup = m_EcsWorld.GetExistingSystemManaged<UpdateSystemGroup>();
            var system = m_EcsWorld.CreateSystemManaged<T>();
            systemGroup.AddSystemToUpdateList(system);
        }

        public void AddGlobalFixedUpdateItem<T>() where T :EcsHpSystemBase, new()
        {
            m_EcsWorld.GetOrCreateSystemManaged<FixedStepSimulationSystemGroup>().AddSystemToUpdateList(World.DefaultGameObjectInjectionWorld.CreateSystemManaged<T>());
        }

        private void OnAwakeEngineTick()
        {
            SetGlobalTickItems();
        }


        private void OnDestroyEngineTick()
        {
            
        }
        #endregion
    }
}
