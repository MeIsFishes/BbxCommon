using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Cin
{
    [DisableAutoCreation]
    public partial class SyncCameraInfoSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            var mainCameraComp = EcsApi.GetSingletonRawComponent<MainCameraSingletonRawComponent>();
            if (mainCameraComp == null)
                return;
            var mainCameraGo = mainCameraComp.GetEntity().GetGameObject();
            if (mainCameraGo == null)
                return;

            mainCameraComp.PosX.SetValue(mainCameraGo.transform.position.x);
            mainCameraComp.PosY.SetValue(mainCameraGo.transform.position.y);
            mainCameraComp.PosZ.SetValue(mainCameraGo.transform.position.z);

            mainCameraComp.RotX.SetValue(mainCameraGo.transform.rotation.x);
            mainCameraComp.RotY.SetValue(mainCameraGo.transform.rotation.y);
            mainCameraComp.RotZ.SetValue(mainCameraGo.transform.rotation.z);
            mainCameraComp.RotW.SetValue(mainCameraGo.transform.rotation.w);
        }
    }
}
