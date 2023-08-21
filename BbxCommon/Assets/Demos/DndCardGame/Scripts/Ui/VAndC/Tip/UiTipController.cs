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
        public void ShowTip(string title, string description)
        {
            m_View.Title.text = title;
            m_View.Description.text = description;
            Show();
        }

        protected override void OnUiInit()
        {
            m_View.FinishButton.onClick.AddListener(OnFinishButton);
        }

        protected override void OnUiShow()
        {
            m_View.TweenGroupOnShow.Play();
        }

        private void OnFinishButton()
        {
            Hide();
        }
    }
}
