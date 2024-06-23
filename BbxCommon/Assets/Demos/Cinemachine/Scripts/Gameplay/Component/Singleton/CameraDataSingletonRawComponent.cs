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
        public ListenableVariable<Entity> CurCamera = new();
        public List<Entity> CamerasInScene = new();

        public void SetActiveCamera(Entity camera)
        {
            foreach (var cameraInScene in CamerasInScene)
            {
                cameraInScene.GetGameObject().GetComponent<CinemachineVirtualCameraBase>().Priority = 0;
            }
            camera.GetGameObject().GetComponent<CinemachineVirtualCameraBase>().Priority = 10;
            CurCamera.SetValue(camera);
        }

        public override void OnCollect()
        {
            CamerasInScene.Clear();
        }
    }
}
