﻿using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateAfter(typeof(SpawnRoomSystem))]
    public partial class SpawnRoomShowSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            foreach (var aspect in GetEnumerator<SpawnRoomShowRawAspect>())
            {
                aspect.ElapsedTime += UnityEngine.Time.deltaTime;
                aspect.Position = aspect.OriginalPos + aspect.Offset * (1 - aspect.TransitionCurve.Evaluate(aspect.ElapsedTime));
                if (aspect.ElapsedTime > aspect.TransitionLength)
                    GameUtility.Room.SpawnRoomEnd(aspect.GetEntity());
            }
        }
    }
}
