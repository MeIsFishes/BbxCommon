using System.Collections.Generic;
using UnityEngine;
using BbxCommon;
using BbxCommon.Framework;

namespace Dcg
{
    public class DcgGameEngine : GameEngineBase<DcgGameEngine>
    {
        protected override string GetGameMainScene()
        {
            return null;
        }

        protected override void InitSingletonComponents()
        {
            
        }

        protected override void SetGlobalLoadItems()
        {
            GlobalStageWrapper.AddGlobalLoadItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Configs/DcgInteractingDataAsset"));
        }

        protected override void SetGlobalTickItems()
        {
            
        }
    }
}
