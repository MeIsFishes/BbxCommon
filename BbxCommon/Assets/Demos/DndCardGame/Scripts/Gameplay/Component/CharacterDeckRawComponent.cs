using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class CharacterDeckRawComponent : EcsRawComponent, IUiModelItem
    {
        public enum EUiEvent
        {
            DeckRefresh,
        }

        public List<UiModelVariable<Dice>> Dices = new();

        private MessageHandler<int> m_MessageHandler = new();
        IMessageDispatcher<int> IUiModelItem.MessageDispatcher => m_MessageHandler;

        public void AddDice(Dice dice)
        {
            var diceVar = UiModelVariable<Dice>.Create(dice);
            Dices.Add(diceVar);
            m_MessageHandler.Dispatch((int)EUiEvent.DeckRefresh);
        }

        public override void OnCollect()
        {
            m_MessageHandler.ClearAndRelease();
            Dices.CollectAndClearElements();
        }
    }
}
