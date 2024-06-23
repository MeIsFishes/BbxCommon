using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class HudCharacterStatusController : HudControllerBase<HudCharacterStatusView>
    {
        private ListenableItemListener m_MaxHpListener;
        private ListenableItemListener m_CurHpListener;

        protected override void OnHudInit()
        {
            m_MaxHpListener = ModelWrapper.CreateVariableDirtyListener<int>(EControllerLifeCycle.Open, OnMaxHpDirty);
            m_CurHpListener = ModelWrapper.CreateVariableDirtyListener<int>(EControllerLifeCycle.Open, OnCurHpDirty);
        }

        protected override void OnHudBind(Entity entity)
        {
            var attributesComp = entity.GetRawComponent<AttributesRawComponent>();
            RefreshHpInfo(attributesComp.MaxHp, attributesComp.CurHp);
            m_MaxHpListener.RebindTarget(attributesComp.MaxHpVariable);
            m_CurHpListener.RebindTarget(attributesComp.CurHpVariable);
        }

        private void OnMaxHpDirty(int value)
        {
            RefreshHpInfo(value, Entity.GetRawComponent<AttributesRawComponent>().CurHp);
        }

        private void OnCurHpDirty(int value)
        {
            RefreshHpInfo(Entity.GetRawComponent<AttributesRawComponent>().MaxHp, value);
        }

        private void RefreshHpInfo(int maxHp, int curHp)
        {
            m_View.HpText.text = curHp.ToString() + " / " + maxHp.ToString();
            m_View.HpFill.fillAmount = (float)curHp / maxHp;
        }
    }
}
