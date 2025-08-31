using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;
using NUnit.Framework;

namespace Dcg
{
    /// <summary>
    /// 用来存放玩家在战斗阶段输入的地方，如使用哪个技能、选择了哪些自由骰
    /// </summary>
    public class CastSkillRawComponent : EcsRawComponent, IListenable
    {
        public enum EUiEvent
        {
            WildDicesRefresh,
        }

        public bool RequestCast;
        /// <summary>
        /// 这个释放目标是临时的处理，后面至少要改成记录entity的id，而且或许会改成直接记录地块
        /// </summary>
        public Entity Target;
        /// <summary>
        /// 临时处理，后面需要将类型替换为技能实例
        /// </summary>
        public string ChosenSkill;
        public List<Dice> WildDices = new();
        /// <summary>
        /// 自由骰的槽位数量
        /// </summary>
        public int WildDiceSlotCount
        {
            get
            {
                return WildDices.Count;
            }
            set
            {
                bool changed = WildDices.Count == value;
                WildDices.ModifyCount(value);
                if (changed) DispatchEvent(EUiEvent.WildDicesRefresh);
            }
        }

        private MessageHandler<int> m_MessageHandler = new();
        IMessageDispatcher<int> IListenable.MessageDispatcher => m_MessageHandler;

        public void DispatchEvent(EUiEvent e)
        {
            m_MessageHandler.Dispatch((int)e, this);
        }

        /// <summary>
        /// 判断是否所有自由骰槽位均被填满
        /// </summary>
        public bool AllSlotFilled()
        {
            foreach (var dice in WildDices)
            {
                if (dice == null)
                    return false;
            }
            return true;
        }

        protected override void OnAllocate()
        {
            SimplePool.Alloc(out WildDices);
        }

        protected override void OnCollect()
        {
            WildDices.CollectToPool();
        }
    }
}
