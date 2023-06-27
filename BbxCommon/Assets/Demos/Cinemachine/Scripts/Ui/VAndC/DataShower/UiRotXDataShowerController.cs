using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public class UiRotXDataShowerController : UiDataShowerControllerBase<UiRotXDataShowerView>
    {
        public override void OnTargetInited()
        {
            var mainCameraComp = EcsApi.GetSingletonRawComponent<MainCameraSingletonRawComponent>();
            RegisterDataShowerListener(mainCameraComp.RotX);
        }
    }
}
