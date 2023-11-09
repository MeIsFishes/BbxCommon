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
        public List<EDiceType> AttackDices;
        public List<EDiceType> DamageDices;
        public EAbility AttackModifier;

        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Intelligence;
        public int Wisdom;
    }
}
