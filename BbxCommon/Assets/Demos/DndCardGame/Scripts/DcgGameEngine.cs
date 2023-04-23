using System;
using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    public class DcgGameEngine : GameEngineBase<DcgGameEngine>
    {
        [NonSerialized]
        public UiDungeonScene UiGameScene;

        protected override void InitSingletonComponents()
        {
            
        }

        protected override void OnAwake()
        {
            UiGameScene = UiSceneWrapper.CreateUiScene<UiDungeonScene>();

            var globalStage = StageWrapper.CreateStage("Global Stage");
            globalStage.AddScene("DcgMain");
            globalStage.SetUiScene(UiGameScene, Resources.Load<UiSceneAsset>("DndCardGame/Configs/UiDungeonScene"));
            globalStage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Configs/DcgInteractingDataAsset"));

            StageWrapper.LoadStage(globalStage);
        }
    }
}
