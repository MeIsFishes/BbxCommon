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
        [Tooltip("骰子拖入后会按该比例缩放")]
        public float DiceScale;
        [Tooltip("骰子会挂在该对象下面，以保证UiInteractor始终能在最上层进行交互")]
        public Transform DiceRoot;
        public UiInteractor UiInteractor;

        public override Type GetControllerType()
        {
            return typeof(UiWildDiceSlotController);
        }
    }
}
