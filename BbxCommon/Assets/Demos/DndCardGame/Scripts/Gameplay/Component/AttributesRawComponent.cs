using System.Collections.Generic;
using BbxCommon;

namespace Dcg
{
    /// <summary>
    /// ����������ԣ�DND�����������������Խ���ability
    /// </summary>
    public enum EAbility
    {
        /// <summary>
        /// ����
        /// </summary>
        Strength,
        /// <summary>
        /// ����
        /// </summary>
        Dexterity,
        /// <summary>
        /// ����
        /// </summary>
        Consititution,
        /// <summary>
        /// ����
        /// </summary>
        Intelligence,
        /// <summary>
        /// ��֪
        /// </summary>
        Wisdom,
    }

    public class AttributesRawComponent : EcsRawComponent
    {
        public int MaxHp;
        public int CurHp;

        public int Strength;
        public int Dexterity;
        public int Constitution;
        public int Intelligence;
        public int Wisdom;

        public void GetModifierDice(EAbility attribute, List<Dice> res)
        {
            int attributeValue = 0;
            switch (attribute)
            {
                case EAbility.Strength:
                    attributeValue = Strength;
                    break;
                case EAbility.Dexterity:
                    attributeValue = Dexterity;
                    break;
                case EAbility.Consititution:
                    attributeValue = Constitution;
                    break;
                case EAbility.Intelligence:
                    attributeValue = Intelligence;
                    break;
                case EAbility.Wisdom:
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
