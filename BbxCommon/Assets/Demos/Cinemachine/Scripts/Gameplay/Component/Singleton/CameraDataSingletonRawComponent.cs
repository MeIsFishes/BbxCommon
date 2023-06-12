using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Cinemachine;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin
{
    public class CameraDataSingletonRawComponent : EcsSingletonRawComponent
    {
        public Entity CurCamera;
        public List<Entity> CamerasInScene = new();

        public void SetActiveCamera(Entity camera)
        {
            if (CurCamera.GetGameObject() != null)
                CurCamera.GetGameObject().GetComponent<CinemachineVirtualCamera>().Priority = 0;
            camera.GetGameObject().GetComponent<CinemachineVirtualCamera>().Priority = 10;
            CurCamera = camera;
        }

        public override void OnCollect()
        {
            CamerasInScene.Clear();
        }
    }
}
