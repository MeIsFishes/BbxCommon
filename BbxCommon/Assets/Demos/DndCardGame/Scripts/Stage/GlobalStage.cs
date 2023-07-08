using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class GlobalStage
    {
        public static GameStage CreateStage()
        {
            var globalStage = DcgGameEngine.Instance.StageWrapper.CreateStage("Global Stage");

            globalStage.AddScene("DcgMain");

            globalStage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Config/DcgInteractingDataAsset"));
            globalStage.AddLoadItem(new InitPrefabData());
            globalStage.AddLoadItem(new InitCamera());

            return globalStage;
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
