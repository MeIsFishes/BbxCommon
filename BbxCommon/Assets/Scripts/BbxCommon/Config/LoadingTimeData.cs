using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace BbxCommon
{
    public class LoadingTimeData: ScriptableObject
    {
        public SerializableDic <string, long> LoadingItemTimeDic = new();
        private Dictionary<string, Dictionary<string, long>> m_StageItemDic = new();

        public long GetLoadingTime(string key)
        {
            if (LoadingItemTimeDic.TryGetValue(key, out long value))
            {
                return value;
            }
            return 1;
        }

        public void Refresh()
        {
            m_StageItemDic.Clear();
            foreach (var pair in LoadingItemTimeDic)
            {
                var strs = pair.Key.Split('.');
                m_StageItemDic.GetOrAdd(strs[0])[pair.Key.TryRemoveStart(strs[0] + ".")] = pair.Value;
            }
        }

        public Dictionary<string, long> GetStageItemDic(string stageName)
        {
            if (m_StageItemDic.TryGetValue(stageName, out var dic))
            {
                return dic;
            }
            return new Dictionary<string, long>();
        }
    }
}