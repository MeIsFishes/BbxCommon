using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiEnemyTurnController : UiControllerBase<UiEnemyTurnView>
    {
        protected override void OnUiInit()
        {
            m_View.TweenGroup.Wrapper.OnPlayingFinishes += OnPlayingFinishes;
        }

        protected override void OnUiDestroy()
        {
            m_View.TweenGroup.Wrapper.OnPlayingFinishes -= OnPlayingFinishes;
        }

        protected override void OnUiShow()
        {
            m_View.TweenGroup.Wrapper.Play();
        }

        private void OnPlayingFinishes()
        {
            Hide();
        }
    }
}
