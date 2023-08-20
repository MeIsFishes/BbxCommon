using System.Collections.Generic;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class DiceGroup : PooledObject
    {
        /// <summary>
        /// 基础骰，如武器攻击骰、防御骰
        /// </summary>
        public DiceList BaseDices;
        /// <summary>
        /// 属性加值
        /// </summary>
        public DiceList AbilityModifier;
        /// <summary>
        /// buff加值
        /// </summary>
        public DiceList BuffModifier;

        /// <param name="entity"> 掷骰主体 </param>
        /// <param name="baseDices"> 基础骰，如武器攻击、防御骰 </param>
        /// <param name="abilityModifier"> 属性加值 </param>
        public static DiceGroup Create(Entity entity, List<Dice> baseDices, EAbility abilityModifier)
        {
            var diceGroup = ObjectPool<DiceGroup>.Alloc();
            diceGroup.BaseDices.Dices.AddList(baseDices);
            var attributesComp = entity.GetRawComponent<AttributesRawComponent>();
            var modifier = SimplePool<List<Dice>>.Alloc();
            attributesComp.GetModifierDiceList(abilityModifier, modifier);
            diceGroup.AbilityModifier.Dices.AddList(modifier);
            modifier.CollectAndClearElements(true);
            return diceGroup;
        }

        /// <summary>
        /// 创建一个应用护甲等级的<see cref="DiceGroup"/>。
        /// </summary>
        public static DiceGroup CreateAcGroup(Entity entity, EAbility abilityModifier)
        {
            var ac = entity.GetRawComponent<AttributesRawComponent>().ArmorClass;
            var acList = SimplePool<List<Dice>>.Alloc();
            foreach (var diceType in ac)
            {
                acList.Add(Dice.Create(diceType));
            }
            var diceGroup = Create(entity, acList, abilityModifier);
            acList.CollectAndClearElements(true);
            return diceGroup;
        }

        public DiceGroupResult GetGroupResult()
        {
            var result = new DiceGroupResult();
            result.BaseDicesResult = BaseDices.GetListResult();
            result.AbilityModifierResult = AbilityModifier.GetListResult();
            result.BuffModifierResult = BuffModifier.GetListResult();
            result.Amount = BaseDices.GetListResult().Amount + AbilityModifier.GetListResult().Amount + BuffModifier.GetListResult().Amount;
            return result;
        }

        public override void OnAllocate()
        {
            BaseDices = ObjectPool<DiceList>.Alloc();
            AbilityModifier = ObjectPool<DiceList>.Alloc();
            BuffModifier = ObjectPool<DiceList>.Alloc();
        }

        public override void OnCollect()
        {
            BaseDices.CollectToPool();
            AbilityModifier.CollectToPool();
            BuffModifier.CollectToPool();
        }
    }

    public struct DiceGroupResult
    {
        public int Amount;
        public DiceListResult BaseDicesResult;
        public DiceListResult AbilityModifierResult;
        public DiceListResult BuffModifierResult;
    }
}
