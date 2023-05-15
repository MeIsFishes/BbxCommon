using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public class AttributesRawComponent : EcsRawComponent
    {
        /// <summary>
        /// 力量
        /// </summary>
        public int Strength;
        /// <summary>
        /// 体质
        /// </summary>
        public int Constitution;
        /// <summary>
        /// 敏捷
        /// </summary>
        public int Dexterity;
        /// <summary>
        /// 智力
        /// </summary>
        public int Intelligence;
        /// <summary>
        /// 感知
        /// </summary>
        public int Wisdom;

        public void GetModifierDice(int attributeValue, List<Dice> res)
        {
            while (attributeValue > 0)
            {
                var value = (attributeValue - 1) % 5 + 1;
                attributeValue -= 5;
                var dice = ObjectPool<Dice>.Alloc();
                switch (value)
                {
                    case 1:
                        dice.DiceType = EDiceType.D4;
                        break;
                    case 2:
                        dice.DiceType = EDiceType.D6;
                        break;
                    case 3:
                        dice.DiceType = EDiceType.D8;
                        break;
                    case 4:
                        dice.DiceType = EDiceType.D10;
                        break;
                    case 5:
                        dice.DiceType = EDiceType.D12;
                        break;
                }
                res.Add(dice);
            }
        }
    }
}
