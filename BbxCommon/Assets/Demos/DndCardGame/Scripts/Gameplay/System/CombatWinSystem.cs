using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    /// <summary>
    /// 监测战斗胜利的条件，并结算
    /// </summary>
    [DisableAutoCreation]
    public partial class CombatWinSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
            var characterEntity = combatInfoComp.Character;
            var attributesComp = characterEntity.GetRawComponent<AttributesRawComponent>();
            // 满足条件
            if (attributesComp.CurHp <= 0)
            {

            }
        }
    }
}
