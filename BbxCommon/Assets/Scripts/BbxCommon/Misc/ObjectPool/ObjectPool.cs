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
        private static UniqueIdGenerator m_IDGenerator = new UniqueIdGenerator();
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
                obj.OnAllocate();
                return obj;
            }
            obj = new T();
            obj.OnAllocate();
            obj.UniqueID = m_IDGenerator.GenerateID();
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
                    item.UniqueID = m_IDGenerator.GenerateID();
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
            obj.UniqueID = m_IDGenerator.GenerateID();
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
        /// <summary>
        /// Check the given reference, allocate and return a pooled object if it is null.
        /// Example: obj = AllocIfNull(obj);
        /// </summary>
        public static Type AllocIfNull<Type>(Type reference) where Type : PooledObject, new()
        {
            if (reference == null)
                return ObjectPool<Type>.Alloc();
            return reference;
        }
    }
}
