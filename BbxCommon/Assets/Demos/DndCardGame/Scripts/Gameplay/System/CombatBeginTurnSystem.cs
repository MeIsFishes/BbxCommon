using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateBefore(typeof(CombatEndTurnSystem))]
    public partial class CombatBeginTurnSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            foreach (var combatTurnComp in GetEnumerator<CombatTurnRawComponent>())
            {
                if (combatTurnComp.DuringTurn == false && combatTurnComp.RequestBegin == true)
                {
                    var entity = combatTurnComp.GetEntity();
                    combatTurnComp.RequestBegin = false;
                    combatTurnComp.DuringTurn = true;
                    var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
                    if (combatTurnComp.GetEntity() == combatInfoComp.Character)
                    {
                        var combatDeckComp = entity.GetRawComponent<CombatDeckRawComponent>();
                        GameUtility.CombatTurn.UpdateUiWhenTurnPass(ECombatTurn.PlayerTurn, entity);
                        combatDeckComp.DrawDice(5);
                    }
                    else if (combatTurnComp.GetEntity() == combatInfoComp.Monster)
                    {
                        GameUtility.CombatTurn.UpdateUiWhenTurnPass(ECombatTurn.EnemyTurn, entity);
                    }
                }
            }
        }
    }
}
