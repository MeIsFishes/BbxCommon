using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public partial class GameEngineBase<TEngine>
    {
        private GameStage CreateGameEngineStage()
        {
            var stage = StageWrapper.CreateStage("Game Engine Stage");

            stage.AddDataGroup("GameEngineDefault");

            return stage;
        }
    }
}
