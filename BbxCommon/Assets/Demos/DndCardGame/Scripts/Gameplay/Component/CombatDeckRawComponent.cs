using System.Collections.Generic;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    public class CombatDeckRawComponent : EcsRawComponent, IListenable
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
        IMessageDispatcher<int> IListenable.MessageDispatcher => m_MessageHandler;

        public void DispatchEvent(EUiEvent e)
        {
            m_MessageHandler.Dispatch((int)e, this);
        }

        /// <summary>
        /// 抽一个骰子
        /// </summary>
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

        /// <summary>
        /// 抽若干个骰子
        /// </summary>
        public void DrawDice(int count)
        {
            for (int i = 0; i < count; i++)
            {
                DrawDice();
            }
        }

        /// <summary>
        /// 弃一个骰子
        /// </summary>
        public void DiscardDice(int index)
        {
            DicesInDiscard.Add(DicesInHand[index]);
            DicesInHand.RemoveAt(index);
            DispatchEvent(EUiEvent.DicesInHandRefresh);
            DispatchEvent(EUiEvent.DicesInDiscardRefresh);
        }

        /// <summary>
        /// 弃一个骰子
        /// </summary>
        public void TryDiscardDice(Dice dice)
        {
            int index = DicesInHand.IndexOf(dice);
            if (index != -1)
                DiscardDice(index);
        }

        /// <summary>
        /// 弃掉所有手牌
        /// </summary>
        public void DiscardAllHandDices()
        {
            DicesInDiscard.AddList(DicesInHand);
            DicesInHand.Clear();
            DispatchEvent(EUiEvent.DicesInHandRefresh);
            DispatchEvent(EUiEvent.DicesInDiscardRefresh);
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
