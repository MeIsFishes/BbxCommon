using System.Text;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

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
            bool succeeded = attackResult.Amount > defendResult.Amount;

            StringBuilder sb = new StringBuilder();
            sb.Append("������������\n");
            GenerateDiceGroupString(sb, attackGroup, attackResult);
            sb.Append("\n������������\n");
            GenerateDiceGroupString(sb, defendGroup, defendResult);
            if (succeeded)
                sb.Append("\n�������У�");
            else
                sb.Append("\n����δ���У�");
            var diceGroupString = sb.ToString();
            sb.Clear();
            Debug.Log(diceGroupString);
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
