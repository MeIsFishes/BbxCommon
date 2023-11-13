using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDiceDisplayView : UiViewBase
    {
        public UiList DiceList;
        public TMP_Text Title;
        public Button CloseButton;

        public override Type GetControllerType()
        {
            return typeof(UiDiceDisplayController);
        }
    }
}
