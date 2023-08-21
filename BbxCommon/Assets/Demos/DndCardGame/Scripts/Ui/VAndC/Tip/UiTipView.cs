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
    public class UiTipView : UiViewBase
    {
        public TMP_Text Title;
        public TMP_Text Description;
        public Button FinishButton;
        public UiTweenGroup TweenGroupOnShow;

        public override Type GetControllerType()
        {
            return typeof(UiTipController);
        }
    }
}
