using System.Collections.Generic;
using UnityEngine;
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
        public EngineGlobalStageWrapper GlobalStageWrapper;

        private void InitWrapper()
        {
            EcsWrapper = new EngineEcsWrapper(this);
            GlobalStageWrapper = new EngineGlobalStageWrapper(this);
        }

        public struct EngineEcsWrapper
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineEcsWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public T AddSingletonRawComponent<T>() where T : EcsSingletonRawComponent, new() => m_Ref.AddSingletonRawComponent<T>();
            public T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent => m_Ref.GetSingletonRawComponent<T>();
        }

        public struct EngineGlobalStageWrapper
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineGlobalStageWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public void AddGlobalLoadItem(IEngineLoad item) => m_Ref.m_GlobalStage.AddLoadItem(item);
            public void AddGlobalUpdateSystem<T>() where T : EcsHpSystemBase, new() => m_Ref.m_GlobalStage.AddUpdateSystem<T>();
            public void AddGlobalFixedUpdateSystem<T>() where T : EcsHpSystemBase, new() => m_Ref.m_GlobalStage.AddFixedUpdateSystem<T>();
            public void AddGlobalScene(params string[] scenes) => m_Ref.m_GlobalStage.AddScene(scenes);
        }
        #endregion

        #region UnityCallbacks
        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this);
            
            InitWrapper();

            OnAwakeWorld();
            OnAwakeGlobalStage();
        }

        private void OnDestroy()
        {

        }
        #endregion

        #region UiScene
        public GameObject UiCanvasProto;
        #endregion

        #region EcsWorld
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

        #region GloblaStage
        protected GameStage m_GlobalStage;

        /// <summary>
        /// <para>
        /// Override and implement this function to set loading items those should be loaded when game starts.
        /// </para><para>
        /// Consider adding items via the function <see cref="EngineGlobalStageWrapper.AddGlobalLoadItem(IEngineLoad)"/>.
        /// </para>
        /// </summary>
        protected abstract void SetGlobalLoadItems();

        /// <summary>
        /// <para>
        /// Override and implement this function to set tickable items which run throughout the whole game.
        /// </para><para>
        /// For different call timings, use <see cref="EngineGlobalStageWrapper.AddGlobalUpdateSystem()"/> or <see cref="EngineGlobalStageWrapper.AddGlobalFixedUpdateSystem()"/>.
        /// </para><para>
        /// Recommends implementing tickable items by inheriting <see cref="EcsHpSystemBase"/> and its derived classes.
        /// </para>
        /// </summary>
        protected abstract void SetGlobalTickItems();

        /// <summary>
        /// It's strongly recommended to put <see cref="GameEngineBase{TEngine}"/> in a separate scene, that ensures the <see cref="GameEngineBase{TEngine}"/> initializes before all other objects.
        /// </summary>
        protected abstract string GetGameMainScene();

        /// <summary>
        /// <para>
        /// Override and implement this function to add global game scene.
        /// </para><para>
        /// Consider adding scenes via the function <see cref="EngineGlobalStageWrapper.AddGlobalScene(string[])"/>.
        /// </para>
        /// </summary>
        protected virtual void SetGlobalScenes() { }

        private void OnAwakeGlobalStage()
        {
            m_GlobalStage = new GameStage("GlobalStage", m_EcsWorld);
            SetGlobalLoadItems();
            SetGlobalTickItems();
            m_GlobalStage.AddScene(GetGameMainScene());
            SetGlobalScenes();
            m_GlobalStage.LoadStage();
        }
        #endregion
    }
}
