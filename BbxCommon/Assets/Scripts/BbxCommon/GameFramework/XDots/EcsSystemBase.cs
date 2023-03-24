using UnityEngine.Events;
using Unity.Entities;

namespace BbxCommon.Framework
{
    /// <summary>
    /// High-performance system, which is almost identical to Unity DOTS system.
    /// </summary>
    public abstract partial class EcsHpSystemBase : SystemBase
    {
        
    }

    /// <summary>
    /// Mixed system, support using <see cref="EcsRawComponent"/> related functions.
    /// </summary>
    public abstract partial class EcsMixSystemBase : EcsHpSystemBase
    {
        protected void ForeachRawComponent<T>(UnityAction<T> action) where T : EcsRawComponent
        {
            RawComponentManager.ForeachRawComponent(action);
        }

        protected T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return RawComponentManager.GetSingletonRawComponent<T>();
        }
    }
}
