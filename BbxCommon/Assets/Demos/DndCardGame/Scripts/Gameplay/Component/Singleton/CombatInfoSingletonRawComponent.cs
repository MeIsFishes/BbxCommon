using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class CombatInfoSingletonRawComponent : EcsSingletonRawComponent
    {
        public Entity Character;
        public Entity Monster;
    }
}
