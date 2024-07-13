using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    /// <summary>
    /// 记录玩家持有的骰子卡组信息，在局外一直存在
    /// </summary>
    public class CharacterDeckRawComponent : EcsRawComponent, IListenable
    {
        public enum EUiEvent
        {
            DeckRefresh,
        }

        public List<Dice> Dices = new();

        private MessageHandler<int> m_MessageHandler = new();
        IMessageDispatcher<int> IListenable.MessageDispatcher => m_MessageHandler;

        public void DispatchEvent(EUiEvent uiEvent)
        {
            m_MessageHandler.Dispatch((int)uiEvent);
        }

        public void AddDice(Dice dice)
        {
            Dices.Add(dice);
            m_MessageHandler.Dispatch((int)EUiEvent.DeckRefresh);
        }

        public override void OnCollect()
        {
            m_MessageHandler.ClearAndRelease();
            Dices.CollectAndClearElements();
        }
    }
}
