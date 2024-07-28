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

        Coroutine m_Coroutine;

        private void ShowTip(DamageTipData data)
        {
            if(m_Coroutine != null)
            {
                StopCoroutine(m_Coroutine);
            }
            m_Data = data;
            Show();
            m_IsShowing=true;
            m_View.Title.text = m_Data.DamageValue.ToString();
            m_Coroutine =  StartCoroutine(HideTip());
        }

        protected override void OnHudBind(Entity entity)
        {
            base.OnHudBind(entity);
            var attributesComp = entity.GetRawComponent<AttributesRawComponent>();
            attributesComp.BindTakeDamage(OnDamageOccured, null);

        }

        public void OnDamageOccured(int damge)
        {
            ShowTip(new DamageTipData
            {
                DamageType = DamageType.None,
                DamageValue = damge,
                DurationTime = 1
            }) ;
        }

        public void UpdatePos(Vector3 pos)
        {
            transform.localPosition = pos;
        }

        protected override void OnHudDestroy()
        {
            base.OnHudDestroy();
            var attributesComp = Entity.GetRawComponent<AttributesRawComponent>();
            attributesComp.UnbindTakeDamage(OnDamageOccured, null);
        }

        IEnumerator HideTip()
        {
            yield return new WaitForSeconds(m_Data.DurationTime);
            Hide();
            m_IsShowing = false;
        }

    }
}

