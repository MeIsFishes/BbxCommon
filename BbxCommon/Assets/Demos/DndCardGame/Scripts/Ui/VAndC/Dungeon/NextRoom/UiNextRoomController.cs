using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiNextRoomController : UiControllerBase<UiNextRoomView>
    {
        protected override void OnUiInit()
        {
            m_View.Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var operationComp = EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>();
            var operation = ObjectPool<OperationGoToNextRoom>.Alloc();
            operationComp.AddBlockedOperation(operation);
        }
    }
}
