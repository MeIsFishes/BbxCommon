using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Cin
{
    public class NoMoveCameraBaker : EcsBaker
    {
        protected override void Bake()
        {
            AddRawComponent<NoMoveCameraSingletonRawComponent>();
            EcsApi.GetSingletonRawComponent<CameraDataSingletonRawComponent>().CamerasInScene.Add(Entity);
        }
    }
}
