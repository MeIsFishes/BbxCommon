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
            globalStage.SetUiScene(UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Configs/UiDungeonScene"));
            globalStage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Configs/DcgInteractingDataAsset"));
            return globalStage;
        }
    }
}
