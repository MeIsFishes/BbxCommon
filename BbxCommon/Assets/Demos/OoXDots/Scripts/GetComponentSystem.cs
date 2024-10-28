using System;
using System.Collections.Generic;
using UnityEngine.Profiling;
using Unity.Entities;
using BbxCommon;

namespace Oxd
{
    public static class Group
    {
        public static Dictionary<Type, EcsRawComponent> RawComponents = new Dictionary<Type, EcsRawComponent>();
    }

    [DisableAutoCreation]
    public class GetComponentSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            Group.RawComponents.TryAdd(typeof(TestRawComponent), new TestRawComponent());
            Profiler.BeginSample("GetComponent");
            for (int i = 0; i < 30000; i++)
            {
                var test = Group.RawComponents[typeof(TestRawComponent)];
            }
            Profiler.EndSample();
        }
    }

    [DisableAutoCreation]
    public class GetAndCastSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            Profiler.BeginSample("GetAndCast");
            for (int i = 0; i < 30000; i++)
            {
                var test = (TestRawComponent)Group.RawComponents[typeof(TestRawComponent)];
            }
            Profiler.EndSample();
        }
    }
}
