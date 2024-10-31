using BbxCommon;
using BbxCommon.Ui;
using Unity.Entities;
using UnityEngine;


namespace Dcg.Ui
{
    public class HudDamageTweenTipController : HudControllerBase<HudDamageTweenTipView>
    {
        public struct DamageTipData
        {
            public int DamageValue;
            public DamageType DamageType;
            public Color TextColor;
        }

        public void SetViewData(int damageValue, Color color)
        {
            if (damageValue == 0)
                m_View.Text.text = "miss";
            else
                m_View.Text.text = damageValue.ToString();
            m_View.Text.color = color;
        }

        protected override void OnHudInit()
        {
            m_View.TweenGroup.OnPlayingFinishes += () => { Close(); };
        }

        protected override void OnHudOpen()
        {
            m_View.TweenGroup.Play();
        }
    }
}

