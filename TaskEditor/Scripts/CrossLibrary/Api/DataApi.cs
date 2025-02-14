using System.Collections.Generic;
using System.IO;

namespace BbxCommon
{
    public enum EDataDistribution
    {
        /// <summary>
        /// In this mode, data with numeric key will be stored in a <see cref="List{T}"/> whose index start from 0, to the maximum key value.
        /// </summary>
        Continuous,
        /// <summary>
        /// In this mode, data with numeric key will be stored in a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        Discrete,
    }

    public static class DataApi
    {
        public static void SetData<T>(T data) where T : new()
        {
            DataManager<T>.SetData(data);
        }

        public static void SetData<T>(string key, T data) where T : new()
        {
            DataManager<T>.SetData(key, data);
        }

        public static void SetData<T>(int key, T data) where T : new()
        {
            DataManager<T>.SetData(key, data);
        }

        /// <summary>
        /// Store an anonymous data. You can't get it out, unless using <see cref="GetEnumerator{T}"/>.
        /// </summary>
        public static void SetAnonymousData<T>(T data) where T : new()
        {
            DataManager<T>.SetAnonymousData(data);
        }

        public static T GetData<T>() where T : new()
        {
            return DataManager<T>.GetData();
        }

        public static T GetData<T>(string key) where T : new()
        {
            return DataManager<T>.GetData(key);
        }

        public static T GetData<T>(int key) where T : new()
        {
            return DataManager<T>.GetData(key);
        }

        /// <param name="tryCollectToPool"> If true and if the data is a <see cref="PooledObject"/>, it will call <see cref="PooledObject.CollectToPool"/>. </param>
        public static void ReleaseData<T>(bool tryCollectToPool = true) where T : new()
        {
            DataManager<T>.ReleaseData(tryCollectToPool);
        }

        /// <param name="tryCollectToPool"> If true and if the data is a <see cref="PooledObject"/>, it will call <see cref="PooledObject.CollectToPool"/>. </param>
        public static void ReleaseData<T>(string key, bool tryCollectToPool = true) where T : new()
        {
            DataManager<T>.ReleaseData(key, tryCollectToPool);
        }

        /// <param name="tryCollectToPool"> If true and if the data is a <see cref="PooledObject"/>, it will call <see cref="PooledObject.CollectToPool"/>. </param>
        public static void ReleaseData<T>(int key, bool tryCollectToPool = true) where T : new()
        {
            DataManager<T>.ReleaseData(key, tryCollectToPool);
        }

        /// <param name="tryCollectToPool"> If true and if the data is a <see cref="PooledObject"/>, it will call <see cref="PooledObject.CollectToPool"/>. </param>
        public static void ReleaseAllAnonymousData<T>(bool tryCollectToPool = true) where T : new()
        {
            DataManager<T>.ReleaseAllAnonymousData(tryCollectToPool);
        }

        /// <param name="tryCollectToPool"> If true and if the data is a <see cref="PooledObject"/>, it will call <see cref="PooledObject.CollectToPool"/>. </param>
        public static void ReleaseAllData<T>(bool tryCollectToPool = true) where T : new()
        {
            DataManager<T>.ReleaseAllData(tryCollectToPool);
        }

        /// <summary>
        /// Return all different (references of) data <typeparamref name="T"/>, regardless of how they are stored.
        /// </summary>
        public static IEnumerable<T> GetEnumerator<T>() where T : new()
        {
            return DataManager<T>.GetEnumerator();
        }

        /// <summary>
        /// Force data to store with the specific strategy.
        /// </summary>
        public static void SetKeyDistribution<T>(EDataDistribution distribution) where T : new()
        {
            DataManager<T>.SetDistribution(distribution);
        }
    }

    /// <summary>
    /// You can store datas with any type in <see cref="DataManager{T}"/> with <see cref="string"/> or <see cref="int"/> key.
    /// If datas are with a continuous key, using <see cref="EDataDistribution.Continuous"/> may give you a better performance. See <see cref="EDataDistribution"/>.
    /// </summary>
    internal static class DataManager<T> where T : new()
    {
        #region Common
        private static EDataDistribution m_Distribution = EDataDistribution.Discrete;
        private static T m_Data;
        private static List<T> m_AnonymousData = new();
        private static List<T> m_DataList = new();
        private static Dictionary<string, T> m_StringDic = new();
        private static Dictionary<int, T> m_IntDic = new();

        internal static void SetDistribution(EDataDistribution distribution)
        {
            if (m_IntDic.Count > 0)
            {
                DebugApi.LogError("There has been elements in IntDic. You cannot set distribution type then.");
                return;
            }
            m_Distribution = distribution;
        }

