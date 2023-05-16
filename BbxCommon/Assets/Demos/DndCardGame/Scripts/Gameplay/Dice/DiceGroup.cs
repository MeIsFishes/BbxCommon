using Unity.Entities;

namespace Dcg
{
    public enum EDiceGroup
    {
        Attack,
        BeAttacked,
        Damage,
    }

    public class DiceGroup
    {
        /// <summary>
        /// ����������������
        /// </summary>
        public DiceList RollingDices;
        /// <summary>
        /// ����ֵ�����Լ�ֵ
        /// </summary>
        public DiceList BaseAndModifier;
        /// <summary>
        /// buff��ֵ
        /// </summary>
        public DiceList BuffModifier;

        /// <param name="diceGroup"> �����ĳ��� </param>
        /// <param name="entity"> �������� </param>
        /// <param name="attrbuteModifier"> ���Լ�ֵ </param>
        /// <param name="savingThrow"> ���Ի��� </param>
        public static DiceList Create(EDiceGroup diceGroup, Entity entity, EAttribute attrbuteModifier, EAttribute savingThrow)
        {
            return DiceList.Create();
        }

        private static DiceList CreateBaseAndModifier(EDiceGroup diceGroup)
        {
            var diceList = DiceList.Create();
            return diceList;
        }
    }

    public class DiceBattle
    {
        public DiceGroup Camp1;
        public DiceGroup Camp2;
    }
}
