using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

public class UiShowAttackResultsController : UiControllerBase<UiShowAttackResultsView>
{
    private UiModelUserOption m_ModelUserOption;
    protected override void OnUiInit()
    {
        m_View.ClosePromptToggle.onValueChanged.AddListener(OnToggleClick);

        m_ModelUserOption = UiApi.GetUiModel<UiModelUserOption>();
        m_ModelUserOption.LoadUserOption();
        m_View.ClosePromptToggle.SetIsOnWithoutNotify(m_ModelUserOption.TipsNeedShow);
    }

    private void OnToggleClick(bool value)
    {
        m_ModelUserOption.TipsNeedShow = value;
    }
}
