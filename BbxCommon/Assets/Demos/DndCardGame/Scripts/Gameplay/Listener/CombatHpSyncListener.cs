using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    // 把Combat实体的血量同步到Dungeon实体
    // 目前是直接从数组强拿的对象，以后出现一个玩家控制多个对象时这里会崩；另外暂未考虑玩家生命上限变化带来的影响。
    public class CombatHpSyncListener : StageListenerBase
    {
        protected override void InitListener()
        {
            var playerComp = EcsApi.GetSingletonRawComponent<LocalPlayerSingletonRawComponent>();
            var combatAttributesComp = playerComp.CombatEntities[0].GetRawComponent<AttributesRawComponent>();
            AddVariableDirtyListener(combatAttributesComp.CurHpVariable, (int value) =>
            {
                playerComp.DungeonEntities[0].GetRawComponent<AttributesRawComponent>().CurHp = value;
            });
        }
    }
}
