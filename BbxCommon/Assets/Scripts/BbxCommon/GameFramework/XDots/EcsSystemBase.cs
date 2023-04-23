using UnityEngine.Events;
using Unity.Entities;

namespace BbxCommon
{
    /// <summary>
    /// Base system, which is almost identical to Unity DOTS system.
    /// </summary>
    public abstract partial class EcsSystemBase : SystemBase
    {
        
    }

    /// <summary>
    /// Mixed system, supports using <see cref="EcsRawComponent"/> related functions.
    /// </summary>
    public abstract partial class EcsMixSystemBase : EcsSystemBase
    {
        protected void ForeachRawComponent<T>(UnityAction<T> action) where T : EcsRawComponent
        {
            EcsDataManager.ForeachRawComponent(action);
        }

        protected T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return EcsDataManager.GetSingletonRawComponent<T>();
        }

        protected void ForeachRawAspect<T>(UnityAction<T> action) where T : EcsRawAspect
        {
            EcsDataManager.ForeachRawComponent(action);
        }
    }
}
