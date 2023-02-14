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
        private static List<T> m_s_Pool = new List<T>();

        public static T Alloc()
        {
            T res;
            if (m_s_Pool.Count > 0)
            {
                res = m_s_Pool[m_s_Pool.Count - 1];
                m_s_Pool.RemoveAt(m_s_Pool.Count - 1);
                return res;
            }
            return new T();
        }

        public static void Collect(T obj)
        {
            m_s_Pool.Add(obj);
        }
    }
}
