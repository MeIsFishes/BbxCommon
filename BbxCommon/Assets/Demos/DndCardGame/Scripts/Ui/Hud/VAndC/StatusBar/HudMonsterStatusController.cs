using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;
using Unity.Entities;

namespace Dcg.Ui
{
    public class HudMonsterStatusController : HudControllerBase<HudMonsterStatusView>
    {
        protected override void OnHudBind(Entity entity)
        {
            var attributesComp = entity.GetRawComponent<AttributesRawComponent>();
            RefreshHpInfo(attributesComp.MaxHp, attributesComp.CurHp);
            AddUiModelVariableListener(EControllerLifeCycle.Open, attributesComp.MaxHpVariable, EUiModelVariableEvent.Dirty, OnMaxHpDirty);
            AddUiModelVariableListener(EControllerLifeCycle.Open, attributesComp.CurHpVariable, EUiModelVariableEvent.Dirty, OnCurHpDirty);
        }

        private void OnMaxHpDirty(MessageDataBase messageData)
        {
            if (messageData is UiModelVariableDirtyMessageData<int> data)
            {
                RefreshHpInfo(data.CurValue, Entity.GetRawComponent<AttributesRawComponent>().CurHp);
            }
        }

        private void OnCurHpDirty(MessageDataBase messageData)
        {
            if (messageData is UiModelVariableDirtyMessageData<int> data)
            {
                RefreshHpInfo(Entity.GetRawComponent<AttributesRawComponent>().MaxHp, data.CurValue);
            }
        }

        private void RefreshHpInfo(int maxHp, int curHp)
        {
            m_View.HpText.text = curHp.ToString() + " / " + maxHp.ToString();
            m_View.HpFill.fillAmount = (float)curHp / maxHp;
        }
    }
}
