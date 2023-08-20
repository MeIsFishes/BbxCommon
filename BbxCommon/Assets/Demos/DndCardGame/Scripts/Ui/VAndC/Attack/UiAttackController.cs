using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiAttackController : UiControllerBase<UiAttackView>
    {
        protected override void OnUiInit()
        {
            m_View.Button.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton()
        {
            var operationComp = EcsApi.GetSingletonRawComponent<OperationRequestSingletonRawComponent>();
            var combatInfoComp = EcsApi.GetSingletonRawComponent<CombatInfoSingletonRawComponent>();
            var operationAttack = ObjectPool<OperationAttack>.Alloc();
            operationAttack.Attacker = combatInfoComp.Character;
            operationAttack.AttackBaseDices.Add(Dice.Create(EDiceType.D12));
            operationAttack.AttackModifier = EAbility.Strength;
            operationAttack.Defender = combatInfoComp.Monster;
            operationAttack.DefendModifier = EAbility.Dexterity;
            operationAttack.DamageBaseDices.Add(Dice.Create(EDiceType.D10));
            operationComp.AddBlockedOperation(operationAttack);
        }
    }
}
