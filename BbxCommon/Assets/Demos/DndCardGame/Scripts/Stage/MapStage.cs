using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;
using System.Runtime.CompilerServices;
using System;

namespace Dcg
{
    public class MapStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("MapStage");

            stage.SetUiScene(DcgGameEngine.Instance.UiScene, Resources.Load<UiSceneAsset>("DndCardGame/Config/UiScene/UiMap"));
   
            return stage;
        }
    }
}
