using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation]
    public class CombatRoundSystem : EcsMixSystemBase
    {
        protected override void OnSystemUpdate()
        {
            var combatRoundComp = EcsApi.GetSingletonRawComponent<CombatRoundSingletonRawComponent>();
            // 初次进入战斗时的初始化
            if (combatRoundComp.EnterCombat == true)
            {
                var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
                combatRoundComp.EnterCombat = false;
                combatRoundComp.RoundOrder.Add(combatInfoComp.Character);
                combatRoundComp.RoundOrder.Add(combatInfoComp.Monster);
                combatRoundComp.CurrentRound = 0;
                combatRoundComp.CurrentTurn = combatRoundComp.RoundOrder.Count - 1;
                combatRoundComp.EndTurn = true;
            }
            // 进入下一回合
            if (combatRoundComp.EndTurn == true)
            {
                combatRoundComp.EndTurn = false;
                if (combatRoundComp.CurrentTurn == combatRoundComp.RoundOrder.Count - 1)
                {
                    combatRoundComp.CurrentRound++;
                    combatRoundComp.CurrentTurn = 0;
                }
                else
                {
                    combatRoundComp.CurrentTurn++;
                }
                var entity = combatRoundComp.RoundOrder[combatRoundComp.CurrentTurn];
                var combatTurnComp = entity.GetRawComponent<CombatTurnRawComponent>();
                combatTurnComp.RequestBegin = true;
            }
        }
    }
}
