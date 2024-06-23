using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class CombatWinListener : StageListenerBase
    {
        protected override void InitListener()
        {
            var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
            var monsterEntity = combatInfoComp.Monster;
            var attributesComp = monsterEntity.GetRawComponent<AttributesRawComponent>();
            AddVariableDirtyListener(attributesComp.CurHpVariable, (int value) =>
            {
                if (value <= 0)
                {
                    DcgGameEngine.Instance.CombatWin();
                }
            });
        }
    }
}