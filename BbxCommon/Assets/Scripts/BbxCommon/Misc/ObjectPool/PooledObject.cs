
namespace BbxCommon
{
    #region PooledObject
    public class PooledObject : IPooledObject
    {
        internal IObjectPoolHandler ObjectPoolBelongs;
        internal uint UniqueID;

        /// <summary>
        /// Call OnCollect() and tell the object pool this object is ready to be reuse.
        /// </summary>
        public void CollectToPool()
        {
            if (ObjectPoolBelongs != null)
                ObjectPoolBelongs.Collect(this);
        }

        public virtual void OnAllocate() { }

        public virtual void OnCollect() { }
    }
    #endregion

    #region ObjRef
    public static class ObjRef
    {
        public static ObjRef<T> AsObjRef<T>(this T obj) where T : PooledObject
        {
            return new ObjRef<T>(obj);
        }
    }

    public struct ObjRef<T> where T : PooledObject
    {
        public T Obj => IsNull() ? null : m_Obj;

        private T m_Obj;
        private uint m_InstanceID;

        public ObjRef(T obj)
        {
            m_Obj = obj;
            m_InstanceID = obj.UniqueID;
        }

        public bool IsNull()
        {
            return m_Obj == null || m_Obj.UniqueID != m_InstanceID;
        }

        public void Release()
        {
            m_Obj = null;
        }

        public static bool operator ==(ObjRef<T> a, ObjRef<T> b)
        {
            return a.Obj == b.Obj;
        }

        public static bool operator !=(ObjRef<T> a, ObjRef<T> b)
        {
            return a.Obj != b.Obj;
        }
    }
    #endregion
}

