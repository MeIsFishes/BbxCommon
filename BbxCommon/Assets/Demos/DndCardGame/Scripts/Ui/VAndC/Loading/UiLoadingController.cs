using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

public class UiLoadingController : UiControllerBase<UiLoadingView>, IUiLoadingController
{
    protected override void OnUiInit()
    {
        base.OnUiInit();
        m_View.LoadingProgress.value = 0;
        Hide();
    }
    

    public void OnLoading(float process)
    {
        m_View.LoadingProgress.value += process;
    }

    public void SetVisible(bool v)
    {
        if (v)
        {
            Show();
        }
        else
        {
            Hide();
        }
        
    }
    
}
