using System;
using System.Collections.Generic;
using UnityEngine;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public class UiEnableNoMoveController : UiControllerBase<UiEnableNoMoveView>
    {
        protected override void OnUiInit()
        {
            m_View.Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var camera = EcsApi.GetSingletonRawComponent<NoMoveCameraSingletonRawComponent>().GetEntity();
            EcsApi.GetSingletonRawComponent<CameraDataSingletonRawComponent>().SetActiveCamera(camera);
        }
    }
}
