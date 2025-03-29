using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg;

public class UiMapRoomController : UiControllerBase<UiMapRoomView>
{
    protected override void OnUiInit()
    {
        m_View.Enter.onClick.AddListener(DcgGameEngine.Instance.EnterCombat);
    }
}
