using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiPromptController : UiControllerBase<UiPromptView>
    {
        private string m_CurPrompt;

        protected override void OnUiUpdate(float deltaTime)
        {
            if (m_View.TweenGroup.Finished)
                Hide();
        }

        protected override void OnUiShow()
        {
            m_View.Text.text = m_CurPrompt;
            m_View.TweenGroup.Play();
        }

        public void ShowPrompt(string prompt)
        {
            if (m_Shown)
                return;

            m_CurPrompt = prompt;
            Show();
        }
    }
}
