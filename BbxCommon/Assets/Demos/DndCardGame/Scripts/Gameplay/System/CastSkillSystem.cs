using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;
using System.Text;

namespace Dcg
{
    [DisableAutoCreation]
    public partial class CastSkillSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            foreach (var castSkillComp in GetEnumerator<CastSkillRawComponent>())
            {
                if (castSkillComp.RequestCast == false)
                    continue;

                castSkillComp.RequestCast = false;  // 重置请求
                var attacker = castSkillComp.GetEntity();
                var defender = castSkillComp.Target;

                var attackGroup = CreateAttackDiceGroup(castSkillComp);
                var defendGroup = DiceGroup.CreateAcGroup(defender, EAbility.Dexterity);
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
                    var damageGroup = CreateDamageDiceGroup(castSkillComp);
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

                    var attackableComp = attacker.GetRawComponent<AttackableRawComponent>();
                    attackableComp.AddCauseDamageRequest(attacker, defender, damageResult.Amount, DamageType.Slash);
                }

                // 数据回收
                attackGroup.CollectToPool();
                defendGroup.CollectToPool();
                var combatDeckComp = attacker.GetRawComponent<CombatDeckRawComponent>();
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

        // 以下函数都是对技能的临时处理，后面大概需要全部重写

        private DiceGroup CreateAttackDiceGroup(CastSkillRawComponent castSkillComp)
        {
            var dices = SimplePool<List<Dice>>.Alloc();
            if (castSkillComp.GetEntity().GetRawComponent<MonsterRawComponent>() != null)
            {
                var monsterComp = castSkillComp.GetEntity().GetRawComponent<MonsterRawComponent>();
                for (int i = 0; i < monsterComp.AttackDices.Count; i++)
                {
                    dices.Add(Dice.Create(monsterComp.AttackDices[i]));
                }
                var diceGroup = DiceGroup.Create(castSkillComp.GetEntity(), dices, monsterComp.Modifier);
                dices.CollectToPool();
                return diceGroup;
            }
            else if (castSkillComp.ChosenSkill == "Sword")
            {
                dices.Add(castSkillComp.WildDices[0]);
                dices.Add(Dice.Create(EDiceType.D4));
                var diceGroup = DiceGroup.Create(castSkillComp.GetEntity(), dices, EAbility.Strength);
                dices.CollectToPool();
                return diceGroup;
            }
            else if (castSkillComp.ChosenSkill == "Dagger")
            {
                dices.Add(castSkillComp.WildDices[0]);
                dices.Add(castSkillComp.WildDices[1]);
                var diceGroup = DiceGroup.Create(castSkillComp.GetEntity(), dices, EAbility.Dexterity);
                dices.CollectToPool();
                return diceGroup;
            }
            return null;
        }

        private DiceGroup CreateDamageDiceGroup(CastSkillRawComponent castSkillComp)
        {
            var dices = SimplePool<List<Dice>>.Alloc();
            if (castSkillComp.GetEntity().GetRawComponent<MonsterRawComponent>() != null)
            {
                var monsterComp = castSkillComp.GetEntity().GetRawComponent<MonsterRawComponent>();
                for (int i = 0; i < monsterComp.DamageDices.Count; i++)
                {
                    dices.Add(Dice.Create(monsterComp.DamageDices[i]));
                }
                var diceGroup = DiceGroup.Create(castSkillComp.GetEntity(), dices, monsterComp.Modifier);
                dices.CollectToPool();
                return diceGroup;
            }
            else if (castSkillComp.ChosenSkill == "Sword")
            {
                dices.Add(castSkillComp.WildDices[1]);
                dices.Add(Dice.Create(EDiceType.D4));
                var diceGroup = DiceGroup.Create(castSkillComp.GetEntity(), dices, EAbility.Strength);
                dices.CollectToPool();
                return diceGroup;
            }
            else if (castSkillComp.ChosenSkill == "Dagger")
            {
                dices.Add(Dice.Create(EDiceType.D4));
                dices.Add(Dice.Create(EDiceType.D4));
                var diceGroup = DiceGroup.Create(castSkillComp.GetEntity(), dices, EAbility.Dexterity);
                dices.CollectToPool();
                return diceGroup;
            }
            return null;
        }
    }
}
