using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public partial class DcgGameEngine
    {
        private GameStage CreateGlobalStage()
        {
            var globalStage = StageWrapper.CreateStage("Global Stage");

            globalStage.AddScene("DcgMain");

            globalStage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Config/DcgInteractingDataAsset"));
            globalStage.AddLoadItem(new InitPrefabData());

            return globalStage;
        }

        private class InitPrefabData : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var prefabData = Resources.Load<PrefabData>("DndCardGame/Config/PrefabData");
                DataApi.SetData(prefabData);
            }

            void IStageLoad.Unload(GameStage stage)
            {
                DataApi.ReleaseData<PrefabData>();
            }
        }
    }
}
