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

        public List<UiModelVariable<Dice>> DicesInDeck = new();
        public List<UiModelVariable<Dice>> DicesInHand = new();
        public List<UiModelVariable<Dice>> DicesInDiscard = new();

        private MessageHandler<int> m_MessageHandler = new();
        IMessageDispatcher<int> IUiModelItem.MessageDispatcher => m_MessageHandler;

        public bool DrawDice()
        {
            // �����ƶ��������ϴ���ƶ�
            if (DicesInDeck.Count == 0)
            {
                DicesInDiscard.Shuffle();
                DicesInDeck.AddFront(DicesInDiscard);
                DicesInDiscard.Clear();
                m_MessageHandler.Dispatch((int)EUiEvent.DicesInDiscardRefresh);
                m_MessageHandler.Dispatch((int)EUiEvent.DicesInDeckRefresh);
            }
            // ����û������
            if (DicesInDeck.Count == 0)
                return false;
            // �����Ӷ����һ������
            DicesInHand.Add(DicesInDeck[DicesInDeck.Count - 1]);
            m_MessageHandler.Dispatch((int)EUiEvent.DicesInHandRefresh);
            DicesInDeck.RemoveBack();
            m_MessageHandler.Dispatch((int)EUiEvent.DicesInDeckRefresh);
            return true;
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
