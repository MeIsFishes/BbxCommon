using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin
{
    public class PlayerSingletonRawComponent : EcsSingletonRawComponent
    {
        public float Speed;
        public Vector3 SpawnPosition;
    }
}
