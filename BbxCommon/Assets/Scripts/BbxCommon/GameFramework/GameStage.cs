using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Entities;
using BbxCommon.Ui;

namespace BbxCommon.Framework
{
    public class GameStage
    {
        #region Common
        public string StageName;

        // one for derived class to use if needs to extend, and another one for game framework to read
        protected bool m_Loaded;
        internal bool Loaded => m_Loaded;

        private World m_EcsWorld;

        public GameStage(string stageName, World ecsWorld)
        {
            StageName = stageName;
            m_EcsWorld = ecsWorld;
        }

        public virtual void LoadStage()
        {
            OnLoadStageScene();
            OnLoadStageUiScene();
            OnLoadStageLoad();
            OnLoadStageTick();
            m_Loaded = true;
        }

        public virtual void UnloadStage()
        {
            OnUnloadStageScene();
            OnUnloadStageUiScene();
            OnUnloadStageLoad();
            OnUnloadStageTick();
            m_Loaded = false;
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
                Debug.LogError("Current stage \"" + StageName + "\" has got a UiScene, you can only call SetUiScene once!");
                return;
            }
            m_UiScene = uiScene;
            m_UiSceneAsset = uiSceneAsset;
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
        protected List<IEngineLoad> m_StageLoadItems = new List<IEngineLoad>();

        public void AddLoadItem(IEngineLoad item)
        {
            m_StageLoadItems.Add(item);
        }

        protected void OnLoadStageLoad()
        {
            foreach (var item in m_StageLoadItems)
            {
                item.Load();
            }
        }

        protected void OnUnloadStageLoad()
        {
            foreach (var item in m_StageLoadItems)
            {
                item.Unload();
            }
        }
        #endregion

        #region StageTick
        protected List<EcsHpSystemBase> m_UpdateSystems = new List<EcsHpSystemBase>();
        protected List<EcsHpSystemBase> m_FixedUpdateSystems = new List<EcsHpSystemBase>();

        public void AddUpdateSystem<T>() where T : EcsHpSystemBase, new()
        {
            var system = m_EcsWorld.CreateSystemManaged<T>();
            m_UpdateSystems.Add(system);
        }

        public void AddFixedUpdateSystem<T>() where T : EcsHpSystemBase, new()
        {
            var system = m_EcsWorld.CreateSystemManaged<T>();
            m_FixedUpdateSystems.Add(system);
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
        }
        #endregion
    }
}
