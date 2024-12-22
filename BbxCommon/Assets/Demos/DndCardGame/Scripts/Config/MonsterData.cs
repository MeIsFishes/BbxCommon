using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Sirenix.OdinInspector;
using BbxCommon;

namespace Dcg
{
    [CreateAssetMenu(fileName = "MonsterData", menuName = "Dcg/MonsterData")]
    public class MonsterData : BbxScriptableObject
    {
        public int Id;
        public GameObject Prefab;
        public string Name;

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
        public DamageType DamageType;
        public List<string> ArmorTags;

        protected override void OnLoad()
        {
            DataApi.SetData(Id, this);
            DataApi.SetData(name, this);
        }

        protected override void OnUnload()
        {
            DataApi.ReleaseData<MonsterData>(Id);
            DataApi.ReleaseData<MonsterData>(Name);
        }
    }
}
