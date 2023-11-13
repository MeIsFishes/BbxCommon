using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDicesInHandController : UiControllerBase<UiDicesInHandView>
    {
        /// <summary>
        /// 绑定到对应的角色entity，然后应用该角色的手牌信息
        /// </summary>
        public void Bind(Entity characterEntity)
        {
            var combatDeckComp = characterEntity.GetRawComponent<CombatDeckRawComponent>();
            AddUiModelListener(EControllerLifeCycle.Open, combatDeckComp, (int)CombatDeckRawComponent.EUiEvent.DicesInHandRefresh, RefreshDices);
            RefreshDices(combatDeckComp);
        }

        private void RefreshDices(MessageDataBase messageData)
        {
            RefreshDices(messageData.GetData<CombatDeckRawComponent>());
        }

        /// <summary>
        /// 采用dirty时暴力重新初始化的做法，后面需要优化
        /// </summary>
        private void RefreshDices(CombatDeckRawComponent combatDeckComp)
        {
            for (int i = 0; i < m_View.DicesList.ItemWrapper.Count; i++)
            {
                m_View.DicesList.ItemWrapper.GetItem<UiDicesInHandItemController>(i).Uninit();
            }
            m_View.DicesList.ItemWrapper.ClearItems();
            for (int i = 0; i < combatDeckComp.DicesInHand.Count; i++)
            {
                var dice = combatDeckComp.DicesInHand[i];
                var diceController = m_View.DicesList.ItemWrapper.AddItem<UiDicesInHandItemController>();
                diceController.Init(i, combatDeckComp);
                diceController.Bind(dice);
            }
        }
    }
}
