using System.Collections.Generic;
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
        protected T GetSingletonRawComponent<T>() where T : EcsSingletonRawComponent
        {
            return EcsDataManager.GetSingletonRawComponent<T>();
        }

        protected IEnumerable<T> GetEnumerator<T>() where T : EcsData
        {
            return EcsDataList<T>.GetEnumerator();
        }
    }
}
