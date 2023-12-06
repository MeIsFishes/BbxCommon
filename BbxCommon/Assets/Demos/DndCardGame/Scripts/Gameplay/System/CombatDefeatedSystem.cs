using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    /// <summary>
    /// 监测战斗失败的条件，并结算
    /// </summary>
    [DisableAutoCreation]
    public partial class CombatDefeatedSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
            var characterEntity = combatInfoComp.Character;
            var attributesComp = characterEntity.GetRawComponent<AttributesRawComponent>();
            // 满足条件
            if (attributesComp.CurHp <= 0)
            {
                DcgGameEngine.Instance.RestartGame();
            }
        }
    }
}
