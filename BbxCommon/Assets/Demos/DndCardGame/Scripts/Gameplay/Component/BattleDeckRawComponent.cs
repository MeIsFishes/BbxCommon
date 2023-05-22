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
            // 把弃牌堆里的骰子洗进牌堆
            if (DicesInDeck.Count == 0)
            {
                DicesInDiscard.Shuffle();
                DicesInDeck.AddFront(DicesInDiscard);
                DicesInDiscard.Clear();
            }
            // 还是没有骰子
            if (DicesInDeck.Count == 0)
                return false;
            // 从骰子堆里抽一个骰子
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
