using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiTipController : UiControllerBase<UiTipView>
    {
        private struct TipInfo
        {
            public string Title;
            public string Description;

            public TipInfo(string title, string description)
            {
                Title = title;
                Description = description;
            }
        }

        private Queue<TipInfo> m_TipInfos = new();

        public void ShowTip(string title, string description)
        {
            m_TipInfos.Enqueue(new TipInfo(title, description));
        }

        protected override void OnUiInit()
        {
            m_View.FinishButton.onClick.AddListener(OnFinishButton);
            m_View.TweenGroup.Wrapper.OnPlayReverseFinishes += () => { Hide(); };
        }

        protected override void OnUiShow()
        {
            m_View.TweenGroup.Wrapper.Play();
        }

        protected override void OnUiUpdate(float deltaTime)
        {
            if (m_TipInfos.Count > 0 && m_Shown == false)
            {
                var tipInfo = m_TipInfos.Dequeue();
                m_View.Title.text = tipInfo.Title;
                m_View.Description.text = tipInfo.Description;
                Show();
            }
        }

        protected override void OnUiClose()
        {
            m_TipInfos.Clear();
        }

        private void OnFinishButton()
        {
            m_View.TweenGroup.Wrapper.PlayReverse();
        }
    }
}
