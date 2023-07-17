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
    public class UiPromptView : UiViewBase
    {
        public UiTweenGroup TweenGroup;
        public TMP_Text Text;

        public override Type GetControllerType()
        {
            return typeof(UiPromptController);
        }
    }
}
