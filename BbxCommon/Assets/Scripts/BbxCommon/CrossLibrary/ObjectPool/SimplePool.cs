using System.Collections;
using System.Collections.Generic;

namespace BbxCommon
{
    /// <summary>
    /// A simple object pool which doesn't force objects to implement IPooledObject. Normally uses as container's pool.
    /// When as container's pool, for example List's, do like this:
    ///     var list = SimplePool<List<T>>.Alloc();
    ///     list.DoSomething();
    ///     list.Collect();
    /// </summary>
    public static class SimplePool<T> where T : new()
    {
        private static List<T> m_Pool = new();

        public static T Alloc()
        {
            T res;
            if (m_Pool.Count > 0)
            {
                res = m_Pool[m_Pool.Count - 1];
                m_Pool.RemoveAt(m_Pool.Count - 1);
                return res;
            }
            return new T();
        }

        public static void Collect(T obj)
        {
#if UNITY_EDITOR
            if (m_Pool.Contains(obj))
            {
                DebugApi.LogError("The object " + obj.GetType().Name + " has already been collected");
                return;
            }
#endif
            if (m_Pool.Count > BbxCrossVar.SimplePoolLimit)
            {
#if UNITY_EDITOR
                DebugApi.LogWarning("Pooled objects count exceeds limit.");
#endif
                return;
            }
            m_Pool.Add(obj);
        }

        public static void Alloc(out T obj)
        {
            obj = Alloc();
        }
    }

    public static class SimplePool
    {
        public static T Alloc<T>(out T obj) where T : new()
        {
            obj = SimplePool<T>.Alloc();
            return obj;
        }

        public static void Collect<T>(T obj) where T : new()
        {
            SimplePool<T>.Collect(obj);
#if UNITY_EDITOR
            if (obj is ICollection)
                DebugApi.LogWarning("You called SimplePool.Collect() to collect a collector, which will not clear elements in it." +
                    "Use collector.CollectToPool instead!");
#endif
        }
    }
}
