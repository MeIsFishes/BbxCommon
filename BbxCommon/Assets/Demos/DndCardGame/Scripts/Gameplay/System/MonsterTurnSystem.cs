using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateAfter(typeof(CombatTurnSystem))]
    public partial class MonsterTurnSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {

        }
    }
}
