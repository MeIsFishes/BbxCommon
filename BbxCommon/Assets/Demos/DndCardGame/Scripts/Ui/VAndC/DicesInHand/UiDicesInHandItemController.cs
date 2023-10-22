using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDicesInHandItemController : UiControllerBase<UiDicesInHandItemView>
    {
        public int Index;

        private UiDiceController m_DiceController;

        protected override void OnUiInit()
        {
            m_DiceController = UiApi.OpenUiController<UiDiceController>(m_View.transform);
            m_DiceController.Show();
            m_DiceController.transform.localPosition = Vector3.zero;
        }

        public void Bind(Dice dice)
        {
            m_DiceController.Bind(dice);
        }
    }
}
