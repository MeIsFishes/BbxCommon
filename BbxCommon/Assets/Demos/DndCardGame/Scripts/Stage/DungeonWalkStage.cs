using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public partial class DcgGameEngine
    {
        private GameStage CreateDungeonWalkStage()
        {
            var dungeonWalkStage = StageWrapper.CreateStage("Dungeon Walk Stage");
            dungeonWalkStage.AddUpdateSystem<WalkToSystem>();
            return dungeonWalkStage;
        }
    }
}
