using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Sirenix.OdinInspector;
using BbxCommon;

namespace Dcg
{
    [CreateAssetMenu(fileName = "MonsterSetData", menuName = "Demos/Dcg/MonsterSetData")]
    public class MonsterSetData : ScriptableObject
    {
        [Serializable]
        public struct MonsterItem
        {
            public int Id;
            public MonsterData Data;
#if UNITY_EDITOR
            public string Comment;
#endif
        }
        public List<MonsterItem> Items;
    }
}
