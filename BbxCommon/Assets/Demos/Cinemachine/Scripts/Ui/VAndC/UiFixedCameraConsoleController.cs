using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public class UiFixedCameraConsoleController : UiControllerBase<UiFixedCameraConsoleView>
    {
        protected override void InitUiModelListeners()
        {
            AddUiModelVariableListener(EControllerLifeCycle.Open, EcsApi.GetSingletonRawComponent<CameraDataSingletonRawComponent>().CurCamera, EUiModelVariableEvent.Dirty,
                (MessageDataBase messageData) =>
                {
                    var data = (UiModelVariableDirtyMessageData<Entity>)messageData;
                    ShowIfChooseCamera(data.CurValue);
                });
        }

        protected override void OnUiInit()
        {
            m_View.ToggleXMoveButton.onClick.AddListener(OnXMoveButton);
            m_View.ToggleZMoveButton.onClick.AddListener(OnZMoveButton);
        }

        protected override void OnUiOpen()
        {
            var cameraEntity = EcsApi.GetSingletonRawComponent<CameraDataSingletonRawComponent>().CurCamera.Value;
            ShowIfChooseCamera(cameraEntity);
        }

        private void ShowIfChooseCamera(Entity cameraEntity)
        {
            if (cameraEntity.HasRawComponent<FixedCameraSingletonRawComponent>())
                this.Show();
            else
                this.Hide();
        }

        private void OnXMoveButton()
        {
            var fixedCameraComp = EcsApi.GetSingletonRawComponent<FixedCameraSingletonRawComponent>();
            fixedCameraComp.XMoveEnabled = !fixedCameraComp.XMoveEnabled;
        }

        private void OnZMoveButton()
        {
            var fixedCameraComp = EcsApi.GetSingletonRawComponent<FixedCameraSingletonRawComponent>();
            fixedCameraComp.ZMoveEnabled = !fixedCameraComp.ZMoveEnabled;
        }
    }
}
