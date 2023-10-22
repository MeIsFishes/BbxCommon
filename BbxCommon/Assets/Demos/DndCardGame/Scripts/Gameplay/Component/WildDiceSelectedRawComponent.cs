using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BbxCommon;
using BbxCommon.Ui;

namespace Dcg
{
    /// <summary>
    /// 释放技能时选出的自由卡牌存储在这里
    /// </summary>
    public class WildDiceSelectedRawComponent : EcsRawComponent
    {
        public List<Dice> Dices;

        /// <summary>
        /// 判断是否所有槽均被填满
        /// </summary>
        public bool AllSlotFilled()
        {
            foreach (var dice in Dices)
            {
                if (dice == null)
                    return false;
            }
            return true;
        }

        public override void OnAllocate()
        {
            SimplePool.Alloc(out Dices);
        }

        public override void OnCollect()
        {
            Dices.CollectToPool();
        }
    }
}
