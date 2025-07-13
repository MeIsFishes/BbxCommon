using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using System.Runtime.CompilerServices;
using System;
using static UiCharacterStateContainerController;

namespace Dcg
{
    public class MapStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("MapStage");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiMap"));
            stage.AddLoadItem<MapInitData>();
            stage.AddUpdateSystem<MapSystem>();
   
            return stage;
        }

        private class MapInitData : IStageLoad
        {
            public void Load(GameStage stage)
            {
                var mapComp = EcsApi.AddSingletonRawComponent<MapSingletonRawComponent>();
                mapComp.InitByConfig();
            }

            public void Unload(GameStage stage)
            {
                EcsApi.RemoveSingletonRawComponent<MapSingletonRawComponent>();
            }
        }
    }
}
