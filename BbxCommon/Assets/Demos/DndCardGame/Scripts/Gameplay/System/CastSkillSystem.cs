using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;
using System.Text;
using UnityEditor.SceneManagement;
using NUnit.Framework;

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

                //攻击转为task
                var taskTest = TaskApi.CreateTaskInfo<TaskContextCastSkill>("CastSkill", 0);
                var timelineInfo = taskTest.CreateTaskTimelineValueInfo(0, 0.5f);
                timelineInfo.AddTimelineInfo(0f, 0.5f, 1);
                timelineInfo.AddTimelineInfo(0.5f, 0f, 2);


                var jumpNode = taskTest.CreateTaskValueInfo<TaskNodeJump>(1);
                jumpNode.AddFieldInfoFromContext(TaskNodeJump.EField.AttackerEntityId, TaskContextCastSkill.EField.AttackerEntityId);
                jumpNode.AddFieldInfo(TaskNodeJump.EField.JumpHeight, 2f);

                var StandardAttackNode = taskTest.CreateTaskValueInfo<TaskNodeStandardAttack>(2);

                StandardAttackNode.AddFieldInfoFromContext(TaskNodeStandardAttack.EField.AttackerEntityId, TaskContextCastSkill.EField.AttackerEntityId);
                StandardAttackNode.AddFieldInfoFromContext(TaskNodeStandardAttack.EField.TargetEntityId, TaskContextCastSkill.EField.TargetEntityId);
                StandardAttackNode.AddFieldInfoFromContext(TaskNodeStandardAttack.EField.WildDiceList, TaskContextCastSkill.EField.WildDices);

                List<EDiceType> baseAttackDices = new();
                List<EDiceType> baseDamageDices = new();
                List<uint> attackWildDiceIndex = new();
                List<uint> damageWildDiceIndex = new();
                EAbility damageModifier = EAbility.None;
                EAbility attackModifier = EAbility.None;
                EAbility ACModifier = EAbility.None;
                DamageType damageType = DamageType.None;

                //为攻击task提供需要的数据。暂时写死
                if (castSkillComp.GetEntity().GetRawComponent<MonsterRawComponent>() != null)
                {
                    var monsterComp = castSkillComp.GetEntity().GetRawComponent<MonsterRawComponent>();
                    for (int i = 0; i < monsterComp.AttackDices.Count; i++)
                    {
                        baseAttackDices.Add(monsterComp.AttackDices[i]);
                    }
                    for (int i = 0; i < monsterComp.DamageDices.Count; i++)
                    {
                        baseDamageDices.Add(monsterComp.DamageDices[i]);
                    }
                    damageModifier = monsterComp.Modifier;
                    ACModifier = EAbility.Dexterity;
                    damageType = DamageType.Slash;
                }
                else if (castSkillComp.ChosenSkill == "Sword")
                {
                    baseAttackDices.Add(EDiceType.D4);
                    baseDamageDices.Add(EDiceType.D4);
                    attackWildDiceIndex.Add(0);
                    damageWildDiceIndex.Add(1);
                    attackModifier = EAbility.Strength;
                    damageModifier = EAbility.Strength;
                    ACModifier = EAbility.Dexterity;
                    damageType = DamageType.Slash;
                }
                else if (castSkillComp.ChosenSkill == "Dagger")
                {
                    baseDamageDices.Add(EDiceType.D4);
                    baseDamageDices.Add(EDiceType.D4);
                    attackWildDiceIndex.Add(0);
                    attackWildDiceIndex.Add(1);
                    attackModifier = EAbility.Dexterity;
                    damageModifier = EAbility.Dexterity;
                    ACModifier = EAbility.Dexterity;
                    damageType = DamageType.Slash;
                }

                StandardAttackNode.AddFieldInfo(TaskNodeStandardAttack.EField.BaseAttackDice, baseAttackDices);
                StandardAttackNode.AddFieldInfo(TaskNodeStandardAttack.EField.BaseDamageDice, baseDamageDices);
                StandardAttackNode.AddFieldInfo(TaskNodeStandardAttack.EField.AttackWildDiceIndexList, attackWildDiceIndex);
                StandardAttackNode.AddFieldInfo(TaskNodeStandardAttack.EField.DamageWildDiceIndexList, damageWildDiceIndex);
                StandardAttackNode.AddFieldInfo(TaskNodeStandardAttack.EField.AttackDamageType, damageType);
                StandardAttackNode.AddFieldInfo(TaskNodeStandardAttack.EField.AttackAbilityModifier, attackModifier);
                StandardAttackNode.AddFieldInfo(TaskNodeStandardAttack.EField.DamageAbilityModifier, damageModifier);
                StandardAttackNode.AddFieldInfo(TaskNodeStandardAttack.EField.ACAbilityModifier, ACModifier);

                // cast skill context
                var context = ObjectPool<TaskContextCastSkill>.Alloc();
                context.AttackerEntityId = castSkillComp.GetEntity().GetUniqueId();
                context.TargetEntityId = castSkillComp.Target.GetUniqueId();
                context.WildDices = castSkillComp.WildDices;

                TaskApi.RunTask("CastSkill", context);
                context.CollectToPool();
            }
        }
    }
}
