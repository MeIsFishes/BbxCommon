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

    public static partial class ContainerExtend
    {
        public static void Collect<T>(this List<T> list)
        {
            list.Clear();
            SimplePool<List<T>>.Collect(list);
        }

        public static void Collect<T>(this Queue<T> queue)
        {
            queue.Clear();
            SimplePool<Queue<T>>.Collect(queue);
        }

        public static void Collect<T>(this Stack<T> stack)
        {
            stack.Clear();
            SimplePool<Stack<T>>.Collect(stack);
        }

        public static void Collect<T>(this LinkedList<T> linkedList)
        {
            linkedList.Clear();
            SimplePool<LinkedList<T>>.Collect(linkedList);
        }

        public static void Collect<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            dic.Clear();
            SimplePool<Dictionary<TKey, TValue>>.Collect(dic);
        }

        public static void Collect<T>(this HashSet<T> hashSet)
        {
            hashSet.Clear();
            SimplePool<HashSet<T>>.Collect(hashSet);
        }
    }
}
