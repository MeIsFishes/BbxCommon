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
            sb.Append("������������\n");
            GenerateDiceGroupString(sb, attackGroup, attackResult);
            sb.Append("\n������������\n");
            GenerateDiceGroupString(sb, defendGroup, defendResult);
            if (crit)
            {
                sb.Append("�������ﵽ��������2.5�����������У�");
            }
            else
            {
                if (hit)
                    sb.Append("\n�������У�");
                else
                    sb.Append("\n����δ���У�");
            }
            var diceGroupString = sb.ToString();
            sb.Clear();
            UiApi.GetUiController<UiTipController>().ShowTip("�����������", diceGroupString);

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
                UiApi.GetUiController<UiTipController>().ShowTip("�˺����", damageGroupString);
                damageGroup.CollectToPool();

                var attackableComp = Attacker.GetRawComponent<AttackableRawComponent>();
                attackableComp.AddCauseDamageRequest(Attacker, Defender, damageResult.Amount);
            }

            attackGroup.CollectToPool();
            defendGroup.CollectToPool();
        }

        private void GenerateDiceGroupString(StringBuilder sb, DiceGroup diceGroup, DiceGroupResult result)
        {
            sb.Append("��������");
            foreach (var dice in diceGroup.BaseDices.Dices)
            {
                sb.Append(dice.DiceType.ToString());
                sb.Append(", ");
            }
            sb.Append("�����");
            foreach (var res in result.BaseDicesResult.DiceResults)
            {
                sb.Append(res.ToString());
                sb.Append(", ");
            }
            sb.Append("\n");

            sb.Append("���Ե�������");
            foreach (var dice in diceGroup.AbilityModifier.Dices)
            {
                sb.Append(dice.DiceType.ToString());
                sb.Append(", ");
            }
            sb.Append("�����");
            foreach (var res in result.AbilityModifierResult.DiceResults)
            {
                sb.Append(res.ToString());
                sb.Append(", ");
            }
            sb.Append("\n");

            sb.Append("Buff��������");
            foreach (var dice in diceGroup.BuffModifier.Dices)
            {
                sb.Append(dice.DiceType.ToString());
                sb.Append(", ");
            }
            sb.Append("�����");
            foreach (var res in result.BuffModifierResult.DiceResults)
            {
                sb.Append(res.ToString());
                sb.Append(", ");
            }
            sb.Append("\n");

            sb.Append("���ս����");
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
