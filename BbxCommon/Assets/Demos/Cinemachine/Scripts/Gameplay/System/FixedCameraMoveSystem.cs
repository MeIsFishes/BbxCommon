using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Cin
{
    [DisableAutoCreation]
    public partial class FixedCameraMoveSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var fixedCameraComp = EcsApi.GetSingletonRawComponent<FixedCameraSingletonRawComponent>();
            if (fixedCameraComp == null)
                return;
            var cameraGo = fixedCameraComp.GetEntity().GetGameObject();
            if (cameraGo == null)
                return;

            if (fixedCameraComp.XMoveEnabled)
            {
                if (fixedCameraComp.XPositive)
                {
                    cameraGo.transform.position = cameraGo.transform.position.AddX(fixedCameraComp.XSpeed * UnityEngine.Time.deltaTime);
                    if (cameraGo.transform.position.x >= fixedCameraComp.XMax)
                        fixedCameraComp.XPositive = false;
                }
                else
                {
                    cameraGo.transform.position = cameraGo.transform.position.AddX(-fixedCameraComp.XSpeed * UnityEngine.Time.deltaTime);
                    if (cameraGo.transform.position.x <= fixedCameraComp.XMin)
                        fixedCameraComp.XPositive = true;
                }
            }

            if (fixedCameraComp.ZMoveEnabled)
            {
                if (fixedCameraComp.ZPositive)
                {
                    cameraGo.transform.position = cameraGo.transform.position.AddZ(fixedCameraComp.ZSpeed * UnityEngine.Time.deltaTime);
                    if (cameraGo.transform.position.z >= fixedCameraComp.ZMax)
                        fixedCameraComp.ZPositive = false;
                }
                else
                {
                    cameraGo.transform.position = cameraGo.transform.position.AddZ(-fixedCameraComp.ZSpeed * UnityEngine.Time.deltaTime);
                    if (cameraGo.transform.position.z <= fixedCameraComp.ZMin)
                        fixedCameraComp.ZPositive = true;
                }
            }
        }
    }
}
