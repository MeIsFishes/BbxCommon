using System.Collections.Generic;
using UnityEngine;

namespace Dcg
{
    public class DiceGroup : MonoBehaviour
    {
        public List<Dice> Dices = new();

        public uint GetRollingRes()
        {
            uint res = 0;
            foreach (var dice in Dices)
            {
                res += dice.RollOnce();
            }
            return res;
        }
    }
}
