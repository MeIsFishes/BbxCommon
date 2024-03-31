using System;
using BbxCommon.Ui;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dcg.Ui
{
    public class UiGameStartView : UiViewBase
    {
        public Image BackGround;
        public RectTransform Rect;
        public Button Button;
        public override Type GetControllerType()
        {
            return typeof(UiGameStartController);
        }
    }
}