using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class DungeonWalkStage
    {
        public static GameStage CreateStage()
        {
            var dungeonWalkStage = DcgGameEngine.Instance.StageWrapper.CreateStage("Dungeon Walk Stage");
            dungeonWalkStage.AddUpdateSystem<WalkToSystem>();
            return dungeonWalkStage;
        }
    }
}
