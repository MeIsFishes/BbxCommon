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
        private ListenableItemListener m_TipsVisibleListener;

        public void ShowTip(string title, string description)
        {
            m_TipInfos.Enqueue(new TipInfo(title, description));
        }

        protected override void OnUiInit()
        {
            m_View.FinishButton.onClick.AddListener(OnFinishButton);
            m_View.TweenGroup.Wrapper.OnPlayReverseFinishes += () => { Hide(); };
            m_TipsVisibleListener = ModelWrapper.CreateVariableDirtyListener<bool>(EControllerLifeCycle.Open, OnVisibleChange);
            var modelUserOption = UiApi.GetUiModel<UiModelUserOption>();
            m_TipsVisibleListener.RebindTarget(modelUserOption.TipsNeedShowVariable);
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

        private void OnVisibleChange(bool value)
        {
            if (!value)
            {
                ClearTips();
            }
        }

        public void ClearTips()
        {
            m_TipInfos.Clear();
            Hide();
        }
    }
}
