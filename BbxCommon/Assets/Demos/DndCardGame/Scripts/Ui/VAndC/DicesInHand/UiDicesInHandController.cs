using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiDicesInHandController : UiControllerBase<UiDicesInHandView>
    {
        private Entity CharacterEntity;

        /// <summary>
        /// 绑定到对应的角色entity，然后应用该角色的手牌信息
        /// </summary>
        public void Bind(Entity characterEntity)
        {
            CharacterEntity = characterEntity;
            RefreshDices();
        }

        private void RefreshDices()
        {
            var deckComp = CharacterEntity.GetRawComponent<CombatDeckRawComponent>();
            m_View.DicesList.ClearItems();
            foreach (var dice in deckComp.DicesInHand)
            {
                var diceController = m_View.DicesList.CreateItem<UiDiceController>();
                diceController.Bind(dice);
            }
        }
    }
}
