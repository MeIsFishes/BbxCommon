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
            // ���ν���ս��ʱ�ĳ�ʼ��
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
            // ������һ�غ�
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
