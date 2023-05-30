using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    public class CombatTurnSingletonRawComponent : EcsSingletonRawComponent
    {
        /// <summary>
        /// 当前行动的<see cref="Entity"/>
        /// </summary>
        public Entity CurrentEntity;
        /// <summary>
        /// 是否还能进行动作
        /// </summary>
        public bool HasAction;
        /// <summary>
        /// 是否还能进行附赠动作
        /// </summary>
        public bool HasBonusAction;
        /// <summary>
        /// 操作者是否请求结束回合
        /// </summary>
        public bool EndTurn;

        /// <summary>
        /// 用于新的一回合开始时刷新回合数据
        /// </summary>
        /// <param name="actEntity"> 该回合行动的<see cref="Entity"/> </param>
        public void RefreshTurn(Entity actEntity)
        {
            CurrentEntity = actEntity;
            HasAction = true;
            HasBonusAction = true;
            EndTurn = false;
        }
    }
}
