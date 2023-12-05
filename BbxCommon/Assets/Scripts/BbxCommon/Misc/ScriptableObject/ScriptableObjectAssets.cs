using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BbxCommon
{
    public class ScriptableObjectAssets : ScriptableObject
    {
        /// <summary>
        /// Keys as group names, value as paths of files.
        /// </summary>
        public SerializableDic<string, SerializableHashSet<string>> Assets;

        public void SetAsset(string group, string path)
        {
            foreach (var pair in Assets)
            {
                if (pair.Value.Contains(path))
                    pair.Value.Remove(path);
            }
            if (Assets.ContainsKey(group) == false)
                Assets[group] = new();
            Assets[group].TryAdd(path);
        }
    }
}
