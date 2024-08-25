using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using BbxCommon;
using BbxCommon.Ui;

public class UiLoadingView : UiViewBase
{
    public Slider LoadingProgress;
    
    public override Type GetControllerType()
    {
        return typeof(UiLoadingController);
    }
}
