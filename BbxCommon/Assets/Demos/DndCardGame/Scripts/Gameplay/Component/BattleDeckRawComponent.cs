using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public class BattleDeckRawComponent : EcsRawComponent
    {
        public List<Dice> DicesInDeck = new();
        public List<Dice> DicesInHand = new();
        public List<Dice> DicesInDiscard = new();

        public bool DrawDice()
        {
            // �����ƶ��������ϴ���ƶ�
            if (DicesInDeck.Count == 0)
            {
                DicesInDiscard.Shuffle();
                DicesInDeck.AddFront(DicesInDiscard);
                DicesInDiscard.Clear();
            }
            // ����û������
            if (DicesInDeck.Count == 0)
                return false;
            // �����Ӷ����һ������
            DicesInHand.Add(DicesInDeck[DicesInDeck.Count - 1]);
            DicesInDeck.RemoveBack();
            return true;
        }

        public override void OnCollect()
        {
            DicesInDeck.Clear();
            DicesInHand.Clear();
            DicesInDiscard.Clear();
        }
    }
}
