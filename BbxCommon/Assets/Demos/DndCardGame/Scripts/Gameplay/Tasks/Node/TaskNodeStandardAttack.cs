using System;
using System.Collections.Generic;
using System.Text;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;
using UnityEditor.Experimental.GraphView;

namespace Dcg
{
    public class TaskNodeStandardAttack : TaskBase
    {
        public EntityID AttackerEntityId;//攻击者，来自context
        public EntityID TargetEntityId;//目标者，来自context
        public List<EDiceType> BaseAttackDiceList;//基础攻击骰
        public List<EDiceType> BaseDamageDiceList;//基础伤害骰

        public List<uint> AttackWildDiceIndexList;//攻击自由骰index列表，WildDices下标
        public List<uint> DamageWildDiceIndexList;//伤害自由骰index列表，WildDices下标

        public List<Dice> WildDiceList;//自由骰列表，来自context

        public DamageType AttackDamageType;//攻击类型
        public EAbility AttackAbilityModifier;//攻击属性加值
        public EAbility DamageAbilityModifier;//伤害属性加值
        public EAbility ACAbilityModifier;//AC属性加值


        public enum EField
        {
            AttackerEntityId,
            TargetEntityId,
            BaseAttackDice,
            BaseDamageDice,
            AttackWildDiceIndexList,
            DamageWildDiceIndexList,
            WildDiceList,
            AttackDamageType,
            AttackAbilityModifier,
            DamageAbilityModifier,
            ACAbilityModifier
        }

        protected override Type GetFieldEnumType()
        {
            return typeof(EField);
        }

        protected override void RegisterFields()
        {
            RegisterField(EField.AttackerEntityId, AttackerEntityId);
            RegisterField(EField.TargetEntityId, TargetEntityId);
            RegisterField(EField.BaseAttackDice, BaseAttackDiceList);
            RegisterField(EField.BaseDamageDice, BaseDamageDiceList);
            RegisterField(EField.AttackWildDiceIndexList, AttackWildDiceIndexList);
            RegisterField(EField.DamageWildDiceIndexList, DamageWildDiceIndexList);
            RegisterField(EField.WildDiceList, WildDiceList);
            RegisterField(EField.AttackDamageType, AttackDamageType);
            RegisterField(EField.AttackAbilityModifier, AttackAbilityModifier);
            RegisterField(EField.DamageAbilityModifier, DamageAbilityModifier);
            RegisterField(EField.ACAbilityModifier, ACAbilityModifier);
        }

        public override void ReadFieldInfo(int fieldEnum, TaskFieldInfo fieldInfo, TaskContextBase context)
        {
            switch (fieldEnum)
            {
                case (int)EField.AttackerEntityId:
                    AttackerEntityId = ReadValue<EntityID>(fieldInfo, context);
                    break;
                case (int)EField.TargetEntityId:
                    TargetEntityId = ReadValue<EntityID>(fieldInfo, context);
                    break;
                case (int)EField.BaseAttackDice:
                    BaseAttackDiceList = ReadList(fieldInfo, context, BaseAttackDiceList);
                    break;
                case (int)EField.BaseDamageDice:
                    BaseDamageDiceList = ReadList(fieldInfo, context, BaseDamageDiceList);
                    break;
                case (int)EField.AttackWildDiceIndexList:
                    AttackWildDiceIndexList = ReadList(fieldInfo, context, AttackWildDiceIndexList);
                    break;
                case (int)EField.DamageWildDiceIndexList:
                    DamageWildDiceIndexList = ReadList(fieldInfo, context, DamageWildDiceIndexList);
                    break;
                case (int)EField.WildDiceList:
                    WildDiceList = ReadList(fieldInfo, context, WildDiceList);
                    break;
                case (int)EField.AttackDamageType:
                    AttackDamageType = ReadEnum<DamageType>(fieldInfo, context);
                    break;
                case (int)EField.AttackAbilityModifier:
                    AttackAbilityModifier = ReadEnum<EAbility>(fieldInfo, context);
                    break;
                case (int)EField.DamageAbilityModifier:
                    DamageAbilityModifier = ReadEnum<EAbility>(fieldInfo, context);
                    break;
                case (int)EField.ACAbilityModifier:
                    ACAbilityModifier = ReadEnum<EAbility>(fieldInfo, context);
                    break;
                default:
                    break;
            }
        }

