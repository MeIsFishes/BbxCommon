using System;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using Dcg.Ui;

namespace Dcg
{
    [DisableAutoCreation, UpdateBefore(typeof(CombatRoundSystem))]
    public class CombatEndTurnSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var combatTurnComp in GetEnumerator<CombatTurnRawComponent>())
            {
                if (combatTurnComp.RequestEnd == true)
                {
                    combatTurnComp.RequestEnd = false;
                    combatTurnComp.DuringTurn = false;
                    var combatRoundComp = EcsApi.GetSingletonRawComponent<CombatRoundSingletonRawComponent>();
                    combatRoundComp.EndTurn = true;
                    var entity = combatTurnComp.GetEntity();
                    var combatDeckComp = entity.GetRawComponent<CombatDeckRawComponent>();
                    if (combatDeckComp != null)
                    {
                        combatDeckComp.DiscardAllHandDices();
                        var castSkillComp = entity.GetRawComponent<CastSkillRawComponent>();
                        if (castSkillComp != null)
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
            }
        }
    }
}
