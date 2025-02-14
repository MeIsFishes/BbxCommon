﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class MonsterRawComponent : EcsRawComponent
    {
        public string Name;
        public List<EDiceType> AttackDices = new();
        public List<EDiceType> DamageDices = new();
        public EAbility Modifier;

        public override void OnCollect()
        {
            AttackDices.Clear();
            DamageDices.Clear();
        }
    }
}