        public override void OnCollect()
        {
            // 清理或重置所有字段
            AttackerEntityId = EntityID.INVALID;
            TargetEntityId = EntityID.INVALID;
            BaseAttackDiceList?.Clear();
            BaseDamageDiceList?.Clear();
            AttackWildDiceIndexList?.Clear();
            DamageWildDiceIndexList?.Clear();
            WildDiceList?.Clear();
            AttackDamageType = default;
            AttackAbilityModifier = default;
            DamageAbilityModifier = default;
            ACAbilityModifier = default;
        }

        protected override void OnEnter()
        {
            var attackGroup = CreateAttackDiceGroup();
            var defendGroup = DiceGroup.CreateAcGroup(TargetEntityId.GetEntity(), ACAbilityModifier);
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

            bool needShowTips = true;
            var gameSetting = UiApi.GetUiModel<UiModelUserOption>();
            if (gameSetting != null)
            {
                needShowTips = gameSetting.TipsNeedShow;
            }

            if (needShowTips)
            {
                UiApi.GetUiController<UiTipController>().ShowTip("攻击掷骰结果", diceGroupString);
            }

            if (hit)
            {
                var damageGroup = CreateDamageDiceGroup();
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
                if (needShowTips)
                {
                    UiApi.GetUiController<UiTipController>().ShowTip("伤害结果", damageGroupString);
                }
                damageGroup.CollectToPool();

                var attackableComp = AttackerEntityId.GetEntity().GetRawComponent<AttackableRawComponent>();
                attackableComp.AddCauseDamageRequest(AttackerEntityId.GetEntity(), TargetEntityId.GetEntity(), damageResult.Amount, AttackDamageType);
            }

            // 数据回收
            attackGroup.CollectToPool();
            defendGroup.CollectToPool();
            var combatDeckComp = AttackerEntityId.GetEntity().GetRawComponent<CombatDeckRawComponent>();
            var castSkillComp = AttackerEntityId.GetEntity().GetRawComponent<CastSkillRawComponent>();
            if (combatDeckComp != null)
            {
                for (int i = 0; i < castSkillComp.WildDices.Count; i++)
                {
                    if (castSkillComp.WildDices[i] != null)
                        combatDeckComp.DicesInDiscard.Add(castSkillComp.WildDices[i]);
                }
                combatDeckComp.DispatchEvent(CombatDeckRawComponent.EUiEvent.DicesInDiscardRefresh);
                castSkillComp.WildDices.Clear();
                castSkillComp.DispatchEvent(CastSkillRawComponent.EUiEvent.WildDicesRefresh);
            }
        }


        protected override ETaskRunState OnUpdate(float deltaTime)
        {
            return ETaskRunState.Succeeded;
        }

        protected override void OnExit()
        {

        }

        private DiceGroup CreateAttackDiceGroup()
        {
            var dices = SimplePool<List<Dice>>.Alloc();

            foreach(var dice in BaseAttackDiceList)//基础攻击骰
            {
                dices.Add(Dice.Create(dice));
            }

            for (int i = 0; i <= WildDiceList.Count; i++)//用于攻击的自由骰
            {
                if (AttackWildDiceIndexList.Contains((uint)i))
                {
                    dices.Add(WildDiceList[i]);
                }
            }

            var diceGroup = DiceGroup.Create(AttackerEntityId.GetEntity(), dices, AttackAbilityModifier);
            dices.CollectToPool();
            return diceGroup;
        }

        private DiceGroup CreateDamageDiceGroup()
        {
            var dices = SimplePool<List<Dice>>.Alloc();
            foreach (var dice in BaseDamageDiceList)//基础伤害骰
            {
                dices.Add(Dice.Create(dice));
            }

            for (int i = 0; i <= WildDiceList.Count; i++)//用于伤害的自由骰
            {
                if (DamageWildDiceIndexList.Contains((uint)i))
                {
                    dices.Add(WildDiceList[i]);
                }
            }

            var diceGroup = DiceGroup.Create(AttackerEntityId.GetEntity(), dices, DamageAbilityModifier);
            dices.CollectToPool();
            return diceGroup;
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
    }
}
