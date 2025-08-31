using System;

namespace BbxCommon
{
    #region PooledObject
    public class PooledObject : IPooledObject
    {
        internal IObjectPoolHandler ObjectPoolBelongs;
        internal ulong UniqueId;
        internal bool IsCollected;

        /// <summary>
        /// Call OnCollect() and tell the object pool this object is ready to be reuse.
        /// </summary>
        public void CollectToPool()
        {
#if UNITY_EDITOR
            if (IsCollected == true)
            {
                DebugApi.LogError("The object has been collected!");
                return;
            }
#endif
            OnCollect();
            if (ObjectPoolBelongs != null)
                ObjectPoolBelongs.Collect(this);
        }

        void IPooledObject.OnAllocate()
        {
            OnAllocate();
        }

        void IPooledObject.OnCollect()
        {
            OnCollect();
        }

        protected virtual void OnAllocate() { }
        protected virtual void OnCollect() { }
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

    public struct ObjRef<T> : IEquatable<ObjRef<T>>
        where T : PooledObject
    {
        public T Obj => IsNull() ? null : m_Obj;

        private T m_Obj;
        private ulong m_InstanceId;

        public ObjRef(T obj)
        {
            if (obj == null)
            {
                m_Obj = null;
                m_InstanceId = 0;
            }
            else
            {
                m_Obj = obj;
                m_InstanceId = obj.UniqueId;
            }
        }

        public bool IsNotNull()
        {
            return m_Obj != null && m_Obj.UniqueId == m_InstanceId;
        }

        public bool IsNull()
        {
            return m_Obj == null || m_Obj.UniqueId != m_InstanceId;
        }

        public void ReleaseRef()
        {
            m_Obj = null;
        }

        bool IEquatable<ObjRef<T>>.Equals(ObjRef<T> other)
        {
            if (m_Obj != other.m_Obj || m_InstanceId != other.m_InstanceId)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return m_InstanceId.GetHashCode();
        }
    }
    #endregion
}

