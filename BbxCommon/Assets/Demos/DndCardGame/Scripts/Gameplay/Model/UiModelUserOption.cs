using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

public class UiModelUserOption : UiModelBase
{
    public ListenableVariable<bool> TipsNeedShowVariable = new();
    public bool TipsNeedShow
    {
        get
        {
            return TipsNeedShowVariable.Value;
        }
        set
        {
            TipsNeedShowVariable.SetValue(value);
            PlayerPrefs.SetInt("NeedShowTips", value ? 1 : 0);
        }
    }

    public void LoadUserOption()
    {
        TipsNeedShow = PlayerPrefs.GetInt("NeedShowTips", 0) == 1;
    }

    public override void OnCollect()
    {
        TipsNeedShowVariable.DispatchInvalid();
    }
}
