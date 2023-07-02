using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [CreateAssetMenu(fileName = "PrefabData", menuName = "Demos/Dcg/PrefabData")]
    public class PrefabData : ScriptableObject
    {
        public SerializableDic<string, GameObject> PrefabDic = new();
    }
}
