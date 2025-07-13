using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Unity.Entities;
using BbxCommon.Ui;
using BbxCommon.Internal;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace BbxCommon
{
    public interface IStageLoad
    {
        void Load(GameStage stage);
        void Unload(GameStage stage);
    }

    public class GameStage
    {
        #region Common
        public string StageName;

        // one for derived class to use if needs to extend, and another one for game framework to read
        protected bool m_Loaded;
        internal bool Loaded => m_Loaded;

        public UnityAction PreLoadStage;
        public UnityAction PostLoadStage;
        public UnityAction PreUnloadStage;
        public UnityAction PostUnloadStage;

        protected World m_EcsWorld;

        internal GameStage(string stageName, World ecsWorld)
        {
            StageName = stageName;
            m_EcsWorld = ecsWorld;
        }

        internal void Init(string stageName, World ecsWorld)
        {
            StageName = stageName;
            m_EcsWorld = ecsWorld;
        }

        // 2023.4.8:
        // Keep core functions to be inernal instead of public virtual. That is for loading stage in GameEngine via Update().
        // Also worrying about that is GameStage enough for development without extension?
        internal async UniTask LoadStage(IProgress<float> progress)
        {
            if (m_Loaded)
                return;
            if (AllParentsLoaded() == false)
                return;
            
            float gameTimeMilliseconds = Time.time * 1000;

            m_Loaded = true;
            PreLoadStage?.Invoke();
            await OnLoadStageLoad(progress);
            OnLoadStageScene();
            OnLoadStageUiScene();
            OnLoadStageData();
            OnLoadStageTick();
            OnLoadStageListener();
            await OnLoadStageLateLoad(progress);
            PostLoadStage?.Invoke();

            if (m_StageLoadItems.Count + m_StageLateLoadItems.Count == 0)
            {
                progress?.Report(StageLoadingWeight);
            }

            var costTime = Time.time * 1000 - gameTimeMilliseconds;
            if (costTime < 1)
            {
                costTime = 1f;
            }
            var loadingTime = DataApi.GetData<LoadingTimeData>();
            loadingTime.dataDictionary[StageName] = costTime;
            
            await UniTask.NextFrame();
        }
        
        public float StageLoadingWeight = 1f;
        
        internal void UnloadStage()
        {
            if (m_Loaded == false)
                return;

            PreUnloadStage?.Invoke();
            OnUnloadStageLateLoad();
            OnUnloadStageListener();
            OnUnloadStageTick();
            OnUnloadStageData();
            OnUnloadStageUiScene();
            OnUnloadStageScene();
            OnUnloadStageLoad();
            m_Loaded = false;
            OnUnloadStageChildStage();
            PostUnloadStage?.Invoke();
        }
        #endregion

        #region ChildStage
        public GameStage Parent;
        private Dictionary<string, GameStage> m_ChildStages = new Dictionary<string, GameStage>();

        public void AddChildStage(GameStage stage)
        {
            m_ChildStages.TryAdd(stage.StageName, stage);
        }

        public void RemoveChildStage(GameStage stage)
        {
            m_ChildStages.TryRemove(stage.StageName);
        }

        public void RemoveChildStage(string stageName)
        {
            m_ChildStages.TryRemove(stageName);
        }

        public GameStage GetChildStage(string stageName)
        {
            m_ChildStages.TryGetValue(stageName, out var stage);
            return stage;
        }

        public bool AllParentsLoaded()
        {
            var stage = this;
            while (stage.Parent != null)
            {
                if (stage.Parent.Loaded == false)
                {
                    DebugApi.LogWarning("You are requiring the GameStage " + StageName + " to load while its parent " + stage.StageName + " has not been loaded!");
                    return false;
                }
                stage = stage.Parent;
            }
            return true;
        }

        public void OnUnloadStageChildStage()
        {
            foreach (var pair in m_ChildStages)
            {
                pair.Value.UnloadStage();
            }
        }
        #endregion

        #region StageData
        // store datas in the stage to offer to load stage
        protected Dictionary<string, object> m_StageDatas = new Dictionary<string, object>();

        public void SetStageData(string key, object value, bool collectToPool = false)
        {
            if (collectToPool && m_StageDatas.TryGetValue(key, out var origin))
                ((PooledObject)origin)?.CollectToPool();
            m_StageDatas[key] = value;
        }

        public object GetStageData(string key)
        {
            m_StageDatas.TryGetValue(key, out var value);
            return value;
        }
        #endregion

        #region StageScene
        protected List<string> m_Scenes = new List<string>();

        public void AddScene(params string[] scenes)
        {
            m_Scenes.AddArray(scenes);
        }

        protected void OnLoadStageScene()
        {
            foreach (var scene in m_Scenes)
            {
                if (scene.IsNullOrEmpty())
                    continue;
                SceneManager.LoadScene(scene, LoadSceneMode.Additive);
            }
        }

        protected void OnUnloadStageScene()
        {
            foreach (var scene in m_Scenes)
            {
                if (scene.IsNullOrEmpty())
                    continue;
                SceneManager.UnloadSceneAsync(scene);
            }
        }
        #endregion

        #region StageUiScene
        protected UiSceneBase m_UiScene;
        protected UiSceneAsset m_UiSceneAsset;

        public void SetUiScene(UiSceneBase uiScene, UiSceneAsset uiSceneAsset)
        {
            if (m_UiScene != null)
            {
                DebugApi.LogError("Current stage \"" + StageName + "\" has got a UiScene, you can only call SetUiScene once!");
                return;
            }
            m_UiScene = uiScene;
            m_UiSceneAsset = Object.Instantiate(uiSceneAsset);
        }

        protected void OnLoadStageUiScene()
        {
            if (m_UiScene != null && m_UiSceneAsset != null)
                m_UiScene.CreateUiByAsset(m_UiSceneAsset);
        }

        protected void OnUnloadStageUiScene()
        {
            if (m_UiScene != null && m_UiSceneAsset != null)
                m_UiScene.DestroyUiByAsset(m_UiSceneAsset);
        }
        #endregion

        #region StageLoad
        protected List<IStageLoad> m_StageLoadItems = new List<IStageLoad>();
        protected List<IStageLoad> m_StageLateLoadItems = new List<IStageLoad>();

        public void AddLoadItem(IStageLoad item)
        {
            m_StageLoadItems.Add(item);
        }

        public void AddLoadItem<T>() where T : IStageLoad, new()
        {
            AddLoadItem(new T());
        }

        protected async UniTask OnLoadStageLoad(IProgress<float> progress)
        {
            foreach (var item in m_StageLoadItems)
            {
                var itemWeight = await AsyncLoadStageItem(item);
                progress?.Report(itemWeight);
            }
            
        }

        private async UniTask<float> AsyncLoadStageItem(IStageLoad item)
        {
            var itemCount = m_StageLoadItems.Count + m_StageLateLoadItems.Count;
            await UniTask.NextFrame();
            item.Load(this);
            return StageLoadingWeight / itemCount;
        }
        
        protected void OnUnloadStageLoad()
        {
            for (int i = m_StageLoadItems.Count - 1; i >= 0; i--)
            {
                m_StageLoadItems[i].Unload(this);
            }
        }

        public void AddLateLoadItem(IStageLoad item)
        {
            m_StageLateLoadItems.Add(item);
        }

        public void AddLateLoadItem<T>() where T : IStageLoad, new()
        {
            AddLateLoadItem(new T());
        }

        protected async UniTask OnLoadStageLateLoad(IProgress<float> progress)
        {
            foreach (var item in m_StageLateLoadItems)
            {
                var itemWeight = await AsyncLoadStageItem(item);
                progress?.Report(itemWeight);
            }
        }

        protected void OnUnloadStageLateLoad()
        {
            for (int i = m_StageLateLoadItems.Count - 1; i >= 0; i--)
            {
                m_StageLateLoadItems[i].Unload(this);
            }
        }
        #endregion

        #region StageTick
        protected List<EcsSystemBase> m_UpdateSystems = new();
        protected List<EcsSystemBase> m_FixedUpdateSystems = new();
        protected List<EcsSystemBase> m_GameEngineEarlySystems = new();
        protected List<EcsSystemBase> m_GameEngineLateSystems = new();

        public void AddUpdateSystem<T>() where T : EcsSystemBase, new()
        {
            var system = m_EcsWorld.CreateSystemManaged<T>();
            m_UpdateSystems.Add(system);
        }

        public void AddFixedUpdateSystem<T>() where T : EcsSystemBase, new()
        {
            var system = m_EcsWorld.CreateSystemManaged<T>();
            m_FixedUpdateSystems.Add(system);
        }

        internal void AddGameEngineEarlyUpdateSystem<T>() where T : EcsSystemBase, new()
        {
            var system = m_EcsWorld.CreateSystemManaged<T>();
            m_GameEngineEarlySystems.Add(system);
        }

        internal void AddGameEngineLateUpdateSystem<T>() where T : EcsSystemBase, new()
        {
            var system = m_EcsWorld.CreateSystemManaged<T>();
            m_GameEngineLateSystems.Add(system);
        }

        private void OnLoadStageTick()
        {
            foreach (var system in m_UpdateSystems)
            {
                var systemGroup = m_EcsWorld.GetExistingSystemManaged<UpdateSystemGroup>();
                systemGroup.AddSystemToUpdateList(system);
            }
            foreach (var system in m_FixedUpdateSystems)
            {
                var systemGroup = m_EcsWorld.GetExistingSystemManaged<FixedStepSimulationSystemGroup>();
                systemGroup.AddSystemToUpdateList(system);
            }
            foreach (var system in m_GameEngineEarlySystems)
            {
                var systemGroup = m_EcsWorld.GetExistingSystemManaged<GameEngineEarlyUpdateSystemGroup>();
                systemGroup.AddSystemToUpdateList(system);
            }
            foreach (var system in m_GameEngineLateSystems)
            {
                var systemGroup = m_EcsWorld.GetExistingSystemManaged<GameEngineLateUpdateSystemGroup>();
                systemGroup.AddSystemToUpdateList(system);
            }
        }


        private void OnUnloadStageTick()
        {
            foreach (var system in m_UpdateSystems)
            {
                var systemGroup = m_EcsWorld.GetExistingSystemManaged<UpdateSystemGroup>();
                systemGroup.RemoveSystemFromUpdateList(system);
            }
            foreach (var system in m_FixedUpdateSystems)
            {
                var systemGroup = m_EcsWorld.GetExistingSystemManaged<FixedStepSimulationSystemGroup>();
                systemGroup.RemoveSystemFromUpdateList(system);
            }
            foreach (var system in m_GameEngineEarlySystems)
            {
                var systemGroup = m_EcsWorld.GetExistingSystemManaged<GameEngineEarlyUpdateSystemGroup>();
                systemGroup.RemoveSystemFromUpdateList(system);
            }
            foreach (var system in m_GameEngineLateSystems)
            {
                var systemGroup = m_EcsWorld.GetExistingSystemManaged<GameEngineLateUpdateSystemGroup>();
                systemGroup.RemoveSystemFromUpdateList(system);
            }
        }
        #endregion

        #region StageListener
        private List<StageListenerBase> m_StageListeners = new();

        public void AddStageListener<T>() where T : StageListenerBase, new()
        {
            var listener = new T();
            m_StageListeners.Add(listener);
        }

        private void OnLoadStageListener()
        {
            foreach (var listener in m_StageListeners)
            {
                listener.OnLoad();
            }
        }

        private void OnUnloadStageListener()
        {
            foreach (var listener in m_StageListeners)
            {
                listener.OnUnload();
            }
        }
        #endregion

        #region StageData
        private List<string> m_LoadDataGroups = new();
        private HashSet<BbxScriptableObject> m_ScriptableObjects = new();

        public void AddDataGroup(string group)
        {
            m_LoadDataGroups.Add(group);
        }

        private void OnLoadStageData()
        {
            var soAssets = Resources.Load<ScriptableObjectAssets>(BbxVar.ExportScriptableObjectPathInResource);
            if (soAssets == null)
                return;
            for (int i = 0; i < m_LoadDataGroups.Count; i++)
            {
                var group = m_LoadDataGroups[i];
                if (soAssets.Assets.TryGetValue(group, out var paths))
                {
                    foreach (var path in paths)
                    {
                        var target = Resources.Load(ResourceApi.EditorOperation.RelativePathToResourcesPath(path));
                        if (target is BbxScriptableObject asset)
                        {
                            Object.Instantiate(asset).Load();
                            m_ScriptableObjects.TryAdd(asset);
                        }
                    }
                }
                if (ResourceApi.DataGroupCsvPairs.TryGetValue(group, out var csvDataList))
                {
                    foreach (var csvObj in csvDataList)
                    {
                        var tableNames = csvObj.GetTableNames();
                        bool foundTable = false;
                        foreach (var name in tableNames)
                        {
                            if (ResourceManager.LoadCsv(name, csvObj) == true)
                                foundTable = true;
                        }
                        if (foundTable == false)
                        {
                            DebugApi.LogWarning("There is no CSV file fits " + csvObj.GetType().FullName + " requires. It requires " + csvObj.GetTableNames());
                        }
                    }
                }
            }
        }

        private void OnUnloadStageData()
        {
            foreach (var asset in m_ScriptableObjects)
            {
                asset.Unload();
            }
        }
        #endregion
    }
}
