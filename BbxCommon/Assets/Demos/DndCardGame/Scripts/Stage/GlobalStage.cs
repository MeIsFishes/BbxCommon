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

            stage.AddLoadItem(new InitSingletonComponent());
            stage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Config/DcgInteractingDataAsset"));
            stage.AddLoadItem(new InitPrefabData());
            stage.AddLoadItem(new InitCamera());

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
    }
}
