using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon.Ui;

namespace BbxCommon
{
    #region SystemGroup
    public partial class UpdateSystemGroup : ComponentSystemGroup { }
    #endregion

    public abstract partial class GameEngineBase<TEngine> : MonoSingleton<TEngine> where TEngine : GameEngineBase<TEngine>
    {
        #region Wrappers
        public EngineUiSceneWp UiSceneWrapper;
        public EngineStageWp StageWrapper;

        private void InitWrapper()
        {
            UiSceneWrapper = new EngineUiSceneWp(this);
            StageWrapper = new EngineStageWp(this);
        }

        public struct EngineUiSceneWp
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineUiSceneWp(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public T CreateUiScene<T>() where T : UiSceneBase => m_Ref.CreateUiScene<T>();
            public T GetUiScene<T>() where T : UiSceneBase => m_Ref.GetUiScene<T>();
            public T GetOrCreateUiScene<T>() where T : UiSceneBase => m_Ref.GetOrCreateUiScene<T>();
        }

        public struct EngineStageWp
        {
            private GameEngineBase<TEngine> m_Ref;

            public EngineStageWp(GameEngineBase<TEngine> engine) { m_Ref = engine; }

            public GameStage CreateStage(string stageName) => m_Ref.CreateStage(stageName);
            public GameStage CreateStage<T>(string stageName) where T : GameStage, new() => m_Ref.CreateStage<T>(stageName);
            public void LoadStage(GameStage stage) => m_Ref.LoadStage(stage);
            public void UnloadStage(GameStage stage) => m_Ref.UnloadStage(stage);
        }
        #endregion

        #region UnityCallbacks
        protected sealed override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(this);
            
            InitWrapper();

            OnAwakeEcsWorld();
            OnAwakeUiScene();
            OnAwakeStage();

            // call overridable OnAwake() after all datas are initialized
            OnAwake();
        }

        protected virtual void OnAwake() { }

        private void Update()
        {
            OnUpdateStage();
        }

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

            UiApi.HudRoot = Instantiate(UiCanvasProto);
            UiApi.HudRoot.name = "HudRoot";
            UiApi.HudRoot.transform.SetParent(m_UiSceneRoot.transform);
            UiApi.HudRoot.GetComponent<Canvas>().sortingOrder = -100;

            var customUiSceneRoot = new GameObject("CustomUiScenes");
            customUiSceneRoot.transform.SetParent(m_UiSceneRoot.transform);

            var uiGameEngineScene = CreateUiScene<UiGameEngineScene>();
            UiApi.SetUiGameEngineScene(uiGameEngineScene);
            m_UiSceneRoot = customUiSceneRoot;  // keep custom UiScenes hang on a separate root to ensure GameEngine can show its UI items above other all
        }
        #endregion

        #region EcsWorld
        private World m_EcsWorld;
        private Entity m_SingletonEntity;

        /// <summary>
        /// You can initialize singleton components before <see cref="GameStage"/> loading.
        /// </summary>
        protected virtual void InitSingletonComponents() { }

        private void OnAwakeEcsWorld()
        {
            m_EcsWorld = World.DefaultGameObjectInjectionWorld;
            m_EcsWorld.CreateSystem<UpdateSystemGroup>();
            m_SingletonEntity = EcsApi.CreateEntity();
            //m_SingletonEntity = EcsApi.CreateEntity();
            EcsDataManager.SetSingletonRawComponentEntity(m_SingletonEntity);
            InitSingletonComponents();
        }
        #endregion

        #region Stage
        private enum EOperateStage
        {
            Load,
            Unload,
        }

        private struct OperateStage
        {
            public GameStage Stage;
            public EOperateStage OperateType;

            public OperateStage(GameStage stage, EOperateStage operateType)
            {
                Stage = stage;
                OperateType = operateType;
            }
        }

        private List<OperateStage> m_OperateStages = new List<OperateStage>();

        public GameStage CreateStage(string stageName)
        {
            return new GameStage(stageName, m_EcsWorld);
        }

        public T CreateStage<T>(string stageName) where T : GameStage, new()
        {
            var stage = new T();
            stage.Init(stageName, m_EcsWorld);
            return stage;
        }

        public void LoadStage(GameStage stage)
        {
            m_OperateStages.Add(new OperateStage(stage, EOperateStage.Load));
        }

        public void UnloadStage(GameStage stage)
        {
            m_OperateStages.Add(new OperateStage(stage, EOperateStage.Unload));
        }

        private void OnAwakeStage()
        {
            LoadStage(CreateGameEngineStage());
        }

        private void OnUpdateStage()
        {
            foreach (var operate in m_OperateStages)
            {
                switch (operate.OperateType)
                {
                    case EOperateStage.Load:
                        operate.Stage.LoadStage();
                        break;
                    case EOperateStage.Unload:
                        operate.Stage.UnloadStage();
                        break;
                }
            }
            m_OperateStages.Clear();
        }
        #endregion
    }
}
