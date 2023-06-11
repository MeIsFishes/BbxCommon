using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BbxCommon.Ui;

namespace Cin.Ui
{
    public class UiEnableNoMoveView : UiViewBase
    {
        public Button Button; 

        public override Type GetControllerType()
        {
            return typeof(UiEnableNoMoveController);
        }
    }
}
