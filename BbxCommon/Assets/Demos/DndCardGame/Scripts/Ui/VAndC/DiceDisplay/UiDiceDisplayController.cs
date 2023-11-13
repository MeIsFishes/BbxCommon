using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    /// <summary>
    /// 用来展示骰子，比如弃牌堆、牌堆一览等。目前选择把它们都做成同一个UI。
    /// </summary>
    public class UiDiceDisplayController : UiControllerBase<UiDiceDisplayView>
    {
        public List<Dice> Dices;

        protected override void OnUiInit()
        {
            m_View.CloseButton.onClick.AddListener(ClosePage);
            SimplePool.Alloc(out Dices);
        }

        protected override void OnUiDestroy()
        {
            Dices.CollectAndClearElements(true);
        }

        public void Display(string title, List<Dice> dices)
        {
            m_View.Title.text = title;
            Dices.Clear();
            Dices.AddList(dices);
            m_View.DiceList.ItemWrapper.ClearItems();
            for (int i = 0; i < Dices.Count; i++)
            {
                var itemController = m_View.DiceList.ItemWrapper.AddItem<UiDiceDisplayItemController>();
                itemController.Bind(Dices[i]);
            }
            ControllerWrapper.Show();
        }

        private void ClosePage()
        {
            ControllerWrapper.Hide();
        }
    }
}
