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
    public class UiWildDiceSlotView : UiViewBase
    {
        public Vector2 DiceOffset;
        public Vector2 DiceScale;

        public override Type GetControllerType()
        {
            return typeof(UiWildDiceSlotController);
        }
    }
}
