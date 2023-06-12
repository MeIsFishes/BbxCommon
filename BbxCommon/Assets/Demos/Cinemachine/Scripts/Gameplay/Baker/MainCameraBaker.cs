using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Cin
{
    public class MainCameraBaker : EcsBaker
    {
        protected override void Bake()
        {
            AddRawComponent<MainCameraSingletonRawComponent>();
        }
    }
}
