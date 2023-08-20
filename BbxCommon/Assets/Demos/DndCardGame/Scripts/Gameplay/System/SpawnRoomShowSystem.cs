using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateAfter(typeof(SpawnRoomSystem))]
    public partial class SpawnRoomShowSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var aspect in GetEnumerator<SpawnRoomShowRawAspect>())
            {
                aspect.ElapsedTime += UnityEngine.Time.deltaTime;
                aspect.Position = aspect.OriginalPos + aspect.Offset * (1 - aspect.TransitionCurve.Evaluate(aspect.ElapsedTime));
                if (aspect.ElapsedTime > aspect.TransitionLength)
                    EntityUtility.Room.SpawnRoomEnd(aspect.GetEntity());
            }
        }
    }
}
