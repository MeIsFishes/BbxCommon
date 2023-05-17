using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public enum EDiceGroup
    {
        Attack,
        BeAttacked,
        Damage,
    }

    public class DiceGroup : PooledObject
    {
        /// <summary>
        /// ����������������������������
        /// </summary>
        public DiceList BaseDices;
        /// <summary>
        /// ����������������
        /// </summary>
        public DiceList RollingDices;
        /// <summary>
        /// ����ֵ�����Լ�ֵ
        /// </summary>
        public DiceList AttributeModifier;
        /// <summary>
        /// buff��ֵ
        /// </summary>
        public DiceList BuffModifier;

        /// <param name="diceGroup"> �����ĳ��� </param>
        /// <param name="entity"> �������� </param>
        /// <param name="attrbuteModifier"> ���Լ�ֵ </param>
        /// <param name="savingThrow"> ���Ի��� </param>
        public static DiceGroup Create(EDiceGroup diceGroup, Entity entity, EAttribute attrbuteModifier, EAttribute savingThrow)
        {
            return ObjectPool<DiceGroup>.Alloc();
        }

        private static DiceList CreateBaseAndModifier(EDiceGroup diceGroup)
        {
            var diceList = DiceList.Create();
            return diceList;
        }

        public override void OnCollect()
        {
            BaseDices.CollectToPool();
            RollingDices.CollectToPool();
            AttributeModifier.CollectToPool();
            BuffModifier.CollectToPool();
        }
    }

    public class DiceBattle
    {
        public DiceGroup Camp1;
        public DiceGroup Camp2;
    }
}
