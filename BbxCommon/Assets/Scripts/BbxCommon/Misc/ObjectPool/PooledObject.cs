
namespace BbxCommon
{
    public class PooledObject : IPooledObject
    {
        internal IObjectPoolHandler ObjectPoolBelongs;
        internal uint UniqueID;

        /// <summary>
        /// Call OnCollect() and tell the object pool this object is ready to be reuse.
        /// </summary>
        public void Collect()
        {
            if (ObjectPoolBelongs != null)
                ObjectPoolBelongs.Collect(this);
        }

        public virtual void OnAllocate() { }

        public virtual void OnCollect() { }
    }
}

