using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public partial class GameEngineBase<TEngine>
    {
        private GameStage CreateGameEngineStage()
        {
            var stage = StageWrapper.CreateStage("Game Engine Stage");

            stage.AddLoadItem(new PreLoadUi());

            return stage;
        }

        private class PreLoadUi : IStageLoad
        {
            void IStageLoad.Load(GameStage stage)
            {
                var data = Resources.Load<PreLoadUiData>(GlobalStaticVariable.ExportPreLoadUiPathInResources);
                data = Instantiate(data);   // create a copy
                DataApi.SetData(data);
            }

            void IStageLoad.Unload(GameStage stage)
            {
                
            }
        }
    }
}
