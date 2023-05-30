using UnityEngine.Events;
using Unity.Entities;

namespace BbxCommon
{
    /// <summary>
    /// Base system, which is almost identical to Unity DOTS system.
    /// </summary>
    public abstract partial class EcsSystemBase : SystemBase
    {
        protected sealed override void OnCreate()
        {
            OnSystemCreate();
        }

        protected sealed override void OnUpdate()
        {
            OnSystemUpdate();
        }

        protected  sealed override void OnDestroy()
        {
            OnSystemDestroy();
        }

        protected virtual void OnSystemCreate() { }
        protected virtual void OnSystemUpdate() { }
        protected virtual void OnSystemDestroy() { }
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
            EcsDataManager.ForeachRawAspect(action);
        }
    }
}
