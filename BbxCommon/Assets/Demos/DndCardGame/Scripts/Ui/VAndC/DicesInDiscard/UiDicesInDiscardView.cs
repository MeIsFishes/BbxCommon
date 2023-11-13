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
    public class UiDicesInDiscardView : UiViewBase
    {
        public UiEventListener EventListener;
        public TMP_Text NumberText;

        public override Type GetControllerType()
        {
            return typeof(UiDicesInDiscardController);
        }
    }
}
