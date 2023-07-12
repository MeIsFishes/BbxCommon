using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class DungeonWalkStage
    {
        public static GameStage CreateStage()
        {
            var stage = DcgGameEngine.Instance.StageWrapper.CreateStage("Dungeon Walk Stage");

            stage.AddUpdateSystem<WalkToSystem>();

            return stage;
        }
    }
}
