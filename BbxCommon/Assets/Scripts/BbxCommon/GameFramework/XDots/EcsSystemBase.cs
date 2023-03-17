using Unity.Entities;

namespace BbxCommon.GameEngine
{
    [DisableAutoCreation]
    public abstract partial class EcsSystemBase : SystemBase, IEngineUpdate
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
