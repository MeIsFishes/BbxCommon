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
        #region Implementation
        [Serializable]
        private struct DataItem<TItemKey, TItemValue>
        {
            public TItemKey Key;
            public TItemValue Value;
        }

        [SerializeField]
        private List<DataItem<TKey, TValue>> m_Items;
        private Dictionary<TKey, TValue> m_Dictionary;

        private bool m_Inited;
        public void Init()
        {
            if (m_Inited == true)
                return;
            m_Inited = true;
            m_Dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in m_Items)
            {
                m_Dictionary.Add(item.Key, item.Value);
            }
        }
        #endregion

        #region RewriteDic
        public TValue this[TKey key]
        {
            get
            {
                if (m_Inited == false)
                    Init();
                return m_Dictionary[key];
            }
        }

        public int Count
        {
            get
            {
                if (m_Inited == false)
                    Init();
                return m_Dictionary.Count;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (m_Inited == false)
                Init();
            m_Dictionary.Add(key, value);
        }

        public void Clear()
        {
            if (m_Inited == false)
                Init();
            m_Dictionary.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.ContainsValue(value);
        }

        public int EnsureCapacity(int capacity)
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.EnsureCapacity(capacity);
        }

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator()
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.GetEnumerator();
        }

        public bool Remove(TKey key, out TValue value)
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.Remove(key, out value);
        }

        public bool Remove(TKey key)
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.Remove(key);
        }

        public bool TryRemove(TKey key)
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.TryRemove(key);
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.TryRemove(key, out value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.TryAdd(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (m_Inited == false)
                Init();
            return m_Dictionary.TryGetValue(key, out value);
        }
        #endregion
    }
}
