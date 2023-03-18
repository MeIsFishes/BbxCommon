using System;
using System.Collections.Generic;
using UnityEngine;
using BbxCommon.Framework;

namespace BbxCommon
{
    public abstract class InteractingDataAsset<TKey> : InteractingDataAsset
    {
        [Serializable]
        public struct Data
        {
            public TKey Requester;
            public List<TKey> Responsers;
        }

        public List<Data> InteractingData = new List<Data>();

        public override void ApplyInteractingData()
        {
            foreach (var data in InteractingData)
            {
                var intList = SimplePool<List<int>>.Alloc();
                foreach (var responser in data.Responsers)
                {
                    intList.Add(responser.GetHashCode());
                }
                InteractorManager.Instance.RegisterInteracting(data.Requester.GetHashCode(), intList.ToArray());
                intList.CollectToPool();
            }
        }
    }

    public abstract class InteractingDataAsset : ScriptableObject, IEngineLoad
    {
        public abstract void ApplyInteractingData();

        public void Load()
        {
            ApplyInteractingData();
        }

        public void Unload()
        {

        }
    }
}
