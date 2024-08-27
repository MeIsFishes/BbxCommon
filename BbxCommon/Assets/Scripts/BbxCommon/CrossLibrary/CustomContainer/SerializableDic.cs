using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    /// <summary>
    /// Input items as List to create a Dictionary.
    /// </summary>
    [Serializable]
    public class SerializableDic<TKey, TValue> : ISerializationCallbackReceiver
    {
        #region List Wrapper
        [Serializable]
        private struct DataItem<TItemKey, TItemValue>
        {
            public TItemKey Key;
            public TItemValue Value;

            public DataItem(TItemKey key, TItemValue value)
            {
                Key = key;
                Value = value;
            }
        }

        [SerializeField]
        private List<DataItem<TKey, TValue>> m_Items = new();
        private Dictionary<TKey, TValue> m_Dictionary = new();

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (m_Dictionary == null)
                SimplePool.Alloc(out m_Dictionary);
            m_Dictionary.Clear();
            foreach (var item in m_Items)
            {
                m_Dictionary.Add(item.Key, item.Value);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Caches the indexes of <see cref="DataItem{TItemKey, TItemValue}.Value"/>s.
        /// </summary>
        private Dictionary<TKey, int> m_Indexes = new();

        private int GetIndexOf(TKey key)
        {
            // init
            if (m_Indexes.Count == 0)
            {
                for (int i = 0; i < m_Items.Count; i++)
                {
                    m_Indexes.Add(m_Items[i].Key, i);
                }
            }
            if (m_Indexes.TryGetValue(key, out var index))
                return index;
            else
                return -1;
        }

        /// <summary>
        /// Remove an element from the <see cref="List{T}"/> <see cref="m_Items"/>. This will not auto update the generated <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        private void RemoveFromList(TKey key)
        {
            m_Items.RemoveAt(GetIndexOf(key));
            m_Indexes.Remove(key);
        }

        /// <summary>
        /// Clear elements in the <see cref="List{T}"/> <see cref="m_Items"/>. This will not auto update the generated <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        private void ClearList()
        {
            m_Items.Clear();
            m_Indexes.Clear();
        }

        /// <summary>
        /// Add an element to the <see cref="List{T}"/> <see cref="m_Items"/>. This will not auto update the generated <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        private void SetToList(TKey key, TValue value)
        {
            var index = GetIndexOf(key);
            if (index >= 0)
            {
                m_Items[index] = new DataItem<TKey, TValue>(key, value);
                m_Indexes[key] = index;
            }
            else
            {
                m_Items.Add(new DataItem<TKey, TValue>(key, value));
                m_Indexes[key] = m_Items.Count - 1;
            }
        }
#endif
        #endregion

        #region Rewrite Dic
        public TValue this[TKey key]
        {
            get
            {
                return m_Dictionary[key];
            }
            set
            {
#if UNITY_EDITOR
                SetToList(key, value);
#endif
                m_Dictionary[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return m_Dictionary.Count;
            }
        }

        public void Add(TKey key, TValue value)
        {
#if UNITY_EDITOR
            SetToList(key, value);
#endif
            m_Dictionary.Add(key, value);
        }

        public void Clear()
        {
#if UNITY_EDITOR
            ClearList();
#endif
            m_Dictionary.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return m_Dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return m_Dictionary.ContainsValue(value);
        }

        public int EnsureCapacity(int capacity)
        {
            return m_Dictionary.EnsureCapacity(capacity);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            return m_Dictionary.GetEnumerator();
        }

        public bool Remove(TKey key, out TValue value)
        {
#if UNITY_EDITOR
            RemoveFromList(key);
#endif
            return m_Dictionary.Remove(key, out value);
        }

        public bool Remove(TKey key)
        {
#if UNITY_EDITOR
            RemoveFromList(key);
#endif
            return m_Dictionary.Remove(key);
        }

        public bool TryRemove(TKey key)
        {
#if UNITY_EDITOR
            RemoveFromList(key);
#endif
            return m_Dictionary.TryRemove(key);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
#if UNITY_EDITOR
            RemoveFromList(key);
#endif
            return m_Dictionary.TryRemove(key, out value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
#if UNITY_EDITOR
            SetToList(key, value);
#endif
            return m_Dictionary.TryAdd(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_Dictionary.TryGetValue(key, out value);
        }
        #endregion
    }
}
