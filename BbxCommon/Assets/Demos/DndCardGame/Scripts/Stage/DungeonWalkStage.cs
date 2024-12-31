using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using System.Runtime.CompilerServices;
using System;
using static UiCharacterStateContainerController;

namespace Dcg
{
    public class DungeonWalkStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Dungeon Walk Stage");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiDungeonWalkScene"));
            stage.AddLateLoadItem(new DungeonWalkStageBindUi());
            stage.AddUpdateSystem<WalkToSystem>();
            
            return stage;
        }

        public class DungeonWalkStageBindUi : IStageLoad
        {
            public void Load(GameStage stage)
            {
                UiApi.GetUiController<UiCharacterStateContainerController>().Refresh(ECharacterType.DungeonWalk);
            }

            public void Unload(GameStage stage)
            {

            }
        }
    }
}
