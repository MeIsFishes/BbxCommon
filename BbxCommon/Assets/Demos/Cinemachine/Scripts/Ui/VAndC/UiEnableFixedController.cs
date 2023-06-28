using System;
using System.Collections.Generic;
using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public class UiEnableFixedController : UiControllerBase<UiEnableFixedView>
    {
        protected override void OnUiInit()
        {
            m_View.Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var camera = EcsApi.GetSingletonRawComponent<FixedCameraSingletonRawComponent>().GetEntity();
            EcsApi.GetSingletonRawComponent<CameraDataSingletonRawComponent>().SetActiveCamera(camera);
        }
    }
}
