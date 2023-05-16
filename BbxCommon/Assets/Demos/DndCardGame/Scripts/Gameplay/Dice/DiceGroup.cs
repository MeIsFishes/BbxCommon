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
        /// 玩家主动拖入的骰子
        /// </summary>
        public DiceList RollingDices;
        /// <summary>
        /// 基础值和属性加值
        /// </summary>
        public DiceList BaseAndModifier;
        /// <summary>
        /// buff加值
        /// </summary>
        public DiceList BuffModifier;

        /// <param name="diceGroup"> 掷骰的场景 </param>
        /// <param name="entity"> 掷骰主体 </param>
        /// <param name="attrbuteModifier"> 属性加值 </param>
        /// <param name="savingThrow"> 属性豁免 </param>
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
