using System.Collections.Generic;
using UnityEngine;

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
        /// Add another List's members to current List.
        /// </summary>
        /// <param name="clear"> If true, the List will be clear first. </param>
        public static void AddList<T>(this List<T> list, List<T> addList, bool clear = false)
        {
            if (clear)
                list.Clear();
            foreach (var m in addList)
            {
                list.Add(m);
            }
        }

        /// <summary>
        /// Add HashSet's members to current List.
        /// </summary>
        /// <param name="clear"> If true, the List will be clear first. </param>
        public static void AddHashSet<T>(this List<T> list, HashSet<T> set, bool clear = false)
        {
            if (clear)
                list.Clear();
            foreach (var m in set)
            {
                list.Add(m);
            }
        }

        public static HashSet<T> ToHashSet<T>(this List<T> list)
        {
            var set = new HashSet<T>();
            foreach (var data in list)
            {
                set.Add(data);
            }
            return set;
        }

        public static void ToHashSet<T>(this List<T> list, HashSet<T> set)
        {
            set.Clear();
            foreach (var data in list)
            {
                set.Add(data);
            }
        }

        /// <summary>
        /// Collect all elements in the List to <see cref="ObjectPool{T}"/>.
        /// </summary>
        /// <param name="collectSelf"> If true, the list will be collected after solving elements. </param>
        public static void CollectAndClearElements<T>(this List<T> list, bool collectSelf = false) where T : PooledObject
        {
            foreach (var element in list)
            {
                element.CollectToPool();
            }
            list.Clear();
            if (collectSelf)
                list.CollectToPool();
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
        public static void TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            if (dic.ContainsKey(key))
                dic.Remove(key);
        }

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

        public static void TryAdd<T>(this HashSet<T> set, T element)
        {
            if (set.Contains(element) == false)
                set.Add(element);
        }

        /// <summary>
        /// Add array's members to the HashSet.
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

        public static List<T> ToList<T>(this HashSet<T> set)
        {
            var list = new List<T>();
            foreach (var data in set)
            {
                list.Add(data);
            }
            return list;
        }

        public static void ToList<T>(this HashSet<T> set, List<T> list)
        {
            list.Clear();
            foreach (var data in set)
            {
                list.Add(data);
            }
        }
        #endregion

        #region String
        public static bool IsNullOrEmpty(this string str)
        {
            return str == null || str.Length == 0;
        }

        public static bool NotNullOrEmpty(this string str)
        {
            return str != null && str.Length > 0;
        }
        #endregion
    }
}
