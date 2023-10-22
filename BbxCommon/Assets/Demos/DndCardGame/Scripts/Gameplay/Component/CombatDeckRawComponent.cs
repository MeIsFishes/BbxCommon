using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class CombatDeckRawComponent : EcsRawComponent, IUiModelItem
    {
        public enum EUiEvent
        {
            DicesInDeckRefresh,
            DicesInHandRefresh,
            DicesInDiscardRefresh,
        }

        public List<Dice> DicesInDeck;
        public List<Dice> DicesInHand;
        public List<Dice> DicesInDiscard;

        private MessageHandler<int> m_MessageHandler;
        IMessageDispatcher<int> IUiModelItem.MessageDispatcher => m_MessageHandler;

        public void DispatchEvent(EUiEvent e)
        {
            m_MessageHandler.Dispatch((int)e);
        }

        public void DrawDice()
        {
            // 如果牌堆里已经没有骰子，则把弃牌堆里的骰子洗进牌堆
            if (DicesInDeck.Count == 0)
            {
                DicesInDiscard.Shuffle();
                DicesInDeck.AddListFront(DicesInDiscard);
                DicesInDiscard.Clear();
                DispatchEvent(EUiEvent.DicesInDiscardRefresh);
                DispatchEvent(EUiEvent.DicesInDeckRefresh);
            }
            // 还是没有骰子
            if (DicesInDeck.Count == 0)
                return;
            // 从骰子堆里抽一个骰子
            DicesInHand.Add(DicesInDeck[DicesInDeck.Count - 1]);
            DispatchEvent(EUiEvent.DicesInHandRefresh);
            DicesInDeck.RemoveBack();
            DispatchEvent(EUiEvent.DicesInDeckRefresh);
        }

        public void DrawDice(int count)
        {
            for (int i = 0; i < count; i++)
            {
                DrawDice();
            }
        }

        public override void OnAllocate()
        {
            ObjectPool.Alloc(out m_MessageHandler);
            SimplePool.Alloc(out DicesInDeck);
            SimplePool.Alloc(out DicesInHand);
            SimplePool.Alloc(out DicesInDiscard);
        }

        public override void OnCollect()
        {
            m_MessageHandler.ClearAndRelease();
            DicesInDeck.CollectAndClearElements();
            DicesInHand.CollectAndClearElements();
            DicesInDiscard.CollectAndClearElements();
        }
    }
}
