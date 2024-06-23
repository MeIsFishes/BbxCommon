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
        /// ��һ������
        /// </summary>
        public void DrawDice()
        {
            // ����ƶ����Ѿ�û�����ӣ�������ƶ��������ϴ���ƶ�
            if (DicesInDeck.Count == 0)
            {
                DicesInDiscard.Shuffle();
                DicesInDeck.AddListFront(DicesInDiscard);
                DicesInDiscard.Clear();
                DispatchEvent(EUiEvent.DicesInDiscardRefresh);
                DispatchEvent(EUiEvent.DicesInDeckRefresh);
            }
            // ����û������
            if (DicesInDeck.Count == 0)
                return;
            // �����Ӷ����һ������
            DicesInHand.Add(DicesInDeck[DicesInDeck.Count - 1]);
            DispatchEvent(EUiEvent.DicesInHandRefresh);
            DicesInDeck.RemoveBack();
            DispatchEvent(EUiEvent.DicesInDeckRefresh);
        }

        /// <summary>
        /// �����ɸ�����
        /// </summary>
        public void DrawDice(int count)
        {
            for (int i = 0; i < count; i++)
            {
                DrawDice();
            }
        }

        /// <summary>
        /// ��һ������
        /// </summary>
        public void DiscardDice(int index)
        {
            DicesInDiscard.Add(DicesInHand[index]);
            DicesInHand.RemoveAt(index);
            DispatchEvent(EUiEvent.DicesInHandRefresh);
            DispatchEvent(EUiEvent.DicesInDiscardRefresh);
        }

        /// <summary>
        /// ��һ������
        /// </summary>
        public void TryDiscardDice(Dice dice)
        {
            int index = DicesInHand.IndexOf(dice);
            if (index != -1)
                DiscardDice(index);
        }

        /// <summary>
        /// ������������
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
