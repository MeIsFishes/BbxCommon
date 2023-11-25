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
        protected override void OnUpdate()
        {
            foreach (var combatTurnComp in GetEnumerator<CombatTurnRawComponent>())
            {
                if (combatTurnComp.DuringTurn == false && combatTurnComp.RequestBegin == true)
                {
                    var entity = combatTurnComp.GetEntity();
                    var combatDeckComp = entity.GetRawComponent<CombatDeckRawComponent>();
                    combatTurnComp.RequestBegin = false;
                    combatTurnComp.DuringTurn = true;
                    var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
                    if (combatTurnComp.GetEntity() == combatInfoComp.Character)
                    {
                        GameUtility.CombatTurn.ShowTurnUi(ECombatTurn.PlayerTurn);
                    }
                    else if (combatTurnComp.GetEntity() == combatInfoComp.Monster)
                    {
                        GameUtility.CombatTurn.ShowTurnUi(ECombatTurn.EnemyTurn);
                    }
                    combatDeckComp.DrawDice(5);
                }
            }
        }
    }
}
