using System.Collections.Generic;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class CombatRoundSingletonRawComponent : EcsSingletonRawComponent
    {
        /// <summary>
        /// ��¼һ���и�����λ���ж�˳�����д洢���Ӧ��<see cref="Entity"/>
        /// </summary>
        public List<Entity> RoundOrder;     // todo:��Ҫ�ĳɴ洢UniqueId
        public int CurrentRound;
        public int CurrentTurn;
        public bool EndTurn;
        /// <summary>
        /// ��Ϊ�棬��ʾ�ոս���ս��
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
