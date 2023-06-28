using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Cin
{
    public class FixedCameraBaker : EcsBaker
    {
        public float XMin;
        public float XMax;
        public float XSpeed;

        public float ZMin;
        public float ZMax;
        public float ZSpeed;

        protected override void Bake()
        {
            EcsApi.GetSingletonRawComponent<CameraDataSingletonRawComponent>().CamerasInScene.Add(Entity);

            var fixedCameraComp = AddRawComponent<FixedCameraSingletonRawComponent>();
            fixedCameraComp.XMin = XMin;
            fixedCameraComp.XMax = XMax;
            fixedCameraComp.XSpeed = XSpeed;
            fixedCameraComp.ZMin = ZMin;
            fixedCameraComp.ZMax = ZMax;
            fixedCameraComp.ZSpeed = ZSpeed;
        }
    }
}
