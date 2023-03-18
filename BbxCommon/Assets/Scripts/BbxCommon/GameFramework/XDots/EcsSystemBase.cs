using Unity.Entities;

namespace BbxCommon.Framework
{
    /// <summary>
    /// High-performance system, which is almost identical to Unity DOTS system.
    /// </summary>
    [DisableAutoCreation]
    public abstract partial class EcsHpSystemBase : SystemBase, IEngineUpdate
    {
        void IEngineUpdate.OnCreate()
        {
            OnCreate();
        }

        void IEngineUpdate.OnUpdate()
        {
            OnUpdate();
        }

        void IEngineUpdate.OnDestroy()
        {
            OnDestroy();
        }
    }
}
