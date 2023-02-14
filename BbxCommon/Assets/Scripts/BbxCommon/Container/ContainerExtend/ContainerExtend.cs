using UnityEngine;
using System.Collections.Generic;

namespace BbxCommon
{
    public static class ContainerExtend
    {
        #region List
        public static void Collect<T>(this List<T> list)
        {
            list.Clear();
            SimplePool<List<T>>.Collect(list);
        }

        /// <summary>
        /// Add array's members to the List.
        /// </summary>
        /// <param name="clear"> If true, the List will be clear first. </param>
        public static void AddArray<T>(this List<T> list, T[] array, bool clear = true)
        {
            if (clear)
                list.Clear();
            foreach (var m in array)
            {
                list.Add(m);
            }
        }

        /// <summary>
        /// Shuffle elements to random order.
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var index = Random.Range(0, list.Count);
                T k = list[i];
                list[i] = list[index];
                list[index] = k;
            }
        }
        #endregion

        #region Queue
        public static void Collect<T>(this Queue<T> queue)
        {
            queue.Clear();
            SimplePool<Queue<T>>.Collect(queue);
        }
        #endregion

        #region Stack
        public static void Collect<T>(this Stack<T> stack)
        {
            stack.Clear();
            SimplePool<Stack<T>>.Collect(stack);
        }
        #endregion

        #region LinkedList
        public static void Collect<T>(this LinkedList<T> linkedList)
        {
            linkedList.Clear();
            SimplePool<LinkedList<T>>.Collect(linkedList);
        }
        #endregion

        #region Dictionary
        public static void Collect<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            dic.Clear();
            SimplePool<Dictionary<TKey, TValue>>.Collect(dic);
        }
        #endregion

        #region HashSet
        public static void Collect<T>(this HashSet<T> set)
        {
            set.Clear();
            SimplePool<HashSet<T>>.Collect(set);
        }

        /// <summary>
        /// Add array's members to the List.
        /// </summary>
        /// <param name="clear"> If true, the HashSet will be clear first. </param>
        public static void AddArray<T>(this HashSet<T> set, T[] array, bool clear = true)
        {
            if (clear)
                set.Clear();
            foreach (var m in array)
            {
                set.Add(m);
            }
        }
        #endregion
    }
}
