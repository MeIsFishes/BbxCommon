using UnityEngine;
using System.Collections.Generic;

namespace BbxCommon
{
    public static class ContainerExtend
    {
        #region List
        public static void CollectToPool<T>(this List<T> list)
        {
            list.Clear();
            SimplePool<List<T>>.Collect(list);
        }

        /// <summary>
        /// Add array's members to the List.
        /// </summary>
        /// <param name="clear"> If true, the List will be clear first. </param>
        public static void AddArray<T>(this List<T> list, T[] array, bool clear = false)
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

        /// <summary>
        /// Remove an element of List, this will cause the List to be unordered.
        /// </summary>
        public static void UnorderedRemove<T>(this List<T> list, int index)
        {
            list[index] = list[list.Count - 1]; // save the value of last element, then remove the last one
            list.RemoveAt(list.Count - 1);
        }
        #endregion

        #region Queue
        public static void CollectToPool<T>(this Queue<T> queue)
        {
            queue.Clear();
            SimplePool<Queue<T>>.Collect(queue);
        }
        #endregion

        #region Stack
        public static void CollectToPool<T>(this Stack<T> stack)
        {
            stack.Clear();
            SimplePool<Stack<T>>.Collect(stack);
        }
        #endregion

        #region LinkedList
        public static void CollectToPool<T>(this LinkedList<T> linkedList)
        {
            linkedList.Clear();
            SimplePool<LinkedList<T>>.Collect(linkedList);
        }
        #endregion

        #region Dictionary
        public static void CollectToPool<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            dic.Clear();
            SimplePool<Dictionary<TKey, TValue>>.Collect(dic);
        }
        #endregion

        #region HashSet
        public static void CollectToPool<T>(this HashSet<T> set)
        {
            set.Clear();
            SimplePool<HashSet<T>>.Collect(set);
        }

        /// <summary>
        /// Add array's members to the List.
        /// </summary>
        /// <param name="clear"> If true, the HashSet will be clear first. </param>
        public static void AddArray<T>(this HashSet<T> set, T[] array, bool clear = false)
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
