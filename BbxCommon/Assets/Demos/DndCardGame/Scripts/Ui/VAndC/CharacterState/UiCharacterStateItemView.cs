using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;
using BbxCommon;
using BbxCommon.Ui;

public class UiCharacterStateItemView : UiViewBase
{
    public Image PlayerStateItemImage;
    public TextMeshProUGUI PlayerStateItemHPTxt;

    public override Type GetControllerType()
    {
        return typeof(UiCharacterStateItemController);
    }
}
