using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateAfter(typeof(SpawnRoomShowSystem))]
    public partial class EnterRoomSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {

        }
    }
}
