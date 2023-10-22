using System;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    /// <summary>
    /// Input items as List to create a Dictionary.
    /// </summary>
    [Serializable]
    public class SerializableDic<TKey, TValue>
    {
        #region ListWrapper
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
        private Dictionary<TKey, TValue> m_Dictionary;

        private bool m_Inited;
        public void Init()
        {
            if (m_Inited == false || m_Dictionary == null)
                ForceInit();
        }

        public void ForceInit()
        {
            m_Inited = true;
            m_Dictionary?.CollectToPool();
            SimplePool.Alloc(out m_Dictionary);
            foreach (var item in m_Items)
            {
                m_Dictionary.Add(item.Key, item.Value);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Remove an element from the <see cref="List{T}"/> <see cref="m_Items"/>. This will not auto refresh generated <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public void RemoveFromList(TKey key)
        {
            int index = 0;
            for (int i = 0; i < m_Items.Count; i++)
            {
                if (m_Items[i].Key.Equals(key))
                {
                    index = i;
                    break;
                }
            }
            m_Items.RemoveAt(index);
        }

        /// <summary>
        /// Clear elements in the <see cref="List{T}"/> <see cref="m_Items"/>. This will not auto refresh generated <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public void ClearList()
        {
            m_Items.Clear();
        }

        /// <summary>
        /// Add an element to the <see cref="List{T}"/> <see cref="m_Items"/>. This will not auto refresh generated <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        public void SetToList(TKey key, TValue value)
        {
            for (int i = 0; i < m_Items.Count; i++)
            {
                if (m_Items[i].Key.Equals(key))
                {
                    m_Items[i] = new DataItem<TKey, TValue>(key, value);
                    return;
                }
            }
            m_Items.Add(new DataItem<TKey, TValue>(key, value));
        }
#endif
        #endregion

        #region RewriteDic
        public TValue this[TKey key]
        {
            get
            {
                Init();
                return m_Dictionary[key];
            }
            set
            {
                Init();
                m_Dictionary[key] = value;
            }
        }

        public int Count
        {
            get
            {
                Init();
                return m_Dictionary.Count;
            }
        }

        public void Add(TKey key, TValue value)
        {
            Init();
            m_Dictionary.Add(key, value);
        }

        public void Clear()
        {
            Init();
            m_Dictionary.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            Init();
            return m_Dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            Init();
            return m_Dictionary.ContainsValue(value);
        }

        public int EnsureCapacity(int capacity)
        {
            Init();
            return m_Dictionary.EnsureCapacity(capacity);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            Init();
            return m_Dictionary.GetEnumerator();
        }

        public bool Remove(TKey key, out TValue value)
        {
            Init();
            return m_Dictionary.Remove(key, out value);
        }

        public bool Remove(TKey key)
        {
            Init();
            return m_Dictionary.Remove(key);
        }

        public bool TryRemove(TKey key)
        {
            Init();
            return m_Dictionary.TryRemove(key);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            Init();
            return m_Dictionary.TryRemove(key, out value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            Init();
            return m_Dictionary.TryAdd(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Init();
            return m_Dictionary.TryGetValue(key, out value);
        }
        #endregion
    }
}
