using System.Collections.Generic;
using UnityEngine;
using BbxCommon;
using BbxCommon.Framework;

namespace Dcg
{
    public class DcgGameEngine : GameEngineBase<DcgGameEngine>
    {
        protected override void InitSingletonComponents()
        {
            
        }

        protected override void OnAwake()
        {
            var globalStage = StageWrapper.CreateStage("Global Stage");
            globalStage.AddLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Configs/DcgInteractingDataAsset"));
            globalStage.LoadStage();
        }
    }
}
