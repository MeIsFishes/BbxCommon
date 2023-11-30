using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    /// <summary>
    /// 执行怪物的回合
    /// </summary>
    [DisableAutoCreation, UpdateBefore(typeof(CombatEndTurnSystem))]
    public partial class CombatMonsterTurnSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var monsterComp in GetEnumerator<MonsterRawComponent>())
            {
                var entity = monsterComp.GetEntity();
                var combatTurnComp = entity.GetRawComponent<CombatTurnRawComponent>();
                if (combatTurnComp == null || combatTurnComp.DuringTurn == false)
                    continue;
                var aiComp = entity.GetRawComponent<AiBehaviourRawComponent>();
                if (aiComp == null)
                    continue;
                var castSkillComp = entity.GetRawComponent<CastSkillRawComponent>();
                aiComp.ElapsedTime += TimeApi.DeltaTime;
                // 执行动作
                if (aiComp.ElapsedTime > AiBehaviourRawComponent.ActionDelay && aiComp.DidAction == false)
                {
                    aiComp.DidAction = true;
                    castSkillComp.RequestCast = true;
                    castSkillComp.Target = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>().Character;
                }
                // 结束回合
                if (aiComp.ElapsedTime > AiBehaviourRawComponent.EndTurnDelay && aiComp.DidEndTurn == false)
                {
                    aiComp.DidEndTurn = true;
                    combatTurnComp.RequestEnd = true;
                    // 重置标志位
                    aiComp.ElapsedTime = 0;
                    aiComp.DidAction = false;
                    aiComp.DidEndTurn = false;
                }
            }
        }
    }
}
