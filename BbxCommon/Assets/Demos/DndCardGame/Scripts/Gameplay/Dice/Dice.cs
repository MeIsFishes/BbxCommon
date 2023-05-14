using System.Collections.Generic;
using BbxCommon;
using Random = UnityEngine.Random;

namespace Dcg
{
    public enum EDiceType
    {
        D4,
        D6,
        D8,
        D10,
        D12,
        D20,
    }

    public class Dice : PooledObject
    {
        public EDiceType DiceType;
        public List<DiceAffixBase> Affixes = new();

        public uint RollOnce()
        {
            uint rollRes = 1;
            switch (DiceType)   // switch没有直接return，做个预留，后面也许每次roll点需要做些其他事情
            {
                case EDiceType.D4:
                    rollRes = (uint)Random.Range(1, 4);
                    break;
                case EDiceType.D6:
                    rollRes = (uint)Random.Range(1, 6);
                    break;
                case EDiceType.D8:
                    rollRes = (uint)Random.Range(1, 8);
                    break;
                case EDiceType.D10:
                    rollRes = (uint)Random.Range(1, 10);
                    break;
                case EDiceType.D12:
                    rollRes = (uint)Random.Range(1, 12);
                    break;
                case EDiceType.D20:
                    rollRes = (uint)Random.Range(1, 20);
                    break;
            }
            return rollRes;
        }
    }

    public abstract class DiceAffixBase : PooledObject
    {

    }
}
