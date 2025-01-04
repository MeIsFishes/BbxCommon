using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using System.Runtime.CompilerServices;
using System;

namespace Dcg
{
    public class GameStartStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Game Start Stage");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UIGameStart"));
   
            return stage;
        }
    }
}
