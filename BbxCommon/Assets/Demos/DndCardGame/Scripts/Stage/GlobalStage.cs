using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class GlobalStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Global Stage");

            stage.AddScene("DcgMain");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiGlobalScene"));

            stage.AddLoadItem(new InitSingletonComponent());
            stage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Config/DcgInteractingDataAsset"));
            stage.AddLoadItem(new InitPrefabData());
            stage.AddLoadItem(new InitCamera());
            stage.AddLoadItem(new InitMonsterData());

            stage.AddUpdateSystem<ProcessOperationSystem>();

            return stage;
        }

        private class InitSingletonComponent : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                EcsApi.AddSingletonRawComponent<OperationRequestSingletonRawComponent>();
            }

            void IStageLoad.Unload(GameStage stage)
            {
                EcsApi.RemoveSingletonRawComponent<OperationRequestSingletonRawComponent>();
            }
        }

        private class InitPrefabData : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var prefabData = Resources.Load<PrefabData>("DndCardGame/Config/PrefabData");
                DataApi.SetData(Object.Instantiate(prefabData));
            }

            void IStageLoad.Unload(GameStage stage)
            {
                DataApi.ReleaseData<PrefabData>();
            }
        }

        private class InitCamera : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var cameraData = Resources.Load<CameraData>("DndCardGame/Config/CameraData");
                DataApi.SetData(Object.Instantiate(cameraData));
                EntityCreator.CreateMainCameraEntity();
            }

            void IStageLoad.Unload(GameStage stage)
            {
                
            }
        }

        /// <summary>
        /// 怪物放到global初始化，以后可能做个怪物图鉴什么的
        /// </summary>
        private class InitMonsterData : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var monsterSetData = Resources.Load<MonsterSetData>("DndCardGame/Config/Monster/MonsterSetData");
                foreach (var item in monsterSetData.Items)
                {
                    DataApi.SetData(item.Id, Object.Instantiate(item.Data));
                }
            }

            void IStageLoad.Unload(GameStage stage)
            {
                DataApi.ReleaseAllData<MonsterData>();
            }
        }
    }
}
