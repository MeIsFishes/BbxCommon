using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg.Ui
{
    public class UiAttackController : UiControllerBase<UiAttackView>
    {
        private Entity m_Entity;

        protected override void OnUiInit()
        {
            m_View.WeaponOptions.NameWrapper.AddOnClickCallback("Sword", OnSwordButton);
            m_View.WeaponOptions.NameWrapper.AddOnClickCallback("Dagger", OnDaggerButton);
            m_View.AttackButton.onClick.AddListener(OnAttackButton);
        }

        private void OnSwordButton()
        {
            m_View.Description.text = "掷出1个自由骰+1d4的攻击骰\n造成1个自由骰+1d4的伤害";
        }

        private void OnDaggerButton()
        {
            m_View.Description.text = "掷出2个自由骰的攻击骰\n造成2d4的伤害";
        }

        private void OnAttackButton()
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
