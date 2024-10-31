using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class HudCommonStatusController : HudControllerBase<HudCommonStatusView>
    {
        private ListenableItemListener m_DamageRequestListener;

        protected override void OnHudInit()
        {
            m_DamageRequestListener = ModelWrapper.CreateListener(EControllerLifeCycle.Open, AttackableRawComponent.EEvent.DamageRequestProcessed, OnTakeDamage);
        }

        protected override void OnHudBind(Entity entity)
        {
            base.OnHudBind(entity);
            var attackComp = entity.GetRawComponent<AttackableRawComponent>();
            m_DamageRequestListener.RebindTarget(attackComp);
        }

        public void OnTakeDamage(MessageData message)
        {
            var damage = message.GetData<DamageRequest>();
            var damageController = UiApi.OpenUiController<HudDamageTweenTipController>(m_View.transform);
            damageController.Show();
            damageController.SetViewData(damage.Damage, Color.red);
        }
    }
}
