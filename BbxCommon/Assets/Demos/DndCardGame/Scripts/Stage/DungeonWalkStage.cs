using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using System.Runtime.CompilerServices;
using System;

namespace Dcg
{
    public class DungeonWalkStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Dungeon Walk Stage");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiDungeonWalkScene"));

            stage.AddUpdateSystem<WalkToSystem>();
            stage.AddUpdateSystem<EnterRoomSystem>();
            
            return stage;
        }
    }
}
