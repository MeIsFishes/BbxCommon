using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    public class OperationAttack : BlockedOperationBase
    {
        public List<Dice> AttackBaseDices;
        public List<Dice> DamageBaseDices;
        public Entity Attacker;
        public EAbility AttackModifier;

        public Entity Defender;
        public EAbility DefendModifier;

        protected override void OnEnter()
        {
            var attackGroup = DiceGroup.Create(Attacker, AttackBaseDices, AttackModifier);
            var defendGroup = DiceGroup.CreateAcGroup(Defender, DefendModifier);
            var attackResult = attackGroup.GetGroupResult();
            var defendResult = defendGroup.GetGroupResult();
            bool hit = attackResult.Amount >= defendResult.Amount;
            bool crit = attackResult.Amount >= defendResult.Amount * 2.5f;

            StringBuilder sb = new StringBuilder();
            sb.Append("攻击方掷骰：\n");
            GenerateDiceGroupString(sb, attackGroup, attackResult);
            sb.Append("\n防御方掷骰：\n");
            GenerateDiceGroupString(sb, defendGroup, defendResult);
            if (crit)
            {
                sb.Append("攻击骰达到防御骰的2.5倍，暴击命中！");
            }
            else
            {
                if (hit)
                    sb.Append("\n攻击命中！");
                else
                    sb.Append("\n攻击未命中！");
            }
            var diceGroupString = sb.ToString();
            sb.Clear();
            UiApi.GetUiController<UiTipController>().ShowTip("攻击掷骰结果", diceGroupString);

            if (hit)
            {
                var damageGroup = DiceGroup.Create(Attacker, DamageBaseDices, AttackModifier);
                if (crit)
                {
                    var baseDicesCount = damageGroup.BaseDices.Dices.Count;
                    for (int i = 0; i < baseDicesCount; i++)
                    {
                        damageGroup.BaseDices.Dices.Add(Dice.Create(damageGroup.BaseDices.Dices[i].DiceType));
                    }
                }
                var damageResult = damageGroup.GetGroupResult();
                GenerateDiceGroupString(sb, damageGroup, damageResult);
                var damageGroupString = sb.ToString();
                sb.Clear();
                UiApi.GetUiController<UiTipController>().ShowTip("伤害结果", damageGroupString);
                damageGroup.CollectToPool();

                var attackableComp = Attacker.GetRawComponent<AttackableRawComponent>();
                attackableComp.AddCauseDamageRequest(Attacker, Defender, damageResult.Amount);
            }

            attackGroup.CollectToPool();
            defendGroup.CollectToPool();
        }

        private void GenerateDiceGroupString(StringBuilder sb, DiceGroup diceGroup, DiceGroupResult result)
        {
            sb.Append("基础骰：");
            foreach (var dice in diceGroup.BaseDices.Dices)
            {
                sb.Append(dice.DiceType.ToString());
                sb.Append(", ");
            }
            sb.Append("结果：");
            foreach (var res in result.BaseDicesResult.DiceResults)
            {
                sb.Append(res.ToString());
                sb.Append(", ");
            }
            sb.Append("\n");

            sb.Append("属性调整骰：");
            foreach (var dice in diceGroup.AbilityModifier.Dices)
            {
                sb.Append(dice.DiceType.ToString());
                sb.Append(", ");
            }
            sb.Append("结果：");
            foreach (var res in result.AbilityModifierResult.DiceResults)
            {
                sb.Append(res.ToString());
                sb.Append(", ");
            }
            sb.Append("\n");

            sb.Append("Buff调整骰：");
            foreach (var dice in diceGroup.BuffModifier.Dices)
            {
                sb.Append(dice.DiceType.ToString());
                sb.Append(", ");
            }
            sb.Append("结果：");
            foreach (var res in result.BuffModifierResult.DiceResults)
            {
                sb.Append(res.ToString());
                sb.Append(", ");
            }
            sb.Append("\n");

            sb.Append("最终结果：");
            sb.Append(result.Amount.ToString());
            sb.Append("\n");
        }

        public override void OnAllocate()
        {
            AttackBaseDices = SimplePool<List<Dice>>.Alloc();
            DamageBaseDices = SimplePool<List<Dice>>.Alloc();
        }

        public override void OnCollect()
        {
            AttackBaseDices.CollectAndClearElements(true);
            DamageBaseDices.CollectAndClearElements(true);
        }
    }
}
