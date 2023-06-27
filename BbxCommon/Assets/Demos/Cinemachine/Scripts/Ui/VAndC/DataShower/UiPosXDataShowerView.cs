using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public class UiPosXDataShowerView : UiDataShowerViewBase
    {
        public override Type GetControllerType()
        {
            return typeof(UiPosXDataShowerController);
        }
    }
}
