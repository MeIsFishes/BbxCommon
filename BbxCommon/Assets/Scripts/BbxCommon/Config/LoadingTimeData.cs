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

        private void OnEnable()
        {
            RefreshStageItemDic();
        }

        public long GetLoadingTime(string key)
        {
            if (LoadingItemTimeDic.TryGetValue(key, out long value))
            {
                return value;
            }
            return 1;
        }

        public void SetLoadingTime(string key, long time)
        {
            LoadingItemTimeDic[key] = time;
        }

        private void RefreshStageItemDic()
        {
            m_StageItemDic.Clear();
            foreach (var pair in LoadingItemTimeDic)
            {
                var strs = pair.Key.Split('.');
                m_StageItemDic.GetOrAdd(strs[0], out var items);
                items[pair.Key] = pair.Value;
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