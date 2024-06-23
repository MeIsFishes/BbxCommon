using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    /// <summary>
    /// 记录单位的标准动作和附赠动作等信息
    /// </summary>
    public class CombatActionRawComponent : EcsRawComponent
    {
        // 考虑rogue类型游戏可能比较狂野，或许会出现增加每回合动作/附赠动作数的情况，
        // 所以使用int而不是bool记录

        public int Action
        {
            get { return ActionVariable.Value; }
            set { ActionVariable.SetValue(value); }
        }
        public int BonusAction
        {
            get { return BonusActionVariable.Value; }
            set { BonusActionVariable.SetValue(value); }
        }
        /// <summary>
        /// 剩余标准动作数量
        /// </summary>
        public ListenableVariable<int> ActionVariable;
        /// <summary>
        /// 剩余附赠动作数量
        /// </summary>
        public ListenableVariable<int> BonusActionVariable;

        public override void OnCollect()
        {
            ActionVariable.DispatchInvalid();
            BonusActionVariable.DispatchInvalid();
        }
    }
}
