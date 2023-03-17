using System.Collections.Generic;
using UnityEngine;
using BbxCommon;
using BbxCommon.GameEngine;

namespace Dcg
{
    public class DcgGameEngine : GameEngineBase<DcgGameEngine>
    {
        protected override void SetGlobalLoadingItems()
        {
            LoadingWrapper.AddGlobalLoadingItem(Resources.Load<DcgInteractingDataAsset>("DndCardGame/Configs/DcgInteractingDataAsset"));
        }

        protected override void SetGlobalTickingItems()
        {
            
        }
    }
}
