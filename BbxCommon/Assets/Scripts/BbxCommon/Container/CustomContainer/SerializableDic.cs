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
        private List<DataItem<TKey, TValue>> m_List;
        private Dictionary<TKey, TValue> m_Dictionary;

        private bool Inited;
        public void Init()
        {
            if (Inited == true)
                return;
            Inited = true;
            m_Dictionary = new Dictionary<TKey, TValue>();
            foreach (var item in m_List)
            {
                m_Dictionary.Add(item.Key, item.Value);
            }
            m_List.Clear();
        }
        #endregion

        #region RewriteDic
        public TValue this[TKey key] => m_Dictionary[key];
        public int Count => m_Dictionary.Count;

        public void Add(TKey key, TValue value) => m_Dictionary.Add(key, value);
        public void Clear() => m_Dictionary.Clear();
        public bool ContainsKey(TKey key) => m_Dictionary.ContainsKey(key);
        public bool ContainsValue(TValue value) => m_Dictionary.ContainsValue(value);
        public int EnsureCapacity(int capacity) => m_Dictionary.EnsureCapacity(capacity);
        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => m_Dictionary.GetEnumerator();
        public bool Remove(TKey key, out TValue value) => m_Dictionary.Remove(key, out value);
        public bool Remove(TKey key) => m_Dictionary.Remove(key);
        public bool TryAdd(TKey key, TValue value) => m_Dictionary.TryAdd(key, value);
        public bool TryGetValue(TKey key, out TValue value) => m_Dictionary.TryGetValue(key, out value);
        #endregion
    }
}
