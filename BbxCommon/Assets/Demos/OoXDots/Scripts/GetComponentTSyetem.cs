using System.Collections.Generic;
using Unity.Entities;
using BbxCommon;
using UnityEngine.Profiling;

namespace Oxd
{
    public class TestRawComponent : EcsRawComponent
    {

    }

    public static class Group<T> where T : EcsRawComponent
    {
        public static Dictionary<Entity, T> RawComponents = new Dictionary<Entity, T>();
    }

    [DisableAutoCreation]
    public class GetComponentTSyetem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            Profiler.BeginSample("GetComponentT");
            for (int i = 0; i < 30000; i++)
            {
                Group<TestRawComponent>.RawComponents.ContainsKey(Entity.Null);
            }
            Profiler.EndSample();
        }
    }

    [DisableAutoCreation]
    public class EmptySystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            Profiler.BeginSample("Empty");
            for (int i = 0; i < 30000; i++) ;
            Profiler.EndSample();
        }
    }
}
