using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateAfter(typeof(CombatEndTurnSystem))]
    public partial class MonsterTurnSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {

        }
    }
}
