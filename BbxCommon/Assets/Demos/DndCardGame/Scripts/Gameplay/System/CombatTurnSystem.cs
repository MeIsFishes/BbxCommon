using System;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateAfter(typeof(CombatRoundSystem))]
    public class CombatTurnSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            
        }
    }
}
