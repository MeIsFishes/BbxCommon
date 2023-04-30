using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public enum EDataDistribution
    {
        /// <summary>
        /// In this mode, datas with numeric key will be stored in a <see cref="List{T}"/> whose index start from 0, to the maximum key value.
        /// </summary>
        Continuous,
        /// <summary>
        /// In this mode, datas with numeric key will be stored in a <see cref="Dictionary{TKey, TValue}"/> whose key is the type <see cref="int"/>.
        /// </summary>
        Discrete,
    }

    public static class DataApi
    {
        public static void SetData<T>(T data)
        {
            DataManager<T>.SetData(data);
        }

        public static void SetData<T>(string key, T data)
        {
            DataManager<T>.SetData(key, data);
        }

        public static void SetData<T>(int key, T data)
        {
            DataManager<T>.SetData(key, data);
        }

        public static T GetData<T>()
        {
            return DataManager<T>.GetData();
        }

        public static T GetData<T>(string key)
        {
            return DataManager<T>.GetData(key);
        }

        public static T GetData<T>(int key)
        {
            return DataManager<T>.GetData(key);
        }

        /// <param name="tryCollectToPool"> If true and if the data is a <see cref="PooledObject"/>, it will call <see cref="PooledObject.CollectToPool"/>. </param>
        public static void ReleaseData<T>(string key, bool tryCollectToPool = true)
        {
            DataManager<T>.ReleaseData(key, tryCollectToPool);
        }

        /// <param name="tryCollectToPool"> If true, the data released will call <see cref="PooledObject.CollectToPool"/> if it is a <see cref="PooledObject"/>. </param>
        public static void ReleaseData<T>(int key, bool tryCollectToPool = true)
        {
            DataManager<T>.ReleaseData(key, tryCollectToPool);
        }
    }

    /// <summary>
    /// You can store datas with any type in <see cref="DataManager{T}"/> with <see cref="string"/> or <see cref="int"/> key.
    /// If datas are with a continuous key, using <see cref="EDataDistribution.Continuous"/> may give you a better performance. See <see cref="EDataDistribution"/>.
    /// </summary>
    internal static class DataManager<T>
    {
        private static EDataDistribution m_Distribution = EDataDistribution.Discrete;
        private static T m_Data;
        private static List<T> m_DataList = new();
        private static Dictionary<string, T> m_StringDic = new();
        private static Dictionary<int, T> m_IntDic = new();

        internal static void SetDistribution(EDataDistribution distribution)
        {
            if (m_IntDic.Count > 0)
            {
                Debug.LogError("There has been elements in IntDic. You cannot set distribution type then.");
                return;
            }
            m_Distribution = distribution;
        }

        internal static void SetData(T data)
        {
            m_Data = data;
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
                        m_DataList.ModifyCount(key);
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

        internal static void ReleaseData(string key, bool tryCollectToPool)
        {
            m_StringDic.Remove(key, out var value);
            if (tryCollectToPool && value is PooledObject pooled)
                pooled.CollectToPool();
        }

        internal static void ReleaseData(int key, bool tryCollectToPool)
        {
            T released = default(T);
            switch (m_Distribution)
            {
                case EDataDistribution.Continuous:
                    released = m_DataList[key];
                    m_DataList[key] = default(T);
                    break;
                case EDataDistribution.Discrete:
                    m_IntDic.Remove(key, out released);
                    break;
            }
            if (tryCollectToPool && released is PooledObject pooled)
                pooled.CollectToPool();
        }
    }
}
