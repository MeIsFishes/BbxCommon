using BbxCommon;
using BbxCommon.Ui;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


namespace Dcg.Ui
{
    public class HudDamageTweenTipController : HudControllerBase<HudDamageTweenTipView>
    {
        
        public struct DamageTipData
        {
            public int DamageValue;
            public DamageType DamageType;
            public Color TextColor;
            public float DurationTime;
        }

        DamageTipData m_Data;
        private bool m_IsShowing = false;
        public bool IsShowing
        {
            get
            {
                return m_IsShowing;
            }
        }

        private ListenableItemListener m_DamageRequestListener;


        Coroutine m_Coroutine;

        protected override void OnHudInit()
        {
            base.OnHudInit();
            m_DamageRequestListener = ModelWrapper.CreateListener(EControllerLifeCycle.Open, AttackableRawComponent.EEvent.DamageRequestProcessed, OnDamageOccured);
            Hide();
        }

        private void ShowTip(DamageTipData data)
        {
            if(m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }
            m_Data = data;
            Show();
            m_IsShowing = true;
            m_View.Title.text = m_Data.DamageValue.ToString();
            m_View.TweenAlpha.Duration = m_Data.DurationTime;
            m_View.TweenAlpha.Play();
            m_Coroutine =  StartCoroutine(HideTip());
        }

        protected override void OnHudBind(Entity entity)
        {
            base.OnHudBind(entity);
            var attackComp = entity.GetRawComponent<AttackableRawComponent>();
            m_DamageRequestListener.RebindTarget(attackComp);
        }

        public void OnDamageOccured(MessageData message)
        {
            var damage = message.GetData<DamageRequest>();
            ShowTip(new DamageTipData
            {
                DamageType = damage.DamageType,
                DamageValue = damage.Damage,
                DurationTime = 2
            });
        }

        public void UpdatePos(Vector3 pos)
        {
            transform.localPosition = pos;
        }

        IEnumerator HideTip()
        {
            yield return new WaitForSeconds(m_Data.DurationTime);
            Hide();
            m_IsShowing = false;
        }
    }
}

