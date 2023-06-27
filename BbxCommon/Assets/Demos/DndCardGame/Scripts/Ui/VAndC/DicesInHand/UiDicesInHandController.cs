using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDicesInHandController : UiControllerBase<UiDicesInHandView>
    {
        private Entity m_CharacterEntity;
        private ObjRef<CombatDeckRawComponent> m_CombatDeckComp;

        /// <summary>
        /// 绑定到对应的角色entity，然后应用该角色的手牌信息
        /// </summary>
        public void Bind(Entity characterEntity)
        {
            m_CharacterEntity = characterEntity;
            m_CombatDeckComp = characterEntity.GetRawComponent<CombatDeckRawComponent>().AsObjRef();
            AddUiModelListener(EControllerLifeCycle.Open, m_CombatDeckComp.Obj, (int)CombatDeckRawComponent.EUiEvent.DicesInHandRefresh, RefreshDices);
            RefreshDices();
        }

        private void RefreshDices(MessageDataBase messageData = null)
        {
            if (m_CombatDeckComp.IsNull())
            {
                Debug.LogError("You are visiting a collected CombatDeckComponent reference!");
                return;
            }

            var deckComp = m_CombatDeckComp.Obj;
            m_View.DicesList.ClearItems();
            foreach (var dice in deckComp.DicesInHand)
            {
                var diceController = m_View.DicesList.CreateItem<UiDiceController>();
                diceController.Bind(dice.Value);
            }
        }
    }
}
