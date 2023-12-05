using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [CreateAssetMenu(fileName = "PrefabData", menuName = "Demos/Dcg/PrefabData")]
    public class PrefabData : BbxScriptableObject
    {
        public SerializableDic<string, GameObject> PrefabDic = new();

        protected override void OnLoad()
        {
            DataApi.SetData(this);
        }
    }
}
