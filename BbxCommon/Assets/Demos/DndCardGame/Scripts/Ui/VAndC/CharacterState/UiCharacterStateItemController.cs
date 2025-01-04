using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg;

public class UiCharacterStateItemController : UiControllerBase<UiCharacterStateItemView>
{
    Entity m_Entity;

    private ListenableItemListener m_MaxHpListener;
    private ListenableItemListener m_CurHpListener;

    protected override void OnUiInit()
    {
        // m_View.PlayerStateItemImage = GetImage();
        m_MaxHpListener = ModelWrapper.CreateVariableDirtyListener<int>(EControllerLifeCycle.Open, OnMaxHpDirty);
        m_CurHpListener = ModelWrapper.CreateVariableDirtyListener<int>(EControllerLifeCycle.Open, OnCurHpDirty);
    }

    public void SetEntity(Entity entity)
    {
        m_Entity = entity;
        var attributesComp = m_Entity.GetRawComponent<AttributesRawComponent>();
        m_MaxHpListener.RebindTarget(attributesComp.MaxHpVariable);
        m_CurHpListener.RebindTarget(attributesComp.CurHpVariable);
        ChangeHP(attributesComp.MaxHp, attributesComp.CurHp);
    }

    public void ChangeHP(int maxhp, int curhp)
    {
        m_View.PlayerStateItemHPTxt.text = $"HP: {curhp}/{maxhp}";
    }

    private void OnMaxHpDirty(int value)
    {
        ChangeHP(value, m_Entity.GetRawComponent<AttributesRawComponent>().CurHp);
    }

    private void OnCurHpDirty(int value)
    {
        ChangeHP(m_Entity.GetRawComponent<AttributesRawComponent>().MaxHp, value);
    }
}
