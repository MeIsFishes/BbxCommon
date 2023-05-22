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
        /// 基础骰，如武器攻击骰、防御骰
        /// </summary>
        public DiceList BaseDices;
        /// <summary>
        /// 玩家主动拖入的骰子
        /// </summary>
        public DiceList RollingDices;
        /// <summary>
        /// 基础值和属性加值
        /// </summary>
        public DiceList AttributeModifier;
        /// <summary>
        /// buff加值
        /// </summary>
        public DiceList BuffModifier;

        /// <param name="diceGroup"> 掷骰的场景 </param>
        /// <param name="entity"> 掷骰主体 </param>
        /// <param name="attrbuteModifier"> 属性加值 </param>
        /// <param name="savingThrow"> 属性豁免 </param>
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
