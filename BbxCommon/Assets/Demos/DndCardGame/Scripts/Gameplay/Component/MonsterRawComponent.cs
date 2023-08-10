using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class MonsterRawComponent : EcsRawComponent
    {
        public List<EDiceType> AttackDices = new();
        public List<EDiceType> DamageDices = new();

        public override void OnCollect()
        {
            AttackDices.Clear();
            DamageDices.Clear();
        }
    }
}
