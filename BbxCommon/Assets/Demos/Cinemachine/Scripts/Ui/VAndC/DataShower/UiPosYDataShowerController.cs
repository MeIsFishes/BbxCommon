using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public class UiPosYDataShowerController : UiDataShowerControllerBase<UiPosYDataShowerView>
    {
        public override void OnTargetInited()
        {
            var mainCameraComp = EcsApi.GetSingletonRawComponent<MainCameraSingletonRawComponent>();
            RegisterDataShowerListener(mainCameraComp.PosY);
        }
    }
}
