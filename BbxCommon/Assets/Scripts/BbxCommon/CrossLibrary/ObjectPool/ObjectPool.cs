﻿using System.Collections.Generic;

namespace BbxCommon
{
    /* Use ObjectPool<T> like this:
     * 
     * var a = ObjectPool<A>.Alloc();
     * a.DoSomething();
     * a.CollectToPool();
     *
     * Remember to manage objects' life cycle yourself. */
    internal interface IObjectPoolHandler
    {
        void Collect(IPooledObject obj);
    }


    public class ObjectPool<T> : Singleton<ObjectPool<T>>, IObjectPoolHandler where T : PooledObject, new()
    {
        private static UniqueIdGenerator m_IdGenerator = new UniqueIdGenerator();
        private static List<T> m_Pool = new List<T>();

        /// <summary>
        /// Allocate an object of given type from pool, and it will call OnAllocate().
        /// </summary>
        public static T Alloc()
        {
            var objectSet = m_Pool;
            T obj;
            if (objectSet.Count > 0)
            {
                obj = objectSet[objectSet.Count - 1];
                objectSet.RemoveAt(objectSet.Count - 1);
            }
            obj = new T();
            obj.OnAllocate();
            obj.UniqueId = m_IdGenerator.GenerateId();
            obj.ObjectPoolBelongs = Instance;
            obj.IsCollected = false;
            return obj;
        }

        /// <summary>
        /// Create a large object pool.
        /// </summary>
        public static void CreatePool()
        {
            CreatePool(128);
        }

        /// <summary>
        /// Create a object pool with given size.
        /// </summary>
        public static void CreatePool(uint size)
        {
            if (m_Pool.Count == 0)
            {
                for (int i = 0; i < size; i++)
                {
                    var item = new T();
                    m_Pool.Add(item);
                    item.ObjectPoolBelongs = Instance;
                    item.UniqueId = m_IdGenerator.GenerateId();
                }
            }
        }

        /// <summary>
        /// Collect the pooled object.
        /// </summary>
        public void Collect(T obj)
        {
            if (m_Pool.Count > BbxCrossVar.ObjectPoolLimit)
            {
                obj.UniqueId = m_IdGenerator.GenerateId();
#if UNITY_EDITOR
                DebugApi.LogWarning("Pooled objects count exceeds limit.");
#endif
                return;
            }
            m_Pool.Add(obj);
            obj.ObjectPoolBelongs = this;
            obj.UniqueId = m_IdGenerator.GenerateId();
            obj.IsCollected = true;
        }

        void IObjectPoolHandler.Collect(IPooledObject obj)
        {
            if (obj is T targetObj)
                Collect(targetObj);
        }
    }

    // a syntactic sugar class
    public static class ObjectPool
    {
        public static void Alloc<T>(out T obj) where T : PooledObject, new()
        {
            obj = ObjectPool<T>.Alloc();
        }

        public static void Alloc<T>(out ObjRef<T> objRef) where T : PooledObject, new()
        {
            objRef = new ObjRef<T>(ObjectPool<T>.Alloc());
        }

        /// <summary>
        /// Check the given reference, allocate and return a pooled object if it is null.
        /// Example: obj = AllocIfNull(obj);
        /// </summary>
        public static T AllocIfNull<T>(T reference) where T : PooledObject, new()
        {
            if (reference == null)
                return ObjectPool<T>.Alloc();
            return reference;
        }
    }
}
