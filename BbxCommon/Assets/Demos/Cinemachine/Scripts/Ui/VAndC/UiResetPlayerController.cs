using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public class UiResetPlayerController : UiControllerBase<UiResetPlayerView>
    {
        protected override void OnUiInit()
        {
            m_View.Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var playerComp = EcsApi.GetSingletonRawComponent<PlayerSingletonRawComponent>();
            var characterController = playerComp.GetEntity().GetGameObject().GetComponent<CharacterController>();
            characterController.enabled = false;
            characterController.transform.position = playerComp.SpawnPosition;
            characterController.enabled = true;
        }
    }
}
