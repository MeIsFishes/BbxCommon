using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDiceDisplayItemController : UiControllerBase<UiDiceDisplayItemView>
    {
        private UiDiceController m_DiceController;

        public void Bind(Dice dice)
        {
            if (m_DiceController == null)
                m_DiceController = UiApi.OpenUiController<UiDiceController>(m_View.transform);

            m_DiceController.Bind(dice);
            m_DiceController.InitDisplay(m_View.Scale);
            m_DiceController.Show();
        }
    }
}
