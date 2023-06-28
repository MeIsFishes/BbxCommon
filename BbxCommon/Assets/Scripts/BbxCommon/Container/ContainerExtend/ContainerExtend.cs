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
        public static void AddArray<T>(this List<T> list, T[] array)
        {
            foreach (var m in array)
            {
                list.Add(m);
            }
        }

        /// <summary>
        /// Add another List's members to current List.
        /// </summary>
        public static void AddList<T>(this List<T> list, List<T> addList)
        {
            foreach (var m in addList)
            {
                list.Add(m);
            }
        }

        /// <summary>
        /// Add HashSet's members to current List.
        /// </summary>
        public static void AddHashSet<T>(this List<T> list, HashSet<T> set)
        {
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
        public static void UnorderedRemoveAt<T>(this List<T> list, int index)
        {
            list[index] = list[list.Count - 1]; // save the value of last element, then remove the last one
            list.RemoveAt(list.Count - 1);
        }

        /// <summary>
        /// Modify the <see cref="List{T}"/>'s capacity and count to fit the specific count, and fill up all the new-created slots.
        /// The capacity will be set as required count * <paramref name="factor"/>, preparing for the upcoming elements.
        /// </summary>
        public static void ModifyCount<T>(this List<T> list, int count, float factor = 1.5f)
        {
            var fill = default(T);
            while (list.Capacity <= count)
                list.Capacity = Mathf.Max((int)(list.Capacity * factor), 8);    // default capacity is 0
            for (int i = list.Count; i < list.Capacity; i++)
                list.Add(fill);
        }

        public static T GetBack<T>(this List<T> list)
        {
            if (list.Count == 0)
                return default(T);
            return list[list.Count - 1];
        }

        /// <summary>
        /// Add the item in the front of the list.
        /// </summary>
        public static void AddFront<T>(this List<T> list, T item)
        {
            list.Insert(0, item);
        }

        /// <summary>
        /// Add items in the front of the list.
        /// </summary>
        public static void AddFront<T>(this List<T> list, List<T> items)
        {
            var originalCount = list.Count;
            list.ModifyCount(list.Count + items.Count);
            for (int i = originalCount; i >= 0; i--)
            {
                list[i + items.Count] = list[i];
            }
            for (int i = 0; i < items.Count; i++)
            {
                list[i] = items[i];
            }
        }

        /// <summary>
        /// Add items in the front of the list.
        /// </summary>
        public static void AddFront<T>(this List<T> list, HashSet<T> items)
        {
            var originalCount = list.Count;
            list.ModifyCount(list.Count + items.Count);
            for (int i = originalCount; i >= 0; i--)
            {
                list[i + items.Count] = list[i];
            }
            var index = 0;
            foreach (var item in items)
            {
                list[index] = item;
                index++;
            }
        }

        public static void RemoveFront<T>(this List<T> list)
        {
            list.RemoveAt(0);
        }

        public static void RemoveBack<T>(this List<T> list)
        {
            list.RemoveAt(list.Count - 1);
        }
        #endregion

        #region Queue
        public static void CollectToPool<T>(this Queue<T> queue)
        {
            queue.Clear();
            SimplePool<Queue<T>>.Collect(queue);
        }

        /// <summary>
        /// Collect all elements in the List to <see cref="ObjectPool{T}"/>.
        /// </summary>
        /// <param name="collectSelf"> If true, the list will be collected after solving elements. </param>
        public static void CollectAndClearElements<T>(this Queue<T> queue, bool collectSelf = false) where T : PooledObject
        {
            while (queue.Count > 0)
            {
                queue.Dequeue().CollectToPool();
            }
            if (collectSelf)
                queue.CollectToPool();
        }

        public static void EnqueueList<T>(this Queue<T> queue, List<T> list)
        {
            foreach (var item in list)
            {
                queue.Enqueue(item);
            }
        }

        public static void EnqueueQueue<T>(this Queue<T> queue, Queue<T> addQueue)
        {
            foreach (var item in addQueue)
            {
                queue.Enqueue(item);
            }
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
        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
        {
            if (dic.ContainsKey(key))
            {
                dic.Remove(key);
                return true;
            }
            return false;
        }

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, out TValue value)
        {
            if (dic.ContainsKey(key))
            {
                dic.Remove(key, out value);
                return true;
            }
            value = default(TValue);
            return false;
        }

        public static void CollectToPool<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            dic.Clear();
            SimplePool<Dictionary<TKey, TValue>>.Collect(dic);
        }

        /// <summary>
        /// Collect all elements in the List to <see cref="ObjectPool{T}"/>.
        /// </summary>
        /// <param name="collectSelf"> If true, the list will be collected after solving elements. </param>
        public static void CollectKeyAndClear<TKey, TValue>(this Dictionary<TKey, TValue> dic, bool collectSelf = false) where TKey : PooledObject
        {
            foreach (var pair in dic)
            {
                pair.Key.CollectToPool();
            }
            dic.Clear();
            if (collectSelf)
                dic.CollectToPool();
        }

        /// <summary>
        /// Collect all elements in the List to <see cref="ObjectPool{T}"/>.
        /// </summary>
        /// <param name="collectSelf"> If true, the list will be collected after solving elements. </param>
        public static void CollectValueAndClear<TKey, TValue>(this Dictionary<TKey, TValue> dic, bool collectSelf = false) where TValue : PooledObject
        {
            foreach (var pair in dic)
            {
                pair.Value.CollectToPool();
            }
            dic.Clear();
            if (collectSelf)
                dic.CollectToPool();
        }

        /// <summary>
        /// Collect all elements in the List to <see cref="ObjectPool{T}"/>.
        /// </summary>
        /// <param name="collectSelf"> If true, the list will be collected after solving elements. </param>
        public static void CollectKeyValueAndClear<TKey, TValue>(this Dictionary<TKey, TValue> dic, bool collectSelf = false)
            where TKey : PooledObject
            where TValue : PooledObject
        {
            foreach (var pair in dic)
            {
                pair.Key.CollectToPool();
                pair.Value.CollectToPool();
            }
            dic.Clear();
            if (collectSelf)
                dic.CollectToPool();
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
        public static void AddArray<T>(this HashSet<T> set, T[] array)
        {
            foreach (var m in array)
            {
                set.Add(m);
            }
        }

        /// <summary>
        /// Add List's members to the HashSet.
        /// </summary>
        public static void AddList<T>(this HashSet<T> set, List<T> list)
        {
            foreach (var m in list)
            {
                set.Add(m);
            }
        }

        /// <summary>
        /// Add HashSet's members to the HashSet.
        /// </summary>
        public static void AddHashSet<T>(this HashSet<T> set, HashSet<T> addSet)
        {
            foreach (var m in addSet)
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

        /// <summary>
        /// Collect all elements in the List to <see cref="ObjectPool{T}"/>.
        /// </summary>
        /// <param name="collectSelf"> If true, the list will be collected after solving elements. </param>
        public static void CollectAndClearElements<T>(this HashSet<T> set, bool collectSelf = false) where T : PooledObject
        {
            foreach (var item in set)
            {
                item.CollectToPool();
            }
            if (collectSelf)
                set.CollectToPool();
        }
        #endregion

        #region String
        public static string TryRemoveStart(this string str, string remove)
        {
            if (str.StartsWith(remove))
                return str.Remove(0, remove.Length);
            return str;
        }

        public static string TryRemoveEnd(this string str, string remove)
        {
            if (str.EndsWith(remove))
                return str.Remove(str.Length - remove.Length, remove.Length);
            return str;
        }

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
