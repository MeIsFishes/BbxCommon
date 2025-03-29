using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using BbxCommon;
using BbxCommon.Ui;

public class UiMapRoomView : UiViewBase
{
    public Button Enter;
    public override Type GetControllerType()
    {
        return typeof(UiMapRoomController);
    }
}
