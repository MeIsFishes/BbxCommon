using UnityEngine;
using Unity.Entities;
using BbxCommon;

namespace Dcg
{
    [DisableAutoCreation, UpdateAfter(typeof(CauseDamageSystem))]
    public partial class TakeDamageSystem : EcsMixSystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var attackableComp in GetEnumerator<AttackableRawComponent>())
            {
                // 没有伤害请求，返回
                if (attackableComp.TakeDamageRequests.Count < 1)
                {
                    continue;
                }

                var attributesRawComponent = attackableComp.GetEntity().GetRawComponent<AttributesRawComponent>();

                // 取出伤害请求
                var damageRequest = attackableComp.TakeDamageRequests[0];
                attackableComp.TakeDamageRequests.RemoveAt(0);
                // 处理造成伤害时的增益
                attackableComp.OnTakeDamage?.Invoke(damageRequest);
                // 伤害和治疗公用逻辑
                attributesRawComponent.CurHp -= damageRequest.Damage;
                if (attributesRawComponent.CurHp > attributesRawComponent.MaxHp)
                {
                    attributesRawComponent.CurHp = attributesRawComponent.MaxHp;
                }
                if (attributesRawComponent.CurHp <= 0)
                {
                    attributesRawComponent.CurHp = 0;
                }

                // 回收
                damageRequest.CollectToPool();
            }
        }
    }
}