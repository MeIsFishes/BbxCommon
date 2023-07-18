using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Sirenix.OdinInspector;
using BbxCommon;

namespace Dcg
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "Demos/Dcg/MonsterData")]
    public class MonsterData : ScriptableObject
    {
        public GameObject Prefab;
        public int HitPoints;
        public List<EDiceType> ArmorClass;
    }
}
