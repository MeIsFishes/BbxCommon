using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon.Ui;

namespace BbxCommon.Framework
{
    #region SystemGroup
    public class UpdateSystemGroup : ComponentSystemGroup { }
    #endregion

    #region EngineWrapper
    public struct EngineUiSceneWrapper<TEngine> where TEngine : GameEngineBase<TEngine>
    {
        private GameEngineBase<TEngine> m_Ref;

        public EngineUiSceneWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

        public T CreateUiScene<T>() where T : UiSceneBase => m_Ref.CreateUiScene<T>();
        public T GetUiScene<T>() where T : UiSceneBase => m_Ref.GetUiScene<T>();
        public T GetOrCreateUiScene<T>() where T : UiSceneBase => m_Ref.GetOrCreateUiScene<T>();
    }

    public struct EngineEcsWrapper<TEngine> where TEngine : GameEngineBase<TEngine>
    {
        private GameEngineBase<TEngine> m_Ref;

        public EngineEcsWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

        public T AddSingletonRawComponent<T>() where T : EcsSingletonRawComponent, new() => m_Ref.AddSingletonRawComponent<T>();
        public T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent => m_Ref.GetSingletonRawComponent<T>();
    }

    public struct EngineStageWrapper<TEngine> where TEngine : GameEngineBase<TEngine>
    {
        private GameEngineBase<TEngine> m_Ref;

        public EngineStageWrapper(GameEngineBase<TEngine> engine) { m_Ref = engine; }

        public GameStage CreateStage(string stageName) => m_Ref.CreateStage(stageName);
    }
    #endregion

    public abstract class GameEngineBase<TEngine> : MonoSingleton<TEngine> where TEngine : GameEngineBase<TEngine>
    {
        #region Wrappers
        public EngineUiSceneWrapper<TEngine> UiSceneWrapper;
        public EngineEcsWrapper<TEngine> EcsWrapper;
        public EngineStageWrapper<TEngine> StageWrapper;

        private void InitWrapper()
        {
            UiSceneWrapper = new EngineUiSceneWrapper<TEngine>(this);
            EcsWrapper = new EngineEcsWrapper<TEngine>(this);
            StageWrapper = new EngineStageWrapper<TEngine>(this);
        }
        #endregion

        #region UnityCallbacks
        protected sealed override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this);
            
            InitWrapper();

            OnAwakeWorld();
            OnAwakeUiScene();

            // call overridable OnAwake() after all datas are initialized
            OnAwake();
        }

        protected virtual void OnAwake() { }

        private void OnDestroy()
        {

        }
        #endregion

        #region UiScene
        public GameObject UiCanvasProto;

        private GameObject m_UiSceneRoot;
        private Dictionary<Type, UiSceneBase> m_UiScenes = new Dictionary<Type, UiSceneBase>();

        public T CreateUiScene<T>() where T : UiSceneBase
        {
            var type = typeof(T);
            var uiSceneName = type.Name;
            var uiSceneGameObject = new GameObject(uiSceneName);
            uiSceneGameObject.transform.SetParent(m_UiSceneRoot.transform);
            var uiScene = uiSceneGameObject.AddComponent<T>();
            uiScene.InitUiScene(UiCanvasProto);
            m_UiScenes.Add(type, uiScene);
            return uiScene;
        }

        public T GetUiScene<T>() where T : UiSceneBase
        {
            m_UiScenes.TryGetValue(typeof(T), out var uiScene);
            return (T)uiScene;
        }

        public T GetOrCreateUiScene<T>() where T : UiSceneBase
        {
            if (m_UiScenes.TryGetValue(typeof(T), out var uiScene))
                return (T)uiScene;
            else
                return CreateUiScene<T>();
        }

        private void OnAwakeUiScene()
        {
            if (UiCanvasProto == null)
                return;
            m_UiSceneRoot = new GameObject("UiSceneRoot");
            m_UiSceneRoot.transform.SetParent(this.transform);
            var customUiSceneRoot = new GameObject("CustomUiScenes");
            customUiSceneRoot.transform.SetParent(m_UiSceneRoot.transform);
            CreateUiScene<UiGameEngineScene>();
            m_UiSceneRoot = customUiSceneRoot;  // keep custom UiScenes hang on a separate root to ensure GameEngine can show its UI items above other all
        }
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
