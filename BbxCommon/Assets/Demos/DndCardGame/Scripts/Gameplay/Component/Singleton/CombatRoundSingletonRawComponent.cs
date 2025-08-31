using System.Collections.Generic;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class CombatRoundSingletonRawComponent : EcsSingletonRawComponent
    {
        /// <summary>
        /// 记录一轮中各个单位的行动顺序，其中存储其对应的<see cref="Entity"/>
        /// </summary>
        public List<Entity> RoundOrder;     // todo:需要改成存储UniqueId
        public int CurrentRound;
        public int CurrentTurn;
        public bool EndTurn;
        /// <summary>
        /// 若为真，表示刚刚进入战斗
        /// </summary>
        public bool EnterCombat;

        protected override void OnAllocate()
        {
            SimplePool.Alloc(out RoundOrder);
        }

        protected override void OnCollect()
        {
            RoundOrder.CollectToPool();
        }
    }
}
