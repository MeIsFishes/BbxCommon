using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Unity.Entities;
using BbxCommon.Ui;
using Cysharp.Threading.Tasks;
using BbxCommon.Internal;
using BbxCommon.Editor;
using UnityEditor;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace BbxCommon
{
    #region SystemGroup
    internal partial class UpdateSystemGroup : ComponentSystemGroup { }
    [UpdateBefore(typeof(UpdateSystemGroup))]
    internal partial class GameEngineEarlyUpdateSystemGroup : ComponentSystemGroup { }
    [UpdateAfter(typeof(UpdateSystemGroup))]
    internal partial class GameEngineLateUpdateSystemGroup : ComponentSystemGroup { }
    #endregion

    public interface IGameEngine
    {
        List<GameStage> GetEnabledGameStage();
    }

    internal static class GameEngineFacade
    {
        #region Loading Progress
        public static float LoadingProgress => m_CurLoadingWeight / (float)m_TotalLoadingWeight;

        private static long m_TotalLoadingWeight;
        private static long m_CurLoadingWeight;

        public static void SetTotalLoadingWeight(long weight)
        {
            m_TotalLoadingWeight = weight;
            m_CurLoadingWeight = 0;
        }

        public static void SetLoadingWeight(long weight)
        {
            m_CurLoadingWeight += weight;
            if (m_CurLoadingWeight > m_TotalLoadingWeight)
                m_CurLoadingWeight = m_TotalLoadingWeight;
        }
        #endregion
    }

    public abstract partial class GameEngineBase<TEngine> : MonoSingleton<TEngine>, IGameEngine where TEngine : GameEngineBase<TEngine>
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

        #region Unity Callbacks
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

#if UNITY_EDITOR
            GameStageWindow.CurGameEngine = this;
#endif
        }

        protected virtual void OnAwake() { }

        private void Update()
        {
            OnUpdateStage();
        }

        private void OnDestroy()
        {
            GameStageWindow.CurGameEngine = null;
        }
        #endregion

        #region UiScene
        public GameObject UiCanvasProto;

        private GameObject m_UiSceneRoot;
        private Dictionary<Type, UiSceneBase> m_UiScenes = new Dictionary<Type, UiSceneBase>();
        private UiControllerBase m_LoadingController;

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

            // initialize UI prefab data
            var data = Resources.Load<PreLoadUiData>(BbxVar.ExportPreLoadUiPathInResources);
            data = Instantiate(data);   // create a copy
            DataApi.SetData(data);
        }
        #endregion

        #region EcsWorld
        private World m_EcsWorld;
        private Entity m_SingletonEntity;


        private void OnAwakeEcsWorld()
        {
            m_EcsWorld = World.DefaultGameObjectInjectionWorld;
            m_EcsWorld.CreateSystem<UpdateSystemGroup>();
            m_EcsWorld.CreateSystem<GameEngineEarlyUpdateSystemGroup>();
            m_EcsWorld.CreateSystem<GameEngineLateUpdateSystemGroup>();
            m_SingletonEntity = EcsApi.CreateEntity();
            EcsDataManager.SetSingletonRawComponentEntity(m_SingletonEntity);
        }
        #endregion

        #region GameStage
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

        private List<GameStage> m_EnabledStages = new();
        private List<OperateStage> m_OperateStages = new();

        List<GameStage> IGameEngine.GetEnabledGameStage()
        {
            return m_EnabledStages;
        }

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

        private bool m_StageIsDirty;
        private void LoadStage(GameStage stage)
        {
            m_OperateStages.Add(new OperateStage(stage, EOperateStage.Load));
            m_StageIsDirty = true;
        }

        private void UnloadStage(GameStage stage)
        {
            m_OperateStages.Add(new OperateStage(stage, EOperateStage.Unload));
            m_StageIsDirty = true;
        }

        private void OnAwakeStage()
        {
            LoadStage(CreateGameEngineStage());
        }

        private async void StartLoading()
        {
            m_LoadingController?.Show();
            var loadingTimeData = DataApi.GetData<LoadingTimeData>();
            if (loadingTimeData == null)
            {
                loadingTimeData = ResourceApi.EditorOperation.LoadOrCreateAssetInResources<LoadingTimeData>(BbxVar.ExportLoadingTimeDataPath);
            }
            DataApi.SetData(loadingTimeData);
            SetStageLoadingWeight();
            // unload stage
            for (int i = 0; i < m_OperateStages.Count; i++)
            {
                if (m_OperateStages[i].OperateType == EOperateStage.Unload)
                {
                    var stage = m_OperateStages[i].Stage;
                    var key = stage.StageName + ".Unload";
                    var sampler = DebugApi.BeginSample(key);
                    stage.UnloadStage();
                    m_EnabledStages.Remove(stage);
                    sampler.EndSample();
                    GameEngineFacade.SetLoadingWeight(loadingTimeData.GetLoadingTime(key));
#if UNITY_EDITOR
                    loadingTimeData.SetLoadingTime(key, sampler.TimeNs);
#endif
                    await UniTask.NextFrame();
                }
            }
            // load stage
            for (int i = 0; i < m_OperateStages.Count; i++)
            {
                if (m_OperateStages[i].OperateType == EOperateStage.Load)
                {
                    await m_OperateStages[i].Stage.LoadStage();
                    m_EnabledStages.Add(m_OperateStages[i].Stage);
                }
            }
            
            m_OperateStages.Clear();
            m_LoadingController?.Hide();
#if UNITY_EDITOR
            ResourceApi.EditorOperation.SetDirtyAndSave(loadingTimeData);
#endif
        }

        private void SetStageLoadingWeight()
        {
            if (m_OperateStages.Count == 0)
            {
                return;
            }

            var loadingTimeData = DataApi.GetData<LoadingTimeData>();
            long totalLoadingTime = 0;
            foreach (var operate in m_OperateStages)
            {
                var stage = operate.Stage;
                if (operate.OperateType == EOperateStage.Load)
                {
                    foreach (var pair in loadingTimeData.LoadingItemTimeDic)
                    {
                        if (pair.Key.StartsWith(stage.StageName + ".Load"))
                            totalLoadingTime += pair.Value;
                    }
                }
                else if (operate.OperateType == EOperateStage.Unload)
                {
                    foreach (var pair in loadingTimeData.LoadingItemTimeDic)
                    {
                        if (pair.Key.StartsWith(stage.StageName + ".Unload"))
                            totalLoadingTime += pair.Value;
                    }
                }
            }
            GameEngineFacade.SetTotalLoadingWeight(totalLoadingTime);
        }
        
        public void SetLoadingUi<T>() where T : UiControllerBase
        {
            if (m_LoadingController != null && m_LoadingController.GetType() == typeof(T))
                return;
            m_LoadingController?.Close();
            m_LoadingController = UiApi.OpenUiController<T>(UiApi.GetUiGameEngineScene().GetUiGroupCanvas(EUiGameEngine.Loading).transform, false);
        }
        
        private void OnUpdateStage()
        {
            if (m_StageIsDirty)
            {
                m_StageIsDirty = false;
                StartLoading();
            }
        }
        #endregion
    }
}
