using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace BbxCommon.Framework
{
    #region SystemGroup
    public class UpdateSystemGroup : ComponentSystemGroup { }
    #endregion

    public abstract class GameEngineBase<TEngine> : MonoSingleton<TEngine> where TEngine : GameEngineBase<TEngine>
    {
        #region Wrappers
        public EngineEcsWrapper EcsWrapper;
        public EngineStageWrapper StageWrapper;

        private void InitWrapper()
        {
            EcsWrapper = new EngineEcsWrapper(this);
            StageWrapper = new EngineStageWrapper(this);
        }

        public struct EngineEcsWrapper
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineEcsWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public T AddSingletonRawComponent<T>() where T : EcsSingletonRawComponent, new() => m_Ref.AddSingletonRawComponent<T>();
            public T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent => m_Ref.GetSingletonRawComponent<T>();
        }

        public struct EngineStageWrapper
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineStageWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public GameStage CreateStage(string stageName) => m_Ref.CreateStage(stageName);
        }
        #endregion

        #region UnityCallbacks
        protected sealed override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this);
            
            InitWrapper();

            OnAwakeWorld();

            OnAwake();
        }

        protected virtual void OnAwake() { }

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

        #region Stage
        public GameStage CreateStage(string stageName)
        {
            return new GameStage(stageName, m_EcsWorld);
        }
        #endregion
    }
}
