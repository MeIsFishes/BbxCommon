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
    public class UiTipListItemView : UiViewBase
    {
        public Button Button;
        public TMP_Text Title;

        public override Type GetControllerType()
        {
            return typeof(UiTipListItemController);
        }
    }
}
