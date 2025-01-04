using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiMapController : UiControllerBase<UiMapView>
    {
        protected override void OnUiInit()
        {
            m_View.Button.onClick.AddListener(DcgGameEngine.Instance.EnterCombat);
        }

    }
}
