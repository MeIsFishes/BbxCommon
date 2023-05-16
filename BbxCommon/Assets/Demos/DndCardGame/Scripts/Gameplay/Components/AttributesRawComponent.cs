using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    public enum EAttribute
    {
        /// <summary>
        /// 力量
        /// </summary>
        Strength,
        /// <summary>
        /// 敏捷
        /// </summary>
        Dexterity,
        /// <summary>
        /// 体质
        /// </summary>
        Consititution,
        /// <summary>
        /// 智力
        /// </summary>
        Intelligence,
        /// <summary>
        /// 感知
        /// </summary>
        Wisdom,
    }

    public class AttributesRawComponent : EcsRawComponent
    {
        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Intelligence;
        public int Wisdom;

        public void GetModifierDice(EAttribute attribute, List<Dice> res)
        {
            int attributeValue = 0;
            switch (attribute)
            {
                case EAttribute.Strength:
                    attributeValue = Strength;
                    break;
                case EAttribute.Dexterity:
                    attributeValue = Dexterity;
                    break;
                case EAttribute.Consititution:
                    attributeValue = Constitution;
                    break;
                case EAttribute.Intelligence:
                    attributeValue = Intelligence;
                    break;
                case EAttribute.Wisdom:
                    attributeValue = Wisdom;
                    break;
            }
            while (attributeValue > 0)
            {
                var value = (attributeValue - 1) % 5 + 1;
                attributeValue -= 5;
                switch (value)
                {
                    case 1:
                        res.Add(Dice.Create(EDiceType.D4));
                        break;
                    case 2:
                        res.Add(Dice.Create(EDiceType.D6));
                        break;
                    case 3:
                        res.Add(Dice.Create(EDiceType.D8));
                        break;
                    case 4:
                        res.Add(Dice.Create(EDiceType.D10));
                        break;
                    case 5:
                        res.Add(Dice.Create(EDiceType.D12));
                        break;
                }
            }
        }
    }
}