        internal static void SetData(T data)
        {
            m_Data = data;
        }

        internal static void SetAnonymousData(T data)
        {
            m_AnonymousData.Add(data);
        }

        internal static void SetData(string key, T data)
        {
            m_StringDic[key] = data;
        }

        internal static void SetData(int key, T data)
        {
            switch (m_Distribution)
            {
                case EDataDistribution.Continuous:
                    if (m_DataList.Count > key)     // there is some overhead when branch prediction misses
                        m_DataList[key] = data;
                    else
                    {
                        if (key + 1 >= m_DataList.Count)
                            m_DataList.ModifyCount(key + 1);
                        m_DataList[key] = data;
                    }
                    break;
                case EDataDistribution.Discrete:
                    m_IntDic[key] = data;
                    break;
            }
        }

        internal static T GetData()
        {
            return m_Data;
        }

        internal static T GetData(string key)
        {
            if (m_StringDic.TryGetValue(key, out var value))
                return value;
            return default(T);
        }

        internal static T GetData(int key)
        {
            switch (m_Distribution)
            {
                case EDataDistribution.Continuous:
                    if (key < m_DataList.Count)
                        return m_DataList[key];
                    break;
                case EDataDistribution.Discrete:
                    if (m_IntDic.TryGetValue(key, out var value))
                        return value;
                    break;
            }
            return default(T);
        }

        internal static void ReleaseData(bool tryCollectToPool)
        {
            if (tryCollectToPool && m_Data is PooledObject pooled)
                pooled.CollectToPool();
            m_Data = default;
        }

        internal static void ReleaseData(string key, bool tryCollectToPool)
        {
            m_StringDic.Remove(key, out var value);
            if (tryCollectToPool && value is PooledObject pooled)
                pooled.CollectToPool();
        }

        internal static void ReleaseData(int key, bool tryCollectToPool)
        {
            T released = default;
            switch (m_Distribution)
            {
                case EDataDistribution.Continuous:
                    released = m_DataList[key];
                    m_DataList[key] = default;
                    break;
                case EDataDistribution.Discrete:
                    m_IntDic.Remove(key, out released);
                    break;
            }
            if (tryCollectToPool && released is PooledObject pooled)
                pooled.CollectToPool();
        }

        internal static void ReleaseAllAnonymousData(bool tryCollectToPool)
        {
            for (int i = 0; i < m_AnonymousData.Count; i++)
            {
                if (tryCollectToPool)
                    (m_AnonymousData[i] as PooledObject)?.CollectToPool();
            }
            m_AnonymousData.Clear();
        }

        internal static void ReleaseAllData(bool tryCollectToPool)
        {
            if (tryCollectToPool && m_Data is PooledObject pooled)
                pooled.CollectToPool();
            m_Data = default;

            for (int i = 0; i < m_AnonymousData.Count; i++)
            {
                if (tryCollectToPool)
                    (m_AnonymousData[i] as PooledObject)?.CollectToPool();
            }
            m_AnonymousData.Clear();

            for (int i = 0; i < m_DataList.Count; i++)
            {
                if (tryCollectToPool)
                    (m_DataList[i] as PooledObject)?.CollectToPool();
            }
            m_DataList.Clear();

            foreach (var pair in m_IntDic)
            {
                if (tryCollectToPool)
                    (pair.Value as PooledObject)?.CollectToPool();
            }
            m_IntDic.Clear();

            foreach (var pair in m_StringDic)
            {
                if (tryCollectToPool)
                    (pair.Value as PooledObject)?.CollectToPool();
            }
            m_StringDic.Clear();
        }

        internal static IEnumerable<T> GetEnumerator()
        {
            var set = SimplePool<HashSet<T>>.Alloc();
            if (m_Data != null)
            {
                set.Add(m_Data);
                yield return m_Data;
            }
            for (int i = 0; i < m_AnonymousData.Count; i++)
            {
                if (set.Contains(m_AnonymousData[i]) == false)
                {
                    set.Add(m_AnonymousData[i]);
                    yield return m_AnonymousData[i];
                }
            }
            for (int i = 0; i < m_DataList.Count; i++)
            {
                if (set.Contains(m_DataList[i]) == false)
                {
                    set.Add(m_DataList[i]);
                    yield return m_DataList[i];
                }
            }
            foreach (var pair in m_IntDic)
            {
                if (set.Contains(pair.Value) == false)
                {
                    set.Add(pair.Value);
                    yield return pair.Value;
                }
            }
            foreach (var pair in m_StringDic)
            {
                if (set.Contains(pair.Value) == false)
                {
                    set.Add(pair.Value);
                    yield return pair.Value;
                }
            }
            set.CollectToPool();
        }
        #endregion
    }
}
