using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public partial class MapSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            // for test
            var context = ObjectPool<TaskContextTest>.Alloc();
            TaskApi.RunTask("Test", context);
            context.CollectToPool();
        }
    }
}

