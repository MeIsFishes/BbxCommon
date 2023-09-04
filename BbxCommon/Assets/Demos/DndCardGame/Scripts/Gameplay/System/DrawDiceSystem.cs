using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public partial class DrawDiceSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var combatDeckComp in GetEnumerator<CombatDeckRawComponent>())
            {
                while (combatDeckComp.DrawDiceRequest > 0)
                {
                    combatDeckComp.DrawDice();
                    combatDeckComp.DrawDiceRequest--;
                }
            }
        }
    }
}
