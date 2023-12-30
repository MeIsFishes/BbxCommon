using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDicesInHandController : UiControllerBase<UiDicesInHandView>
    {
        private ModelListener m_DicesInHandRefreshListener;

        protected override void OnUiInit()
        {
            m_DicesInHandRefreshListener = ModelWrapper.CreateListener(EControllerLifeCycle.Open, (int)CombatDeckRawComponent.EUiEvent.DicesInHandRefresh, RefreshDices);
        }

        protected override void OnUiClose()
        {
            for (int i = 0; i < m_View.DicesList.ItemWrapper.Count; i++)
            {
                m_View.DicesList.ItemWrapper.GetItem<UiDicesInHandItemController>(i).Uninit();
            }
            m_View.DicesList.ClearItems();
        }

        /// <summary>
        /// �󶨵���Ӧ�Ľ�ɫentity��Ȼ��Ӧ�øý�ɫ��������Ϣ
        /// </summary>
        public void Bind(Entity characterEntity)
        {
            var combatDeckComp = characterEntity.GetRawComponent<CombatDeckRawComponent>();
            m_DicesInHandRefreshListener.RebindModelItem(combatDeckComp);
            RefreshDices(combatDeckComp);
        }

        private void RefreshDices(MessageData messageData)
        {
            RefreshDices(messageData.GetData<CombatDeckRawComponent>());
        }

        /// <summary>
        /// ����dirtyʱ�������³�ʼ����������������Ҫ�Ż�
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
