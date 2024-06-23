using BbxCommon;
using System.Collections.Generic;
using UnityEngine;

namespace Dcg
{
    public class CombatDefeatedListener : StageListenerBase
    {
        protected override void InitListener()
        {
            var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
            var characterEntity = combatInfoComp.Character;
            var attributesComp = characterEntity.GetRawComponent<AttributesRawComponent>();
            AddVariableDirtyListener(attributesComp.CurHpVariable, (int value) =>
            {
                if (value <= 0)
                {
                    DcgGameEngine.Instance.RestartGame();
                }
            });
        }
    }
}
