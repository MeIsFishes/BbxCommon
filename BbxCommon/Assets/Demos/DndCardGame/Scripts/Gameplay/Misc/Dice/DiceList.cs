using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public class DiceList : PooledObject
    {
        public List<Dice> Dices = new();

        private bool m_Rolled;
        private DiceListResult m_Result;

        public static DiceList Create()
        {
            var diceList = ObjectPool<DiceList>.Alloc();
            return diceList;
        }

        public static DiceList Create(List<Dice> dices)
        {
            var diceList = ObjectPool<DiceList>.Alloc();
            diceList.Dices.AddList(dices);
            return diceList;
        }

        public static DiceList Create(params Dice[] dices)
        {
            var diceList = ObjectPool<DiceList>.Alloc();
            diceList.Dices.AddArray(dices);
            return diceList;
        }

        public static DiceList Create(params EDiceType[] dices)
        {
            var diceList = ObjectPool<DiceList>.Alloc();
            foreach (var type in dices)
            {
                diceList.Dices.Add(Dice.Create(type));
            }
            return diceList;
        }

        public DiceListResult GetListResult()
        {
            if (m_Rolled == false)
            {
                m_Result = new DiceListResult(this);
                m_Rolled = true;
            }
            return m_Result;
        }

        protected override void OnCollect()
        {
            Dices.CollectAndClearElements(true);
            m_Rolled = false;
        }
    }

    public struct DiceListResult
    {
        public int Amount;
        public List<int> DiceResults;

        public DiceListResult(DiceList diceList)
        {
            Amount = 0;
            DiceResults = SimplePool<List<int>>.Alloc();

            foreach (var dice in diceList.Dices)
            {
                var result = dice.RollOnce();
                DiceResults.Add(result);
                Amount += result;
            }
        }
    }
}
