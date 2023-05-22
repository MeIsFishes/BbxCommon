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

        public static Dice Create(EDiceType diceType)
        {
            var dice = ObjectPool<Dice>.Alloc();
            dice.DiceType = diceType;
            return dice;
        }

        public int RollOnce()
        {
            int rollRes = 1;
            switch (DiceType)   // switch没有直接return，做个预留，后面也许每次roll点需要做些其他事情
            {
                case EDiceType.D4:
                    rollRes = Random.Range(1, 4);
                    break;
                case EDiceType.D6:
                    rollRes = Random.Range(1, 6);
                    break;
                case EDiceType.D8:
                    rollRes = Random.Range(1, 8);
                    break;
                case EDiceType.D10:
                    rollRes = Random.Range(1, 10);
                    break;
                case EDiceType.D12:
                    rollRes = Random.Range(1, 12);
                    break;
                case EDiceType.D20:
                    rollRes = Random.Range(1, 20);
                    break;
            }
            return rollRes;
        }

        public string GetDiceTittle()
        {
            switch (DiceType)
            {
                case EDiceType.D4:
                    return "d4";
                case EDiceType.D6:
                    return "d6";
                case EDiceType.D8:
                    return "d8";
                case EDiceType.D10:
                    return "d10";
                case EDiceType.D12:
                    return "d12";
                case EDiceType.D20:
                    return "d20";
            }
            return null;
        }

        public override void OnCollect()
        {
            DiceType = EDiceType.D4;
            Affixes.Clear();
        }
    }

    public abstract class DiceAffixBase : PooledObject
    {

    }
}
