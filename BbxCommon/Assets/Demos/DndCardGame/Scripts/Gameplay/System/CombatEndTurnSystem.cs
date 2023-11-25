using System;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateBefore(typeof(CombatRoundSystem))]
    public class CombatEndTurnSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            
        }
    }
}
