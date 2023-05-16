using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public class DiceList : PooledObject
    {
        public List<Dice> Dices = new();

        public static DiceList Create()
        {
            var diceList = ObjectPool<DiceList>.Alloc();
            return diceList;
        }

        public int GetRollingRes()
        {
            int res = 0;
            foreach (var dice in Dices)
            {
                res += dice.RollOnce();
            }
            return res;
        }
    }
}
