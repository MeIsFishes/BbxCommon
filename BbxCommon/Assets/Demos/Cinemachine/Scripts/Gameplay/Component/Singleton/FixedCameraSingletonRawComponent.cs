using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin
{
    public class FixedCameraSingletonRawComponent : EcsSingletonRawComponent
    {
        public bool XMoveEnabled;
        public float XMin;
        public float XMax;
        public float XSpeed;
        public bool XPositive;

        public bool ZMoveEnabled;
        public float ZMin;
        public float ZMax;
        public float ZSpeed;
        public bool ZPositive;
    }
}
