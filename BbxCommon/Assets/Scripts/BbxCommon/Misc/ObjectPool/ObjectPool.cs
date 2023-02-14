using UnityEngine;
using System.Collections.Generic;

namespace BbxCommon
{
    /* Use ObjectPool<T> like this:
     * 
     * var a = ObjectPool<A>.StaticAllocAnObject();
     * a.DoSomething();
     * a.Collect();
     *
     * Remember to manage objects' life cycle yourself. */
    internal interface IObjectPoolHandler
    {
        void Collect(IPooledObject obj);
    }


    public class ObjectPool<T> : Singleton<ObjectPool<T>>, IObjectPoolHandler where T : PooledObject, new()
    {
        private static UniqueIdGenerator m_s_IDGenerator = new UniqueIdGenerator();
        private List<T> m_Pool = new List<T>();

        /// <summary>
        /// Allocate an object of given type from pool, and it will call OnAllocate().
        /// </summary>
        public static T Alloc()
        {
            var objectSet = Instance.m_Pool;
            T res;
            if (objectSet.Count > 0)
            {
                res = objectSet[objectSet.Count - 1];
                objectSet.RemoveAt(objectSet.Count - 1);
                res.OnAllocate();
                return res;
            }
            res = new T();
            res.OnAllocate();
            res.UniqueID = m_s_IDGenerator.GenerateID();
            return res;
        }

        /// <summary>
        /// Create a large object pool.
        /// </summary>
        public static void StaticCreatePool()
        {
            StaticCreatePool(128);
        }

        /// <summary>
        /// Create a object pool with given size.
        /// </summary>
        public static void StaticCreatePool(uint size)
        {
            if (Instance.m_Pool.Count == 0)
            {
                for (int i = 0; i < size; i++)
                {
                    var item = new T();
                    Instance.m_Pool.Add(item);
                    item.ObjectPoolBelongs = Instance;
                    item.UniqueID = m_s_IDGenerator.GenerateID();
                }
            }
        }

        /// <summary>
        /// Collect the pooled object.
        /// </summary>
        public void Collect(T obj)
        {
            m_Pool.Add(obj);
            obj.ObjectPoolBelongs = this;
            obj.UniqueID = m_s_IDGenerator.GenerateID();
        }

        void IObjectPoolHandler.Collect(IPooledObject obj)
        {
            if (obj is T targetObj)
                Collect(targetObj);
        }
    }

    public static class ObjectPool
    {
        /// <summary>
        /// Check the given reference, allocate and return a pooled object if it is null.
        /// </summary>
        public static Type AllocIfNull<Type>(Type reference) where Type : PooledObject, new()
        {
            if (reference == null)
                return ObjectPool<Type>.Alloc();
            return reference;
        }
    }

    public struct PooledObjRef<T> where T : PooledObject
    {
        public T Obj => IsNull() ? null : m_Obj;

        private T m_Obj;
        private uint m_InstanceID;

        public PooledObjRef(T obj)
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
    }
}
