using System;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace BbxCommon
{
    [Serializable]
    public class SerializableHashSet<T> : ISerializationCallbackReceiver
    {
        #region List Wrapper
        [SerializeField]
        private List<T> m_Items = new();
        private HashSet<T> m_HashSet = new();

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_HashSet == null)
                SimplePool.Alloc(out m_HashSet);
            m_HashSet.Clear();
            for (int i = 0; i < m_Items.Count; i++)
            {
                m_HashSet.Add(m_Items[i]);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Caches the indexes of items in <see cref="m_HashSet"/>.
        /// </summary>
        private Dictionary<T, int> m_Indexes = new();

        private int GetIndexOf(T key)
        {
            // init
            if (m_Indexes.Count == 0)
            {
                for (int i = 0; i < m_Items.Count; i++)
                {
                    m_Indexes.Add(m_Items[i], i);
                }
            }
            if (m_Indexes.TryGetValue(key, out var index))
                return index;
            else
                return -1;
        }

        /// <summary>
        /// Remove an element from the <see cref="List{T}"/> <see cref="m_Items"/>. This will not auto update the generated <see cref="HashSet{T}"/>.
        /// </summary>
        private void RemoveFromList(T key)
        {
            m_Items.RemoveAt(GetIndexOf(key));
            m_Indexes.Remove(key);
        }

        /// <summary>
        /// Clear elements in the <see cref="List{T}"/> <see cref="m_Items"/>. This will not auto update the generated <see cref="HashSet{T}"/>.
        /// </summary>
        private void ClearList()
        {
            m_Items.Clear();
            m_Indexes.Clear();
        }

        /// <summary>
        /// Add an element to the <see cref="List{T}"/> <see cref="m_Items"/>. This will not auto update the generated <see cref="HashSet{T}"/>.
        /// </summary>
        private void SetToList(T key)
        {
            var index = GetIndexOf(key);
            if (index >= 0)
            {
                m_Items.Add(key);
                m_Indexes[key] = index;
            }
            else
            {
                m_Items.Add(key);
                m_Indexes[key] = m_Items.Count - 1;
            }
        }
#endif
        #endregion

        #region Rewrite HashSet
        #region Default
        public HashSet<T>.Enumerator GetEnumerator()
        {
            return m_HashSet.GetEnumerator();
        }

        public bool Add(T item)
        {
#if UNITY_EDITOR
            SetToList(item);
#endif
            return m_HashSet.Add(item);
        }

        public void Clear()
        {
#if UNITY_EDITOR
            ClearList();
#endif
            m_HashSet.Clear();
        }

        public bool Contains(T item)
        {
            return m_HashSet.Contains(item);
        }

        public int EnsureCapacity(int capacity)
        {
            return m_HashSet.EnsureCapacity(capacity);
        }

        public void Remove(T item)
        {
#if UNITY_EDITOR
            RemoveFromList(item);
#endif
            m_HashSet.Remove(item);
        }

        public bool TryGetValue(T item, out T value)
        {
            return m_HashSet.TryGetValue(item, out value);
        }
        #endregion

        #region Extension
        /// <summary>
        /// If contained, return false, or add and return true.
        /// </summary>
        public bool TryAdd(T element)
        {
            if (Contains(element))
                return false;
            else
            {
                Add(element);
                return true;
            }
        }

        /// <summary>
        /// Add array's members to the HashSet.
        /// </summary>
        public void AddArray(T[] array)
        {
            foreach (var m in array)
            {
                Add(m);
            }
        }

        /// <summary>
        /// Add List's members to the HashSet.
        /// </summary>
        public void AddList(List<T> list)
        {
            foreach (var m in list)
            {
                Add(m);
            }
        }

        /// <summary>
        /// Add HashSet's members to the HashSet.
        /// </summary>
        public void AddHashSet(HashSet<T> addSet)
        {
            foreach (var m in addSet)
            {
                Add(m);
            }
        }

        /// <summary>
        /// Add HashSet's members to the HashSet.
        /// </summary>
        public void AddHashSet(SerializableHashSet<T> addSet)
        {
            foreach (var m in addSet)
            {
                Add(m);
            }
        }

        /// <summary>
        /// Add Dictionary's key to the HashSet.
        /// </summary>
        public void AddDicKey<TValue>(Dictionary<T, TValue> dic)
        {
            foreach (var pair in dic)
            {
                Add(pair.Key);
            }
        }

        /// <summary>
        /// Add Dictionary's key to the HashSet.
        /// </summary>
        public void AddDicKey<TValue>(SerializableDic<T, TValue> dic)
        {
            foreach (var pair in dic)
            {
                Add(pair.Key);
            }
        }

        /// <summary>
        /// Add Dictionary's value to the HashSet.
        /// </summary>
        public void AddDicValue<TKey>(Dictionary<TKey, T> dic)
        {
            foreach (var pair in dic)
            {
                Add(pair.Value);
            }
        }

        /// <summary>
        /// Add Dictionary's value to the HashSet.
        /// </summary>
        public void AddDicValue<TKey>(SerializableDic<TKey, T> dic)
        {
            foreach (var pair in dic)
            {
                Add(pair.Value);
            }
        }

        public List<T> ToList()
        {
            var list = new List<T>();
            foreach (var data in this)
            {
                list.Add(data);
            }
            return list;
        }

        public void ToList(List<T> list)
        {
            list.Clear();
            foreach (var data in this)
            {
                list.Add(data);
            }
        }
        #endregion
        #endregion
    }
}
