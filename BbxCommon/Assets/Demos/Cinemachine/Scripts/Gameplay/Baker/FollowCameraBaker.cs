using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Cin
{
    public class FollowCameraBaker : EcsBaker
    {
        protected override void Bake()
        {
            AddRawComponent<FollowCameraSingletonRawComponent>();
            EcsApi.GetSingletonRawComponent<CameraDataSingletonRawComponent>().CamerasInScene.Add(Entity);
        }
    }
}
