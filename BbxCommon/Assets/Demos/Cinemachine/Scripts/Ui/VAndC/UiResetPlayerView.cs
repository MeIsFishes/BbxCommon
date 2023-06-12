using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BbxCommon;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public class UiResetPlayerView : UiViewBase
    {
        public Button Button;

        public override Type GetControllerType()
        {
            return typeof(UiResetPlayerController);
        }
    }
}
